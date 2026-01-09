
using Amazon.Polly;
using NAudio.Wave;
using Newtonsoft.Json.Linq;
using OSCVRCWiz.Resources.Audio;
using OSCVRCWiz.Resources.StartUp.StartUp;
using OSCVRCWiz.Services.Speech;
using OSCVRCWiz.Services.Speech.TextToSpeech;
using OSCVRCWiz.Services.Speech.TextToSpeech.TTSEngines;
using OSCVRCWiz.Services.Text;
using OSCVRCWiz.Settings;
using System.Diagnostics;
using System.Globalization;
using WebRtcVadSharp;
using static System.Net.Mime.MediaTypeNames;
using Windows.Devices.Spi;

namespace OSCVRCWiz.Speech_Recognition
{
    public class VoiceWizardProRecognition
    {

        private static bool DeepGramEnabled = false;

        private static WebRtcVad vad;
        private static FrameLength frameLength = FrameLength.Is30ms;
        private static int frameSize;
        public static CancellationTokenSource deepgramCt = new();
        private static readonly HttpClient client = new HttpClient();

        public static void deepgramStartup()
        {

            vad = new WebRtcVad()
            {
                OperatingMode = OperatingMode.HighQuality,
                FrameLength = frameLength,
                SampleRate = SampleRate.Is16kHz,
            };
            frameSize = (int)vad.SampleRate / 1000 * 2 * (int)frameLength;
        }

        public static async Task doRecognition(string apiKey,bool calibrating)
        {
            try
            {
                deepgramCt = new();
                int minDuration = 2;
                int maxDuration = 10;
                int howQuiet = 1000;
                string language = "en";
                int silenceScale = 30000;
                double minValidDuration = 0.5;
                OperatingMode VADMode = OperatingMode.HighQuality;

                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    minDuration = Int32.Parse(VoiceWizardWindow.MainFormGlobal.minimumAudio.Text);
                    maxDuration = Int32.Parse(VoiceWizardWindow.MainFormGlobal.maximumAudio.Text);
                    howQuiet = Int32.Parse(VoiceWizardWindow.MainFormGlobal.textBoxSilence.Text);
                    language = VoiceWizardWindow.MainFormGlobal.comboBoxSpokenLanguage.SelectedItem.ToString();
                    silenceScale = Int32.Parse(VoiceWizardWindow.MainFormGlobal.textBoxSilenceScale.Text);
                    minValidDuration = Convert.ToDouble(VoiceWizardWindow.MainFormGlobal.textBoxMinValidDeepgramDur.Text.ToString(), CultureInfo.InvariantCulture);
                    VADMode = (OperatingMode)VoiceWizardWindow.MainFormGlobal.comboBoxVADMode.SelectedIndex;

                });

