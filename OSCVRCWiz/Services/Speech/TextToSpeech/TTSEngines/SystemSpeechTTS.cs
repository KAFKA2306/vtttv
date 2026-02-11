using OSCVRCWiz.Resources.Audio;
using OSCVRCWiz.Services.Text;

namespace OSCVRCWiz.Services.Speech.TextToSpeech.TTSEngines
{
    public class SystemSpeechTTS
    {

        public static List<string> systemSpeechVoiceList = new List<string>();

        public static void InitializeSystemSpeech()
        {
            try
            {
                getVoices();
                SystemSpeechRecognition.getInstalledRecogs();
            }
            catch (Exception ex) { MessageBox.Show("System Speech Startup Error: " + ex.Message); }
        }

        public static void getVoices()
        {
            System.Speech.Synthesis.SpeechSynthesizer synthesizerVoices = new System.Speech.Synthesis.SpeechSynthesizer();

            foreach (var voice in synthesizerVoices.GetInstalledVoices())
            {
                var info = voice.VoiceInfo;

                systemSpeechVoiceList.Add(info.Name + "|" + info.Culture);

            }

        }

        public static async void systemTTSAction(TTSMessageQueue.TTSMessage TTSMessageQueued, CancellationToken ct = default)
        {

            try
            {
                string phrase = TTSMessageQueued.Voice;
                string[] words = phrase.Split('|');
                int counter = 1;
                var voice = "none";

                foreach (var word in words)
                {
                    if (counter == 1)
                    {

                        voice = word;

                    }
                    if (counter == 2)
                    {

                    }
                    counter++;
                }

                System.Speech.Synthesis.SpeechSynthesizer synthesizerLite = new System.Speech.Synthesis.SpeechSynthesizer();
                synthesizerLite.SelectVoice(voice);

                MemoryStream memoryStream = new MemoryStream();
                synthesizerLite.SetOutputToWaveStream(memoryStream);
                synthesizerLite.Speak(TTSMessageQueued.text);

                AudioDevices.PlayAudioStream(memoryStream, TTSMessageQueued, ct, true, AudioFormat.Wav);
                memoryStream.Dispose();

                synthesizerLite.Dispose();
                synthesizerLite = null;
            }
            catch (Exception ex)
            {
                OutputText.outputLog("System Speech TTS Error: " + ex.Message + "]", Color.Red);
                Task.Run(() => TTSMessageQueue.PlayNextInQueue());

            }

        }

        public static void SetVoices(ComboBox voices, ComboBox styles, ComboBox accents)
        {
            accents.Items.Clear();
            accents.Items.Add("default");
            accents.SelectedIndex = 0;

            voices.Items.Clear();
            foreach (string voice in SystemSpeechTTS.systemSpeechVoiceList)
            {
                voices.Items.Add(voice);
            }
            voices.SelectedIndex = 0;
            styles.Items.Clear();
            styles.Items.Add("default");
            styles.SelectedIndex = 0;
            styles.Enabled = false;
            voices.Enabled = true;
        }

        }
}
