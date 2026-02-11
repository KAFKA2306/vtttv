using OSCVRCWiz.Resources.Audio;
using OSCVRCWiz.Resources.StartUp.StartUp;
using OSCVRCWiz.Services.Integrations;
using OSCVRCWiz.Services.Integrations.Heartrate;
using OSCVRCWiz.Services.Integrations.Media;
using OSCVRCWiz.Services.Speech.Speech_Recognition;
using OSCVRCWiz.Services.Speech.TextToSpeech.TTSEngines;
using OSCVRCWiz.Services.Text;
using OSCVRCWiz.Speech_Recognition;
using System.Configuration;

namespace OSCVRCWiz.Resources.StartUp
{
    public class StartUps
    {
        public static int fontSize = 20;
        public static bool safeStart = true;

        public static void SetTextBoxFontSize()
        {
            if (fontSize >= 1)
            {
                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    VoiceWizardWindow.MainFormGlobal.richTextBox3.Font = new Font("Segoe UI", fontSize);
                });
            }
        }
        public static void OnAppStart()
        {
            AudioDevices.InitializeAudioDevices();
            SystemSpeechTTS.InitializeSystemSpeech();

        }

        public static void OnFormLoad()
        {

            OSC.InitializeOSC();

            HomeScreenBanner.initiateTimer();
            OutputText.loadTextDelays();
            OutputText.initiateTextTimers();
            VRChatListener.initiateTimer();
            SpotifyAddon.initiateTimer();
            WhisperRecognition.initiateWhisper();
            VoiceWizardProRecognition.deepgramStartup();
            ToastNotification.initiateTimer();

            Updater.getGithubInfo();
            Hotkeys.InitiateHotkeys();
            SetTextBoxFontSize();
            OSCListener.OnStartUp();
            VRChatListener.OnStartUp();
            HeartratePulsoid.OnStartUp();
            WindowsMedia.getWindowsMedia();
            VoiceCommands.voiceCommands();
            VoiceCommands.refreshCommandList();
            ToastNotification.ToastListen();
            MinimizeSystemTray.StartInSystemTray();
            OutputText.EmptyTextOutput();
            WindowsMedia.addSoundPad();
            WebSocketServer.ActivateOnStartUp();

            OutputText.outputLog("[Guides: https://ttsvoicewizard.com/docs/intro ]");

            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonUsePro.Checked == false)
            {
                OutputText.outputLog("[Consider becoming a VoiceWizardPro member for instant access to all the best voices: https://www.patreon.com/ttsvoicewizard ]", Color.DarkOrange);
            }
            else
            {

                VoiceWizardWindow.MainFormGlobal.iconButton17.ForeColor = Color.White;
                VoiceWizardWindow.MainFormGlobal.iconButton17.IconColor = Color.White;
            }

        }

        public static void saveBackupOfSettings()
        {

            string configPathBackup;
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
                configPathBackup = config.FilePath + ".bak";
                config.SaveAs(configPathBackup, ConfigurationSaveMode.Full, true);

            }
            catch (ConfigurationErrorsException ex)
            {
                safeStart = false;
                string filename = ex.Filename;
                configPathBackup = filename + ".bak";

                DateTime timestamp = DateTime.Now;
                string timestampString = timestamp.ToString("-yyyyMMdd_HHmmss");

                string configPathFOREVERBackup = filename + timestampString + ".bak";

                try
                {
                    if (File.Exists(filename) == true)
                    {

                        File.Delete(filename);

                    }
                }
                catch (System.Exception exx)
                {

                }

                try
                {

                    if (!string.IsNullOrEmpty(configPathBackup) && File.Exists(configPathBackup))
                    {
                        File.Copy(configPathBackup, configPathFOREVERBackup, true);

                        File.Copy(configPathBackup, filename, true);

                    }
                }
                catch (System.Exception exx)
                {

                }

            }

        }
        public static void BackupStatus()
        {
            if(safeStart)
            {
                OutputText.outputLog("[A backup of your settings was successfully created]", Color.Purple);
            }
            else
            {
                OutputText.outputLog("[Configuration system failed to initialize - Your settings were loaded from a backup (changes to settings from your last session may have been lost)]", Color.MediumVioletRed);
            }
        }

    }
}
