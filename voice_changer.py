"""
VRChat Voice Changer - Minimal Implementation
PICO4 Mic → Whisper STT → VOICEVOX TTS → Virtual Mic Output

Requirements:
    pip install faster-whisper sounddevice numpy requests scipy

Setup:
    1. Install VB-Audio Virtual Cable: https://vb-audio.com/Cable/
    2. Start VOICEVOX: https://voicevox.hiroshiba.jp/
    3. Set VRChat microphone to "CABLE Output (VB-Audio Virtual Cable)"
"""

import sounddevice as sd
import numpy as np
import requests
import io
import threading
import queue
from scipy.io import wavfile

# ========== CONFIG ==========
VOICEVOX_URL = "http://127.0.0.1:50021"
SPEAKER_ID = 1  # 1=ずんだもん, 3=四国めたん
WHISPER_MODEL = "tiny"  # tiny/base/small/medium/large
SAMPLE_RATE = 16000
SILENCE_THRESHOLD = 0.02
MIN_SPEECH_DURATION = 0.5  # seconds
MAX_SPEECH_DURATION = 10.0  # seconds

# ========== GLOBALS ==========
audio_queue = queue.Queue()
is_running = True


def list_devices():
    """List available audio devices"""
    print("\n=== Audio Devices ===")
    devices = sd.query_devices()
    for i, d in enumerate(devices):
        direction = ""
        if d['max_input_channels'] > 0:
            direction += "IN "
        if d['max_output_channels'] > 0:
            direction += "OUT"
        print(f"  [{i}] {d['name'][:40]:<40} {direction}")
    print()


def init_whisper():
    """Initialize Whisper model"""
    from faster_whisper import WhisperModel
    print(f"Loading Whisper model: {WHISPER_MODEL}...")
    model = WhisperModel(WHISPER_MODEL, device="cuda", compute_type="float16")
    print("Whisper ready.")
    return model


def transcribe(model, audio_data):
    """Transcribe audio to text"""
    segments, _ = model.transcribe(audio_data, language="ja", beam_size=1)
    text = "".join(seg.text for seg in segments).strip()
    return text


def voicevox_tts(text):
    """Convert text to speech using VOICEVOX"""
    if not text:
        return None
    
    # Get audio query
    query = requests.post(
        f"{VOICEVOX_URL}/audio_query",
        params={"text": text, "speaker": SPEAKER_ID},
        timeout=10
    )
    if query.status_code != 200:
        print(f"[VOICEVOX query error: {query.status_code}]")
        return None
    
    # Synthesize
    synth = requests.post(
        f"{VOICEVOX_URL}/synthesis",
        params={"speaker": SPEAKER_ID},
        json=query.json(),
        timeout=30
    )
    if synth.status_code != 200:
        print(f"[VOICEVOX synthesis error: {synth.status_code}]")
        return None
    
    return synth.content


def play_audio(wav_bytes, output_device):
    """Play WAV audio to specified device"""
    rate, data = wavfile.read(io.BytesIO(wav_bytes))
    data = data.astype(np.float32) / 32768.0
    sd.play(data, rate, device=output_device)
    sd.wait()


def audio_callback(indata, frames, time, status):
    """Audio input callback"""
    audio_queue.put(indata.copy())


def process_loop(model, output_device):
    """Main processing loop"""
    global is_running
    buffer = []
    is_speaking = False
    silence_count = 0
    
    while is_running:
        try:
            chunk = audio_queue.get(timeout=0.1)
        except queue.Empty:
            continue
        
        level = np.abs(chunk).mean()
        
        if level > SILENCE_THRESHOLD:
            if not is_speaking:
                is_speaking = True
                print("🎤 Listening...")
            buffer.append(chunk)
            silence_count = 0
        elif is_speaking:
            buffer.append(chunk)
            silence_count += 1
            
            # End of speech detection
            duration = len(buffer) * len(chunk) / SAMPLE_RATE
            if silence_count > 5 or duration > MAX_SPEECH_DURATION:
                if duration >= MIN_SPEECH_DURATION:
                    # Process speech
                    audio_data = np.concatenate(buffer).flatten()
                    print("📝 Transcribing...")
                    text = transcribe(model, audio_data)
                    
                    if text:
                        print(f"💬 {text}")
                        print("🔊 Generating voice...")
                        wav = voicevox_tts(text)
                        if wav:
                            play_audio(wav, output_device)
                            print("✓ Done")
                    else:
                        print("(no text detected)")
                
                buffer = []
                is_speaking = False
                silence_count = 0


def main():
    global is_running
    
    print("=" * 50)
    print("  VRChat Voice Changer")
    print("  PICO4 → Whisper → VOICEVOX → VRChat")
    print("=" * 50)
    
    list_devices()
    
    # Device selection
    input_device = int(input("Input device (PICO4 mic): "))
    output_device = int(input("Output device (VB-Cable Input): "))
    
    # Check VOICEVOX
    try:
        r = requests.get(f"{VOICEVOX_URL}/speakers", timeout=5)
        if r.status_code == 200:
            print("✓ VOICEVOX connected")
    except:
        print("✗ VOICEVOX not running! Start it first.")
        return
    
    # Init Whisper
    model = init_whisper()
    
    # Start processing thread
    processor = threading.Thread(target=process_loop, args=(model, output_device))
    processor.start()
    
    # Start audio input
    print("\n🎧 Running... Press Ctrl+C to stop\n")
    with sd.InputStream(
        device=input_device,
        channels=1,
        samplerate=SAMPLE_RATE,
        blocksize=int(SAMPLE_RATE * 0.1),
        callback=audio_callback
    ):
        try:
            while True:
                sd.sleep(100)
        except KeyboardInterrupt:
            print("\nStopping...")
            is_running = False
    
    processor.join()
    print("Done.")


if __name__ == "__main__":
    main()