                if (!calibrating)
                {
                    if (!VoiceWizardWindow.MainFormGlobal.rjToggleDeepGramContinuous.Checked)
                    {
                        OutputText.outputLog("[DeepGram Listening]");
                        DoSpeech.speechToTextOnSound();

                        using (MemoryStream audioStream = await RecordAudio(minDuration, maxDuration, howQuiet, silenceScale, minValidDuration, VADMode, false, deepgramCt))
                        {

                            if (audioStream != null)
                            {

                                 string transcribedText = await Task.Run(() => CallVoiceProAPIAsync(apiKey, audioStream, language, howQuiet));

                                  TTSMessageQueue.QueueMessage(transcribedText, "DeepGram (Pro Only)");

                            }
                            else
                            {
                                if (VoiceWizardWindow.MainFormGlobal.rjToggleDeepgramDebug.Checked)
                                {
                                    OutputText.outputLog("[DeepGram: No voice detected]");
                                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                                    {
                                        var typingbubble = new CoreOSC.OscMessage("/chatbox/typing", false);
                                        OSC.OSCSender.Send(typingbubble);

                                    }
                                }

                            }
                            DoSpeech.speechToTextButtonOff();
                        }
                    }
                    else
                    {
                        if (!DeepGramEnabled)
                        {
                            OutputText.outputLog("[DeepGram Listening (Continuous)]");
                            DoSpeech.speechToTextOnSound();
                            DeepGramEnabled = true;

                            while (DeepGramEnabled)
                            {
                                try
                                {
                                    VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                                    {
                                        minDuration = Int32.Parse(VoiceWizardWindow.MainFormGlobal.minimumAudio.Text);
                                        maxDuration = Int32.Parse(VoiceWizardWindow.MainFormGlobal.maximumAudio.Text);
                                        howQuiet = Int32.Parse(VoiceWizardWindow.MainFormGlobal.textBoxSilence.Text);
                                        language = VoiceWizardWindow.MainFormGlobal.comboBoxSpokenLanguage.SelectedItem.ToString();
                                        silenceScale = Int32.Parse(VoiceWizardWindow.MainFormGlobal.textBoxSilenceScale.Text);
                                        minValidDuration = Convert.ToDouble(VoiceWizardWindow.MainFormGlobal.textBoxMinValidDeepgramDur.Text.ToString(), CultureInfo.InvariantCulture);
                                        VADMode = (OperatingMode)VoiceWizardWindow.MainFormGlobal.comboBoxVADMode.SelectedIndex;

                                    });
                                }
                                catch (Exception ex)
                                {
                                    OutputText.outputLog($"[Deepgram Settings Error: {ex.Message}", Color.Red);
                                }
                                  using (MemoryStream audioStream = await RecordAudio(minDuration, maxDuration, howQuiet, silenceScale, minValidDuration, VADMode, false,deepgramCt))
                                  {

                                    if (DeepGramEnabled)
                                    {

                                    if (audioStream != null)
                                        {

                                                        string transcribedText = await Task.Run(() => CallVoiceProAPIAsync(apiKey, audioStream, language, howQuiet));

                                                        TTSMessageQueue.QueueMessage(transcribedText, "DeepGram (Pro Only)");

                                        }
                                        else
                                        {

                                        if (VoiceWizardWindow.MainFormGlobal.rjToggleDeepgramDebug.Checked)
                                            {
                                                OutputText.outputLog("[DeepGram: No voice detected]");
                                                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                                                {
                                                    var typingbubble = new CoreOSC.OscMessage("/chatbox/typing", false);
                                                    OSC.OSCSender.Send(typingbubble);

                                                }
                                            }

                                        }
                                    }

                                }

                            }
                        }
                        else
                        {

                            OutputText.outputLog("[DeepGram Stopped Listening]");
                            DoSpeech.speechToTextOffSound();
                            DeepGramEnabled = false;
                            deepgramCt.Cancel();
                        }
                    }
                }
                else
                {
                    OutputText.outputLog("[DeepGram Calibrating]");
                    OutputText.outputLog("[Deepgram is being calibrated to ignore your background noise, do not speak. Speaking will ruin the calibration]",Color.Orange);
                    DoSpeech.speechToTextOnSound();

                    using (MemoryStream audioStream = await RecordAudio(minDuration, maxDuration, howQuiet, silenceScale, minValidDuration, VADMode, true, deepgramCt))
                    {

                        OutputText.outputLog("[DeepGram Calibration Complete]");
                        OutputText.outputLog("[You may now activate Deepgram recognition]",Color.Green);
                        DoSpeech.speechToTextButtonOff();
                    }

                }
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[DeepGram Stopped Listening]");

                var errorMsg = ex.Message + "\n" + ex.TargetSite + "\n\nStack Trace:\n" + ex.StackTrace;

                try
                {
                    errorMsg += "\n\n" + ex.InnerException.Message + "\n" + ex.InnerException.TargetSite + "\n\nStack Trace:\n" + ex.InnerException.StackTrace;

                }
                catch { }
                OutputText.outputLog("[VoiceWizardPro Reognition Error: " + errorMsg + "]", Color.Red);

