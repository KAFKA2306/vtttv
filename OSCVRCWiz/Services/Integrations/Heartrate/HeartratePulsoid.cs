using OSCVRCWiz.Resources.StartUp.StartUp;
using OSCVRCWiz.Services.Text;
using System.Net.WebSockets;
using System.Text;

namespace OSCVRCWiz.Services.Integrations.Heartrate
{
    public class HeartratePulsoid
    {
        public static System.Threading.Timer heartRateTimer;
        public static string heartrateIntervalPulsoid = "1500";
        static int HRPrevious = 0;
        static int currentHR = 0;
        public static bool pulsoidEnabled = false;

        static CancellationTokenSource PulsoidCt = new();

        public static void OnStartUp()
        {

            if (VoiceWizardWindow.MainFormGlobal.rjToggleActivatePulsoidStart.Checked == true)
            {
                if (!HeartratePulsoid.pulsoidEnabled)
                {
                    ConnectToPulsoid(VoiceWizardWindow.MainFormGlobal.pulsoidAuthToken.Text.ToString());
                }

            }
        }

        public static void PulsoidStop()
        {
            try
            {
                if (pulsoidEnabled == true)
                {
                    PulsoidCt.Cancel();
                    pulsoidEnabled = false;
                    StopHeartTimer();
                    var message1 = new CoreOSC.OscMessage("/avatar/parameters/isHRConnected", false);
                    OSC.OSCSender.Send(message1);
                    VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                    {
                        VoiceWizardWindow.MainFormGlobal.buttonPulsoidConnect.ForeColor = Color.Red;
                    });
                    OutputText.outputLog($"[Pulsoid WebSocket Disabled]");
                }
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[Error Stopping Pulsoid: " + ex.Message + "]", Color.Red);
            }

        }

        public static async Task ConnectToPulsoid(string accessToken)
        {

            PulsoidCt = new();

            Uri uri = new Uri($"wss://dev.pulsoid.net/api/v1/data/real_time?access_token={accessToken}");

            using (ClientWebSocket clientWebSocket = new ClientWebSocket())
            {
                try
                {
                    await clientWebSocket.ConnectAsync(uri, PulsoidCt.Token);
                    await HeartrateConnected(clientWebSocket);
                }
                catch (WebSocketException ex) when (ex.Message.Contains("403"))
                {

                    OutputText.outputLog($"Pulsoid WebSocketException: {ex.Message}", Color.Red);
                    OutputText.outputLog($"Your authorization token may be invalid, try re-copying it.", Color.Orange);

                    pulsoidEnabled = false;
                    VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                    {
                        VoiceWizardWindow.MainFormGlobal.buttonPulsoidConnect.ForeColor = Color.Red;
                    });
                }
                catch (WebSocketException ex)
                {

                    OutputText.outputLog($"Pulsoid WebSocketException: {ex.Message}", Color.Red);
                    OutputText.outputLog($"Attempting to reconnect...", Color.Orange);
                    pulsoidEnabled = true;
                    VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                    {
                        VoiceWizardWindow.MainFormGlobal.buttonPulsoidConnect.ForeColor = Color.Orange;
                    });
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    if (PulsoidCt.Token.IsCancellationRequested)
                    {
                        return;
                    }
                    ConnectToPulsoid(accessToken);

                }

