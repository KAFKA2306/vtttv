using System.Text.Json;
using OSCVRCWiz.Resources.Audio;
using OSCVRCWiz.Services.Text;

namespace OSCVRCWiz.Services.Speech.TextToSpeech.TTSEngines

{
    public class GoogleVoice
    {
        public string Name { get; set; }
        public string SsmlGender { get; set; }
        public string[] LanguageCodes { get; set; }

    }
    public class GoogleTTS
    {
        public static Dictionary<string, string[]> GoogleRememberLanguageVoices = new Dictionary<string, string[]>();
        public static bool GooglefirstVoiceLoad = true;

        public static async Task SynthesisGetAvailableVoicesAsync(string fromLanguageFullname)
        {

            VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Clear();

            if (!GoogleRememberLanguageVoices.ContainsKey(fromLanguageFullname))
            {

                List<string> localList = new List<string>();
                switch (fromLanguageFullname)
                {
                    case "Arabic [ar]":
                        localList.Add("ar-XA");

                        break;

                    case "Chinese [zh]":
                        localList.Add("cmn-CN");
                        localList.Add("cmn-TW");

                        break;
                    case "Czech [cs]": localList.Add("cs-CZ"); break;
                    case "Danish [da]": localList.Add("da-DK"); break;
                    case "Dutch [nl]":
                        localList.Add("nl-BE");
                        localList.Add("nl-NL"); break;
                    case "English [en]":
                        localList.Add("en-US");
                        localList.Add("en-GB");
                        localList.Add("en-AU");
                        localList.Add("en-CA");

                        localList.Add("en-PH");

                        break;

                    case "Filipino [fil]": localList.Add("fil-PH"); break;
                    case "Finnish [fi]": localList.Add("fi-FI"); break;
                    case "French [fr]":
                        localList.Add("fr-FR");
                        localList.Add("fr-BE");
                        localList.Add("fr-CA");
                        localList.Add("fr-CH");

                        break;
                    case "German [de]":
                        localList.Add("de-AT");
                        localList.Add("de-CH");
                        localList.Add("de-DE");
                        break;

                    case "Hindi [hi]": localList.Add("hi-IN"); break;
                    case "Hungarian [hu]": localList.Add("hu-HU"); break;
                    case "Indonesian [id]": localList.Add("id-ID"); break;

                    case "Italian [it]": localList.Add("it-IT"); break;

                    case "Japanese [ja]": localList.Add("ja-JP"); break;
                    case "Korean [ko]": localList.Add("ko-KR"); break;
                    case "Norwegian [nb]": localList.Add("nb-NO"); break;
                    case "Polish [pl]": localList.Add("pl-PL"); break;
                    case "Portuguese [pt]":
                        localList.Add("pt-BR");
                        localList.Add("pt-PT"); break;
                    case "Russian [ru]": localList.Add("ru-RU"); break;
                    case "Spanish [es]":
                        localList.Add("es-MX");
                        localList.Add("es-ES");
                        localList.Add("es-US");

                        break;
                    case "Swedish [sv]": localList.Add("sv-SE"); break;
                    case "Thai [th]": localList.Add("th-TH"); break;
                    case "Ukrainian [uk]": localList.Add("uk-UA"); break;
                    case "Vietnamese [vi]": localList.Add("vi-VN"); break;

                    default: localList.Add("en-US"); break;
                }
                List<string> voiceList = new List<string>();

                foreach (var locale in localList)
                {
                    string basePath = AppDomain.CurrentDomain.BaseDirectory;

                    string relativePath = "Assets/voices/googleVoices.json";

                    string fullPath = Path.Combine(basePath, relativePath);

                    string jsonFilePath = fullPath;

                    string jsonData = "";
                    try
                    {
                        jsonData = File.ReadAllText(jsonFilePath);
                    }
                    catch (Exception ex)
                    {
                        OutputText.outputLog("[Could not find directory, try running TTSVoiceWizard as admin or moving the entire folder to a new location. (if it's on the desktop move it to documents or where your games are stored for example)]", Color.Red);
                    }

                    GoogleVoice[] voices = JsonSerializer.Deserialize<GoogleVoice[]>(jsonData);

                    foreach (var voice in voices)
                    {
                        if (voice.LanguageCodes[0] == locale)
                        {

                            VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add(voice.Name + " | " + voice.SsmlGender);
                            voiceList.Add(voice.Name + " | " + voice.SsmlGender);
                        }

                    }

                }
                GoogleRememberLanguageVoices.Add(fromLanguageFullname, voiceList.ToArray());

            }
            else
            {

                foreach (string voice in GoogleRememberLanguageVoices[fromLanguageFullname])
                {
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add(voice);
                }
            }

            VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.SelectedIndex = 0;

        }
        public static async void GooglePlayAudio(string audioString, TTSMessageQueue.TTSMessage TTSMessageQueued, CancellationToken ct)
        {
            var audiobytes = Convert.FromBase64String(audioString);
            MemoryStream memoryStream = new MemoryStream(audiobytes);

            AudioDevices.PlayAudioStream(memoryStream, TTSMessageQueued, ct, true, AudioFormat.Mp3);
            memoryStream.Dispose();

        }

        public static void SetVoices(ComboBox voices, ComboBox styles, ComboBox accents)
        {
            accents.Items.Clear();

            var voiceAccentsGoogle = new List<string>()
                    {
                        "Arabic [ar]",
                        "Chinese [zh]",
                        "Czech [cs]",
                        "Danish [da]",
                        "Dutch [nl]",
                        "English [en]",

                        "Filipino [fil]",
                        "Finnish [fi]",
                        "French [fr]",
                        "German [de]",
                        "Hindi [hi]",
                        "Hungarian [hu]",
                        "Indonesian [id]",

                        "Italian [it]",
                        "Japanese [ja]",
                        "Korean [ko]",
                        "Norwegian [nb]",
                        "Polish [pl]",
                        "Portuguese [pt]",
                        "Russian [ru]",
                        "Spanish [es]",
                        "Swedish [sv]",
                        "Thai [th]",
                        "Ukrainian [uk]",
                        "Vietnamese [vi]"
                    };
            foreach (var accent in voiceAccentsGoogle)
            {
                accents.Items.Add(accent);
            }
            accents.SelectedIndex = 5;

            GoogleTTS.SynthesisGetAvailableVoicesAsync(accents.Text.ToString());

            styles.SelectedIndex = 0;
            styles.Enabled = false;
            voices.Enabled = true;
        }

        }
}