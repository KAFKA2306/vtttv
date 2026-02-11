using Vosk;
using Newtonsoft.Json.Linq;
using NAudio.Wave;
using System.Diagnostics;
using CoreOSC;
using OSCVRCWiz.Resources.Audio;
using OSCVRCWiz.Services.Speech.TextToSpeech;
using OSCVRCWiz.Services.Text;
using OSCVRCWiz.Resources.StartUp.StartUp;
using OSCVRCWiz.Services.Speech;

namespace OSCVRCWiz
{
    public class VoskRecognition
    {

        static Model model;
        static VoskRecognizer rec;
        static WaveInEvent waveIn;

        static bool voskEnabled = false;

        static bool voskPause = false;

        public static void toggleVosk()
        {
            if (VoiceWizardWindow.MainFormGlobal.modelTextBox.Text.ToString() != "no folder selected")
            {
                if (voskEnabled == false)
                {
                    DoSpeech.speechToTextOnSound();

                    if (voskPause==false)
                    {
                        Task.Run(() => VoskRecognition.doVosk());
                        voskPause= true;
                    }
                    else
                    {
                        Task.Run(() => VoskRecognition.unpauseVosk());
                    }

                        voskEnabled = true;

                }
                else
                {
                    DoSpeech.speechToTextOffSound();
                    Task.Run(() => VoskRecognition.pauseVosk());

                    voskEnabled = false;

                }
            }
            else
            {
                OutputText.outputLog("[No vosk model folder selected. When selecting you model foler make sure that the folder you select DIRECTLY contains the model files or the program will crash!]", Color.Red);
                MessageBox.Show("No vosk model folder selected. When selecting your model folder make sure that the folder you select DIRECTLY contains the model files or the program will crash!");

            }
        }
        public static void AutoStopVoskRecog()
        {
            if (voskEnabled == true || voskPause==true)
            {
                VoskRecognition.stopVosk();
                voskEnabled = false;
                voskPause= false;

            }

        }
            public static void doVosk()
        {

            var path = VoiceWizardWindow.MainFormGlobal.modelTextBox.Text.ToString();
            try
            {
                if (!Directory.Exists(path + "\\graph"))
                {

                    if (MessageBox.Show("Are you sure this is a valid vosk model? If the selected folder is not valid TTS Voice Wizard will crash. (The most common mistake is picking the outer folder when you should select the folder that contains the 'readme' and 'graph' folder etc.) ", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {

                        Task.Run(() => runVoskNow(path));
                    }
                    else
                    {

                        OutputText.outputLog("[Vosk Failed to Start]", Color.Red);
                        voskEnabled = false;
                    }
                }
                else
                {
                    Task.Run(() => runVoskNow(path));
                }

            }
            catch (Exception ex)
            {
                voskEnabled = false;
                OutputText.outputLog("[Vosk Failed to Start]", Color.Red);

                MessageBox.Show("Vosk Error: " + ex.Message);
                DoSpeech.speechToTextOffSound();

            }
        }
        private static void runVoskNow(string path)
        {
            OutputText.outputLog("[Starting Up Vosk...(don't click anything)]");
            model = new Model(path);
            rec = new VoskRecognizer(model, 48000f);

            waveIn = new WaveInEvent();
            waveIn.DeviceNumber = AudioDevices.getCurrentInputDevice();

            waveIn.WaveFormat = new WaveFormat(48000, 1);
            waveIn.DataAvailable += WaveInOnDataAvailable;
            waveIn?.StartRecording();
            OutputText.outputLog("[Vosk Listening]");

            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
            {
                var sttListening = new OscMessage("/avatar/parameters/stt_listening", true);
                OSC.OSCSender.Send(sttListening);
            }
        }
        private static async void WaveInOnDataAvailable(object? sender, WaveInEventArgs e)
        {
            try
            {
                if (rec != null)
                {
                    if (rec.AcceptWaveform(e.Buffer, e.BytesRecorded))
                    {

                        string json = rec.Result();
                        var text = JObject.Parse(json)["text"].ToString();
                        System.Diagnostics.Debug.WriteLine("Vosk: " + text);
                        if (text != "")
                        {

                            TTSMessageQueue.QueueMessage(text, "Vosk");
                        }
                    }
                    else
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void stopVosk()
        {
            try
            {
                if (voskEnabled == true)
                {
                    pauseVosk();
                    Debug.WriteLine("wavein stopped");
                    waveIn = null;
                    Debug.WriteLine("wavein nulled");
                }

                model?.Dispose();
                Debug.WriteLine("model disposed");

                rec?.Dispose();
                Debug.WriteLine("rec disposed");

                OutputText.outputLog("[Vosk Stopped Listening (resources freed)]");

                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                {
                    var sttListening = new OscMessage("/avatar/parameters/stt_listening", false);
                    OSC.OSCSender.Send(sttListening);
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }
        public static void pauseVosk()
        {
            try
            {
                waveIn?.StopRecording();

                OutputText.outputLog("[Vosk Muted, to free resources switch speech to text mode]");
                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                {
                    var sttListening = new OscMessage("/avatar/parameters/stt_listening", false);
                    OSC.OSCSender.Send(sttListening);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                {
                    var sttListening = new OscMessage("/avatar/parameters/stt_listening", false);
                    OSC.OSCSender.Send(sttListening);
                }
            }

        }
        public static void unpauseVosk()
        {
            try
            {

                waveIn?.StartRecording();

                OutputText.outputLog("[Vosk Unmuted]");
                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                {
                    var sttListening = new OscMessage("/avatar/parameters/stt_listening", true);
                    OSC.OSCSender.Send(sttListening);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
