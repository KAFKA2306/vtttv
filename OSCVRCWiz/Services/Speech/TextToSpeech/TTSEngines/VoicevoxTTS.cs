using OSCVRCWiz.Resources.Audio;
using OSCVRCWiz.Services.Text;
using System.Net;
using System.Text;
using System.Text.Json;

namespace OSCVRCWiz.Services.Speech.TextToSpeech.TTSEngines
{
    public class VoicevoxTTS
    {
        private static readonly HttpClient client = new HttpClient() { Timeout = TimeSpan.FromSeconds(30) };
        public static string BaseUrl = "http://127.0.0.1:50021";
        public static List<(int id, string name)> Speakers = new List<(int, string)>();

        public static async Task SynthesizeSpeech(TTSMessageQueue.TTSMessage TTSMessageQueued, CancellationToken ct = default)
        {
            try
            {
                int speakerId = 1;
                if (int.TryParse(TTSMessageQueued.Voice.Split('|')[0], out int parsed))
                    speakerId = parsed;

                var queryUrl = $"{BaseUrl}/audio_query?text={Uri.EscapeDataString(TTSMessageQueued.text)}&speaker={speakerId}";
                var queryResponse = await client.PostAsync(queryUrl, null, ct);
                if (!queryResponse.IsSuccessStatusCode)
                {
                    OutputText.outputLog($"[VOICEVOX Query Error: {queryResponse.StatusCode}]", Color.Red);
                    Task.Run(() => TTSMessageQueue.PlayNextInQueue());
                    return;
                }

                var queryJson = await queryResponse.Content.ReadAsStringAsync(ct);

                var synthUrl = $"{BaseUrl}/synthesis?speaker={speakerId}";
                var synthRequest = new HttpRequestMessage(HttpMethod.Post, synthUrl)
                {
                    Content = new StringContent(queryJson, Encoding.UTF8, "application/json")
                };
                var synthResponse = await client.SendAsync(synthRequest, ct);
                if (!synthResponse.IsSuccessStatusCode)
                {
                    OutputText.outputLog($"[VOICEVOX Synthesis Error: {synthResponse.StatusCode}]", Color.Red);
                    Task.Run(() => TTSMessageQueue.PlayNextInQueue());
                    return;
                }

                var audioBytes = await synthResponse.Content.ReadAsByteArrayAsync(ct);
                var memoryStream = new MemoryStream(audioBytes);
                AudioDevices.PlayAudioStream(memoryStream, TTSMessageQueued, ct, true, AudioFormat.Wav);
                memoryStream.Dispose();
            }
            catch (Exception ex)
            {
                OutputText.outputLog($"[VOICEVOX TTS Error: {ex.Message}]", Color.Red);
                Task.Run(() => TTSMessageQueue.PlayNextInQueue());
            }
        }

        public static async Task LoadSpeakers()
        {
            try
            {
                var response = await client.GetAsync($"{BaseUrl}/speakers");
                if (!response.IsSuccessStatusCode) return;

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                Speakers.Clear();
                foreach (var speaker in doc.RootElement.EnumerateArray())
                {
                    var name = speaker.GetProperty("name").GetString() ?? "";
                    foreach (var style in speaker.GetProperty("styles").EnumerateArray())
                    {
                        var styleName = style.GetProperty("name").GetString() ?? "";
                        var id = style.GetProperty("id").GetInt32();
                        Speakers.Add((id, $"{id}|{name}({styleName})"));
                    }
                }
            }
            catch { }
        }

        public static void SetVoices(ComboBox voices, ComboBox styles, ComboBox accents)
        {
            accents.Items.Clear();
            accents.Items.Add("default");
            accents.SelectedIndex = 0;

            voices.Items.Clear();
            if (Speakers.Count == 0)
            {
                Task.Run(async () =>
                {
                    await LoadSpeakers();
                    voices.Invoke((MethodInvoker)delegate
                    {
                        foreach (var s in Speakers) voices.Items.Add(s.name);
                        if (voices.Items.Count > 0) voices.SelectedIndex = 0;
                    });
                });
                voices.Items.Add("1|ずんだもん(ノーマル)");
            }
            else
            {
                foreach (var s in Speakers) voices.Items.Add(s.name);
            }
            if (voices.Items.Count > 0) voices.SelectedIndex = 0;

            styles.Items.Clear();
            styles.Items.Add("default");
            styles.SelectedIndex = 0;
            styles.Enabled = false;
            voices.Enabled = true;
        }
    }
}
