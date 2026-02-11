using CoreOSC;
using Json.Net;
using Newtonsoft.Json;
using OSCVRCWiz.Services.Integrations;
using OSCVRCWiz.Services.Text;
using OSCVRCWiz.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using static System.Net.Mime.MediaTypeNames;

namespace OSCVRCWiz.Resources.StartUp.StartUp
{
    public class OSC
    {
        public static UDPSender OSCSender;

        public static void InitializeOSC()
        {
            try
            {
                OSCSender = new UDPSender(Settings1.Default.OSCAddress, Convert.ToInt32(Settings1.Default.OSCPort));

                VoiceWizardWindow.MainFormGlobal.textBoxOSCAddress.Text = Settings1.Default.OSCAddress;
                VoiceWizardWindow.MainFormGlobal.textBoxOSCPort.Text = Settings1.Default.OSCPort;
            }
            catch (Exception ex) { MessageBox.Show("OSC Startup Error: " + ex.Message); }

        }
        public static void ChangeAddressAndPort(string address, string port)
        {
            try
            {
                Settings1.Default.OSCAddress = address;
                Settings1.Default.OSCPort = port;
                Settings1.Default.Save();
                OSCSender = new UDPSender(address, Convert.ToInt32(port));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }

    }
}
