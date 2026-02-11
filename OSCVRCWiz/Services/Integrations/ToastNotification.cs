
using OSCVRCWiz.Resources.StartUp.StartUp;
using OSCVRCWiz.Services.Text;
using Windows.Foundation.Metadata;
using Windows.UI.Notifications;
using Windows.UI.Notifications.Management;

namespace OSCVRCWiz.Services.Integrations
{
    public class ToastNotification
    {
        static UserNotificationListener listener;

        public static List<uint> alreadySeen = new List<uint>();
        public static void IsSupported()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.Notifications.Management.UserNotificationListener"))
            {

                OutputText.outputLog("[Toast Listener Supported]", Color.Green);
            }

            else
            {

                OutputText.outputLog("[Toast Listener NOT Supported]", Color.Red);
            }
        }

        public async static void ToastListen()
        {

            if (VoiceWizardWindow.MainFormGlobal.rjToggleDiscordToast.Checked == true)
            {
                IsSupported();
            }

            UserNotificationListener listener = UserNotificationListener.Current;

            UserNotificationListenerAccessStatus accessStatus = await listener.RequestAccessAsync();

            switch (accessStatus)
            {

                case UserNotificationListenerAccessStatus.Allowed:

                    if (VoiceWizardWindow.MainFormGlobal.rjToggleDiscordToast.Checked == true)
                    {
                        OutputText.outputLog("[Toast Listener has Access]", Color.Green);
                    }

                    try
                    {

                        Task.Run(() => LoopListener());
                    }
                    catch (Exception ex)
                    {

                        if (VoiceWizardWindow.MainFormGlobal.rjToggleDiscordToast.Checked == true)
                        {
                            OutputText.outputLog("Toast Notification Error: " + ex.Message, Color.Red);
                        }
                    }
                    break;

                case UserNotificationListenerAccessStatus.Denied:

                    OutputText.outputLog("Toast Listener does not have access, go to windows settings to fix this!", Color.Red);
                    break;

                case UserNotificationListenerAccessStatus.Unspecified:

                    OutputText.outputLog("Toast Listener does not have access, go to windows settings to fix this!", Color.Red);
                    break;
            }

        }
        private static async void LoopListener()
        {
            try
            {
                while (true)
                {
                    if (VoiceWizardWindow.MainFormGlobal.rjToggleDiscordToast.Checked == true)
                    {

                        UserNotificationListener listener = UserNotificationListener.Current;
                        IReadOnlyList<UserNotification> notifs = await listener.GetNotificationsAsync(NotificationKinds.Toast);
                        foreach (UserNotification noti in notifs)
                        {
                            if (!alreadySeen.Contains(noti.Id))
                            {
                                NotificationBinding toastBinding = noti.Notification.Visual.GetBinding(KnownNotificationBindings.ToastGeneric);
                                if (toastBinding != null)
                                {
                                    string appName = noti.AppInfo.DisplayInfo.DisplayName;
                                    if (appName == "Discord")
                                    {
                                        IReadOnlyList<AdaptiveNotificationText> textElements = toastBinding.GetTextElements();
                                        string username = textElements.FirstOrDefault()?.Text;
                                        if (username != null && username != "")
                                        {
                                            alreadySeen.Add(noti.Id);

                                            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonLog.Checked == true)
                                            {
                                                OutputText.outputLog("Discord message recieved from " + username);

                                            }

                                            try
                                            {
                                                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                                                {
                                                    var message0 = new CoreOSC.OscMessage(VoiceWizardWindow.MainFormGlobal.textBoxDiscordPara.Text.ToString(), true);
                                                    OSC.OSCSender.Send(message0);
                                                    ToastNotification.toastTimer.Change(int.Parse(VoiceWizardWindow.MainFormGlobal.textBoxDiscTimer.Text.ToString()), 0);
                                                });

                                            }
                                            catch (Exception ex)
                                            {
                                                OutputText.outputLog("[Discord Toast Error: " + ex.Message + "]", Color.Red);

                                            }

                                        }
                                    }
                                }
                            }
                        }

                    }
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {

                var errorMsg = ex.Message + "\n" + ex.TargetSite + "\n\nStack Trace:\n" + ex.StackTrace;

                try
                {
                    errorMsg += "\n\n" + ex.InnerException.Message + "\n" + ex.InnerException.TargetSite + "\n\nStack Trace:\n" + ex.InnerException.StackTrace;

                }
                catch { }
                OutputText.outputLog("Discord Toast Error (Stopping): " + errorMsg, Color.Red);
                OutputText.outputLog("[Please note that the Discord toast feature does not work on x86 versions]", Color.Orange);

            }

        }

        public static System.Threading.Timer toastTimer;

        public static void initiateTimer()
        {
            toastTimer = new System.Threading.Timer(toasttimertick);
            toastTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public static void toasttimertick(object sender)
        {

            Thread t = new Thread(doToastTimerTick);
            t.IsBackground = true;
            t.Start();
        }

        private static void doToastTimerTick()
        {
            try
            {
                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    var message0 = new CoreOSC.OscMessage(VoiceWizardWindow.MainFormGlobal.textBoxDiscordPara.Text.ToString(), false);
                    OSC.OSCSender.Send(message0);
                });
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[Discord Toast Error: " + ex.Message + "]", Color.Red);

            }

        }

    }
}
