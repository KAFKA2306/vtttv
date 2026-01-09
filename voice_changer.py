import sounddevice as sd
import numpy as np
import requests
import io
import threading
import queue
from scipy.io import wavfile

VOICEVOX_URL = "http://127.0.0.1:50021"
SPEAKER_ID = 1
WHISPER_MODEL = "tiny"
SAMPLE_RATE = 16000
SILENCE_THRESHOLD = 0.02
MIN_SPEECH_DURATION = 0.5
MAX_SPEECH_DURATION = 10.0

audio_queue = queue.Queue()
is_running = True

def list_devices():
    devices = sd.query_devices()
    for i, d in enumerate(devices):
        direction = ("IN " if d['max_input_channels'] > 0 else "") + ("OUT" if d['max_output_channels'] > 0 else "")
        print(f"  [{i}] {d['name'][:40]:<40} {direction}")

def init_whisper():
    from faster_whisper import WhisperModel
    return WhisperModel(WHISPER_MODEL, device="cuda", compute_type="float16")

def transcribe(model, audio_data):
    segments, _ = model.transcribe(audio_data, language="ja", beam_size=1)
    return "".join(seg.text for seg in segments).strip()

def voicevox_tts(text):
    if not text: return None
    q = requests.post(f"{VOICEVOX_URL}/audio_query", params={"text": text, "speaker": SPEAKER_ID}, timeout=10)
    if q.status_code != 200: return None
    s = requests.post(f"{VOICEVOX_URL}/synthesis", params={"speaker": SPEAKER_ID}, json=q.json(), timeout=30)
    return s.content if s.status_code == 200 else None

def play_audio(wav_bytes, output_device):
    rate, data = wavfile.read(io.BytesIO(wav_bytes))
    sd.play(data.astype(np.float32) / 32768.0, rate, device=output_device)
    sd.wait()

def audio_callback(indata, frames, time, status):
    audio_queue.put(indata.copy())

def process_loop(model, output_device):
    global is_running
    buffer, is_speaking, silence_count = [], False, 0
    while is_running:
        try: chunk = audio_queue.get(timeout=0.1)
        except queue.Empty: continue
        if np.abs(chunk).mean() > SILENCE_THRESHOLD:
            if not is_speaking:
                is_speaking = True
                print("🎤 Listening...")
            buffer.append(chunk)
            silence_count = 0
        elif is_speaking:
            buffer.append(chunk)
            silence_count += 1
            duration = len(buffer) * len(chunk) / SAMPLE_RATE
            if silence_count > 5 or duration > MAX_SPEECH_DURATION:
                if duration >= MIN_SPEECH_DURATION:
                    audio_data = np.concatenate(buffer).flatten()
                    print("📝 Transcribing...")
                    text = transcribe(model, audio_data)
                    if text:
                        print(f"💬 {text}\n🔊 Generating...")
                        wav = voicevox_tts(text)
                        if wav:
                            play_audio(wav, output_device)
                            print("✓ Done")
                buffer, is_speaking, silence_count = [], False, 0

def main():
    global is_running
    list_devices()
    input_device = int(input("Input device ID: "))
    output_device = int(input("Output device ID: "))
    try:
        if requests.get(f"{VOICEVOX_URL}/speakers", timeout=5).status_code != 200:
            raise Exception("VOICEVOX error")
    except:
        print("✗ VOICEVOX not running!"); return
    model = init_whisper()
    threading.Thread(target=process_loop, args=(model, output_device), daemon=True).start()
    with sd.InputStream(device=input_device, channels=1, samplerate=SAMPLE_RATE, blocksize=int(SAMPLE_RATE * 0.1), callback=audio_callback):
        try:
            while True: sd.sleep(100)
        except KeyboardInterrupt:
            is_running = False
    print("Done.")

if __name__ == "__main__":
    main()