                DoSpeech.speechToTextButtonOff();
            }
        }

        private static async Task<string> CallVoiceProAPIAsync(string apiKey, MemoryStream memoryStream, string lang, int silenceThreshold)
        {

            var branch = "eastus";
            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
            {
                branch = VoiceWizardWindow.MainFormGlobal.comboBoxProBranch.Text.ToString();
            });

            var url = $"https://ttsvoicewizard.herokuapp.com/api/transcribe?";
            switch (branch)
            {
                case "eastus": url = $"https://ttsvoicewizard.herokuapp.com/api/transcribe?"; break;
                case "dev": url = $"https://ttsvoicewizard-playground.herokuapp.com/api/transcribe?"; break;
                case "local": url = $"http://localhost:54029/api/transcribe?"; break;
                default: break;
            }

            url +=

            $"apiKey={apiKey}" +
               $"&fromLang={lang}" +
               $"&silenceThreshold={silenceThreshold}";

            var response = await client.PostAsync(url, new StreamContent(memoryStream)).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                OutputText.outputLog("VoiceWizardPro API Error: " + response.StatusCode + ": " + errorMessage, Color.Red);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            System.Diagnostics.Debug.WriteLine("VoiceWizardPro API: " + json);

            var hoursUsed = JObject.Parse(json).SelectToken("hoursUsed").ToString();
            var hoursLimit = JObject.Parse(json).SelectToken("hoursLimit").ToString();

            _ = Task.Run(() =>
            {
                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {

                    VoiceWizardWindow.MainFormGlobal.SpeechHoursUsed.Text = $"Speech Hours Used: {Math.Round(decimal.Parse(hoursUsed), 4)}/{hoursLimit}";

                    Settings1.Default.hoursUsed = VoiceWizardWindow.MainFormGlobal.SpeechHoursUsed.Text.ToString();
                    Settings1.Default.Save();
                });
            });

            string duration = JObject.Parse(json).SelectToken("duration").ToString();

            if (VoiceWizardWindow.MainFormGlobal.rjToggleDeepgramDebug.Checked)
            {
                OutputText.outputLog($"Audio Duration: {duration}");
            }

            var roundedDuration = JObject.Parse(json).SelectToken("roundedDuration");
            if (roundedDuration != null)
            {
                if (VoiceWizardWindow.MainFormGlobal.rjToggleDeepgramDebug.Checked)
                {
                    OutputText.outputLog($"Audio Rounded Duration: {roundedDuration.ToString()}");
                }
            }

            string transcribedText = JObject.Parse(json).SelectToken("text").ToString();
            return transcribedText;

        }

        public static async Task<MemoryStream> RecordAudio(int minDuration, int maxDuration, int howQuiet, int silenceDuration, double minValidDuration, OperatingMode VADMode,bool calibration, CancellationTokenSource ct)
        {

            MemoryStream outputStream = new MemoryStream();

            WaveFormat waveFormat = new WaveFormat(16000, 16, 1);
            WaveInEvent waveSource = new WaveInEvent();
            waveSource.WaveFormat = waveFormat;
            waveSource.DeviceNumber = AudioDevices.getCurrentInputDevice();

            bool isVoiceDetected = false;
            bool validAudioClip = false;
            TimeSpan startTime = DateTime.MinValue.TimeOfDay;
            TimeSpan endTime = DateTime.MinValue.TimeOfDay;

            vad.OperatingMode = VADMode;

            int silenceThreshold = 1000;

            int recordingDuration = 25000;
            int initialDelay = 5000;

            silenceThreshold = howQuiet;
            recordingDuration = maxDuration * 1000;
            initialDelay = minDuration * 1000;

            bool isSilence = false;
            int silenceCounter = 0;
            int recordingCounter = 0;
            int soundVolume = 0;
            int calibrationMax = 0;

            waveSource.DataAvailable += (sender, e) =>
            {
                if (ct.Token.IsCancellationRequested)
                {
                    validAudioClip = false;
                    waveSource.StopRecording();
                }

                var bufferVAD = e.Buffer.Take(frameSize).ToArray();

                if (vad.HasSpeech(bufferVAD))
                {
                    if (!isVoiceDetected)
                    {

                        startTime = DateTime.Now.TimeOfDay;
                        isVoiceDetected = true;

                        VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                        {
                            VoiceWizardWindow.MainFormGlobal.labelVADIndicator.ForeColor = Color.Green;
                        });
                        if (VoiceWizardWindow.MainFormGlobal.rjToggleDeepgramDebug.Checked)
                        {
                            OutputText.outputLog("VAD Start Time: " + startTime);
                        }
                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonTypingIndicator.Checked == true)
                        {
                            var typingbubble = new CoreOSC.OscMessage("/chatbox/typing", true);
                            OSC.OSCSender.Send(typingbubble);
                        }
                    }

                    endTime = DateTime.Now.TimeOfDay;
                }
                else
                {
                    if (isVoiceDetected)
                    {

                        VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                        {
                            VoiceWizardWindow.MainFormGlobal.labelVADIndicator.ForeColor = Color.White;
                        });
                        if (VoiceWizardWindow.MainFormGlobal.rjToggleDeepgramDebug.Checked)
                        {
                            OutputText.outputLog("VAD End Time: " + endTime);
                        }

                        isVoiceDetected = false;

                        if ((endTime - startTime).TotalSeconds >= minValidDuration)
                        {
                            validAudioClip = true;
                        }

                        startTime = DateTime.MinValue.TimeOfDay;
                        endTime = DateTime.MinValue.TimeOfDay;
                    }
                }

                if (e.BytesRecorded > 0)
                {
                    byte[] buffer = e.Buffer;
                    int bytesRecorded = e.BytesRecorded;

                    for (int i = 0; i < bytesRecorded; i += 2)
                    {

                        short sample = (short)((buffer[i + 1] << 8) | buffer[i]);
                          soundVolume = Math.Abs((int)sample);

                        if (calibration && (soundVolume > calibrationMax))
                        {
                            calibrationMax = soundVolume;
                        }

                        if (soundVolume < silenceThreshold)
                        {

                            silenceCounter += waveFormat.BlockAlign;
                        }

                        else
                        {

                            silenceCounter = 0;
                        }
                    }

                    outputStream.Write(buffer, 0, bytesRecorded);

                    _ = Task.Run(() =>
                    {
                        if (!VoiceWizardWindow.MainFormGlobal.IsDisposed)
                    {
                        VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                       {

                        int checker = silenceCounter;
                        if (checker > silenceDuration)
                        {
                            checker = silenceDuration;
                        }
                        if (soundVolume > 2000) {soundVolume = 2000;}
                        if (soundVolume < 0){soundVolume = 0;}

                           if (calibration)
                         {
                               VoiceWizardWindow.MainFormGlobal.textBoxSilence.Text = (calibrationMax+100).ToString();
                          }

                        VoiceWizardWindow.MainFormGlobal.progressBar1.Value = soundVolume;
                        VoiceWizardWindow.MainFormGlobal.progressBar2.Maximum = silenceDuration;
                        int reversedValue = silenceDuration - checker;
                        VoiceWizardWindow.MainFormGlobal.progressBar2.Value = reversedValue;

                       });
                    }
                    else
                    {
                        return;
                    }
                    });

                    recordingCounter += (bytesRecorded / waveFormat.BlockAlign) * 1000 / waveFormat.SampleRate;

                    if(calibration && (recordingCounter>=3000))
                    {
                        waveSource.StopRecording();
                    }
                    if (recordingCounter >= recordingDuration)
                    {
                        Debug.WriteLine("Max Record Length");
                        waveSource.StopRecording();
                    }

                    else if (silenceCounter >= silenceDuration && recordingCounter > initialDelay)
                    {
                        Debug.WriteLine("Ended by silence");
                        waveSource.StopRecording();
                    }

                }
            };

            TaskCompletionSource<bool> recordingTaskCompletionSource = new TaskCompletionSource<bool>();

            waveSource.RecordingStopped += (sender, e) =>
            {
                waveSource.Dispose();
                outputStream.Position = 0;
                recordingTaskCompletionSource.SetResult(true);
            };

            waveSource.StartRecording();

            await recordingTaskCompletionSource.Task;

            if(calibration)
            {

                return null;
            }

            if (!isVoiceDetected && !validAudioClip)
            {

                return null;
            }

            return outputStream;
        }

    }

}