                catch (Exception ex)
                {

                    OutputText.outputLog($"Heartrate Error: {ex.Message}", Color.Red);
                    pulsoidEnabled = false;
                    VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                    {
                        VoiceWizardWindow.MainFormGlobal.buttonPulsoidConnect.ForeColor = Color.Red;
                    });
                }
            }

        }

        private static async Task HeartrateConnected(ClientWebSocket clientWebSocket)
        {

            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
            {
                VoiceWizardWindow.MainFormGlobal.buttonPulsoidConnect.ForeColor = Color.Green;
                if (VoiceWizardWindow.MainFormGlobal.groupBoxHeartrate.ForeColor != Color.Green)
                {
                    VoiceWizardWindow.MainFormGlobal.groupBoxHeartrate.ForeColor = Color.Green;
                    VoiceWizardWindow.MainFormGlobal.HeartrateLabel.ForeColor = Color.Green;
                }
            });

            var message1 = new CoreOSC.OscMessage("/avatar/parameters/isHRConnected", true);
            OSC.OSCSender.Send(message1);

            OutputText.outputLog("[Pulsoid WebSocket Connected]", Color.Green);
            StartHeartTimer();
            pulsoidEnabled = true;

            while (clientWebSocket.State == WebSocketState.Open)
            {
                if (PulsoidCt.Token.IsCancellationRequested)
                {

                    await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Cancellation requested", CancellationToken.None);
                    break;
                }

                await ReceiveData(clientWebSocket);
            }
        }

        static async Task ReceiveData(ClientWebSocket clientWebSocket)
        {
            var buffer = new byte[1024];
            var result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                var data = Encoding.UTF8.GetString(buffer, 0, result.Count);

                string jsonMessage = data.ToString();

                HeartRateResponse heartRateResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<HeartRateResponse>(jsonMessage);

                currentHR = heartRateResponse.data.heart_rate;
                OSCListener.globalBPM = currentHR.ToString();
            }

        }

        public static void StartHeartTimer()
        {
            heartRateTimer = new System.Threading.Timer(heartratetimertick);
            heartRateTimer.Change(Int32.Parse(heartrateIntervalPulsoid), 0);

        }
        public static void StopHeartTimer()
        {
            if (heartRateTimer != null)
            {
                heartRateTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        public static void heartratetimertick(object sender)
        {
            Thread t = new Thread(doHeartrateTimerTick);
            t.Start();
        }

        private static async void doHeartrateTimerTick()
        {

            var message0 = new CoreOSC.OscMessage("/avatar/parameters/HR", currentHR);
            OSC.OSCSender.Send(message0);

            int hundreds = currentHR / 100;
            int tens = (currentHR / 10) % 10;
            int ones = currentHR % 10;

            var message1 = new CoreOSC.OscMessage("/avatar/parameters/onesHR", ones);
            OSC.OSCSender.Send(message1);
            var message2 = new CoreOSC.OscMessage("/avatar/parameters/tensHR", tens);
            OSC.OSCSender.Send(message2);
            var message3 = new CoreOSC.OscMessage("/avatar/parameters/hundredsHR", hundreds);
            OSC.OSCSender.Send(message3);

            float HRPercent = (float)currentHR / 255;
            var message4 = new CoreOSC.OscMessage("/avatar/parameters/HRPercent", (float)HRPercent);
            OSC.OSCSender.Send(message4);

            var labelBattery = $"❤️ {OSCListener.globalBPM}";

            if (currentHR > HRPrevious)
            {
                OSCListener.HREleveated = "🔺";
                labelBattery += " " + OSCListener.HREleveated;
            }
            else if (currentHR < HRPrevious)
            {
                OSCListener.HREleveated = "🔻";
                labelBattery += " " + OSCListener.HREleveated;

            }
            else if (currentHR == HRPrevious)
            {
                OSCListener.HREleveated = "";
                labelBattery += " " + OSCListener.HREleveated;

            }

            HRPrevious = currentHR;

            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
            {
                VoiceWizardWindow.MainFormGlobal.HeartrateLabel.Text = labelBattery;
            });

            if (VoiceWizardWindow.MainFormGlobal.rjToggleOutputHeartrateDirect.Checked)
            {

                if ((Int32.Parse(heartrateIntervalPulsoid)) < 1500)
                {
                    VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                    {
                        VoiceWizardWindow.MainFormGlobal.labelHeartIntervalTooFast.Visible = true;
                    });

                }
                else
                {
                    VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                    {
                        VoiceWizardWindow.MainFormGlobal.labelHeartIntervalTooFast.Visible = false;
                    });

                    if (OSCListener.stopBPM == false && OSCListener.pauseBPM == false)
                    {

                        if (VoiceWizardWindow.MainFormGlobal.rjToggleOSCListenerSpamLog.Checked)
                        {
                            OutputText.outputLog("Heartrate: " + currentHR.ToString() + " bpm");

                        }
                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true)
                        {
                            OutputText.outputVRChat("Heartrate: " + currentHR.ToString() + " bpm", OutputText.DisplayTextType.HeartRate);

                        }
                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                        {
                            Task.Run(() => OutputText.outputVRChatSpeechBubbles("💓 " + currentHR.ToString() + " bpm", OutputText.DisplayTextType.HeartRate));

                        }

                    }
                }
            }

            heartRateTimer.Change(Int32.Parse(heartrateIntervalPulsoid), 0);

        }

    }

    class HeartRateResponse
    {
        public long measured_at { get; set; }
        public HeartRateData data { get; set; }
    }

    class HeartRateData
    {
        public int heart_rate { get; set; }
    }
}
