
using OSCVRCWiz.Settings;
using System.Diagnostics;
using Settings;
using OSCVRCWiz.Speech_Recognition;
using OSCVRCWiz.Resources.Audio;
using OSCVRCWiz.Resources.Themes;
using OSCVRCWiz.Services.Speech.TextToSpeech;
using OSCVRCWiz.Services.Text;
using OSCVRCWiz.Services.Integrations.Media;
using OSCVRCWiz.Services.Integrations;
using OSCVRCWiz.Services.Speech;
using OSCVRCWiz.Services.Speech.TextToSpeech.TTSEngines;
using OSCVRCWiz.Resources.StartUp;
using OSCVRCWiz.Resources.StartUp.StartUp;
using FontAwesome.Sharp;
using OSCVRCWiz.Services.Speech.TranslationAPIs;
using OSCVRCWiz.Services.Integrations.Heartrate;
using System.Configuration;
using OSCVRCWiz.Services.Speech.Speech_Recognition;
using System.Media;
using OSCVRCWiz.RJControls;
using System.Globalization;
using AutoUpdaterDotNET;
namespace OSCVRCWiz
{
    public partial class VoiceWizardWindow : Form
    {
        public static VoiceWizardWindow MainFormGlobal;
        bool forceClose = false;
        private static bool settingsLoaded = false;
        public VoiceWizardWindow()
        {
             InitializeComponent();
            MainFormGlobal = this;
            StartUps.saveBackupOfSettings();
                mainTabControl.Appearance = TabAppearance.FlatButtons;
                mainTabControl.ItemSize = new System.Drawing.Size(0, 1);
                mainTabControl.SizeMode = TabSizeMode.Fixed;
                labelCharCount.Text = richTextBox3.Text.ToString().Length.ToString();
                navbarHome.BackColor = SelectedNavBar;
                StartUps.OnAppStart();
                LanguageSelect.loadLanguages(comboBoxSpokenLanguage, comboBoxTranslationLanguage);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
                    LoadSettings.LoadingSettings();
                    settingsLoaded = true;
                StartUps.OnFormLoad();
                StartUps.BackupStatus();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hotkeys.UnregisterHotKey(this.Handle, 0);
            Hotkeys.UnregisterHotKey(this.Handle, 1);
            Hotkeys.UnregisterHotKey(this.Handle, 2);
            if (!forceClose)
            {
                SaveSettings.SavingSettings();
            }
            MoonbaseTTS.CloseMoonbaseTerminal();
            StopAllRecogntion();
            HeartratePulsoid.PulsoidStop();
        }
        private void VoiceWizardWindow_Resize(object sender, EventArgs e)
        {
            if (rjToggleButtonSystemTray.Checked )
            {
                bool cursorNotInBar = Screen.GetWorkingArea(this).Contains(Cursor.Position);
                if (this.WindowState == FormWindowState.Minimized && cursorNotInBar)
                {
                    this.ShowInTaskbar = false;
                    notifyIcon1.Visible = true;
                    this.Hide();
                    Hotkeys.CUSTOMRegisterHotKey(0, Hotkeys.modifierKeySTTTS, Hotkeys.normalKeySTTTS);
                    Hotkeys.CUSTOMRegisterHotKey(1, Hotkeys.modifierKeyStopTTS, Hotkeys.normalKeyStopTTS);
                    Hotkeys.CUSTOMRegisterHotKey(2, Hotkeys.modifierKeyQuickType, Hotkeys.normalKeyQuickType);
                }
            }
        }
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
                notifyIcon1.Visible = false;
                this.Show();
                Hotkeys.CUSTOMRegisterHotKey(0, Hotkeys.modifierKeySTTTS, Hotkeys.normalKeySTTTS);
                Hotkeys.CUSTOMRegisterHotKey(1, Hotkeys.modifierKeyStopTTS, Hotkeys.normalKeyStopTTS);
                Hotkeys.CUSTOMRegisterHotKey(2, Hotkeys.modifierKeyQuickType, Hotkeys.normalKeyQuickType);
                this.WindowState = FormWindowState.Normal;
            }
        }
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            Hotkeys.CatchHotkey(ref m);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TTSButton.PerformClick();
                e.Handled = true;
            }
        }
        private void buttonQuickTypeEdit_Click(object sender, EventArgs e)
        {
            Hotkeys.HotkeyEdit(textBoxQuickType1, textBoxQuickType2, buttonQuickTypeEdit, buttonQuickTypeSave);
        }
        private void buttonQuickTypeSave_Click(object sender, EventArgs e)
        {
            (Hotkeys.modifierKeyQuickType, Hotkeys.normalKeyQuickType) = Hotkeys.HotkeySave(textBoxQuickType1, textBoxQuickType2, buttonQuickTypeEdit, buttonQuickTypeSave, 2);
        }
        private void button28_Click(object sender, EventArgs e)
        {
            Hotkeys.HotkeyEdit(textBox4, textBox1, button28, button27);
        }
        private void button27_Click(object sender, EventArgs e)
        {
            (Hotkeys.modifierKeySTTTS, Hotkeys.normalKeySTTTS) = Hotkeys.HotkeySave(textBox4, textBox1, button28, button27, 0);
        }
        private void button39_Click(object sender, EventArgs e)
        {
            Hotkeys.HotkeyEdit(textBoxStopTTS1, textBoxStopTTS2, button39, button40);
        }
        private void button40_Click(object sender, EventArgs e)
        {
            (Hotkeys.modifierKeyStopTTS, Hotkeys.normalKeyStopTTS) = Hotkeys.HotkeySave(textBoxStopTTS1, textBoxStopTTS2, button39, button40, 1);
        }
        private void rjToggleButtonQuickTypeEnabled_CheckedChanged(object sender, EventArgs e)
        {
            Hotkeys.HotkeyEnableChanged(rjToggleButtonQuickTypeEnabled, Hotkeys.modifierKeyQuickType, Hotkeys.normalKeyQuickType, 2);
        }
        private void rjToggleButton9_CheckedChanged(object sender, EventArgs e)
        {
            Hotkeys.HotkeyEnableChanged(rjToggleButton9, Hotkeys.modifierKeySTTTS, Hotkeys.normalKeySTTTS, 0);
        }
        private void rjToggleButton12_CheckedChanged(object sender, EventArgs e)
        {
            Hotkeys.HotkeyEnableChanged(rjToggleButton12, Hotkeys.modifierKeyStopTTS, Hotkeys.normalKeyStopTTS, 1);
        }
        private void textBoxQuickType1_KeyDown(object sender, KeyEventArgs e)
        {
            Hotkeys.HotkeyKeyDown(textBoxQuickType1, e, true);
        }
        private void textBoxQuickType2_KeyDown(object sender, KeyEventArgs e)
        {
            Hotkeys.HotkeyKeyDown(textBoxQuickType2, e, false);
        }
        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            Hotkeys.HotkeyKeyDown(textBox4, e, true);
        }
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            Hotkeys.HotkeyKeyDown(textBox1, e, false);
        }
        private void textBoxStopTTS1_KeyDown(object sender, KeyEventArgs e)
        {
            Hotkeys.HotkeyKeyDown(textBoxStopTTS1, e, true);
        }
        private void textBoxStopTTS2_KeyDown(object sender, KeyEventArgs e)
        {
            Hotkeys.HotkeyKeyDown(textBoxStopTTS2, e, false);
        }
        private void buttonUpEdit_Click(object sender, EventArgs e)
        {
            Hotkeys.HotkeyEdit(textBoxVoiceScrollUp1, textBoxVoiceScrollUp2, buttonUpEdit, buttonUpSave);
        }
        private void buttonUpSave_Click(object sender, EventArgs e)
        {
            (Hotkeys.modifierKeyScrollUp, Hotkeys.normalKeyScrollUp) = Hotkeys.HotkeySave(textBoxVoiceScrollUp1, textBoxVoiceScrollUp2, buttonUpEdit, buttonUpSave, 3);
        }
        private void buttonDownEdit_Click(object sender, EventArgs e)
        {
            Hotkeys.HotkeyEdit(textBoxVoiceScrollDown1, textBoxVoiceScrollDown2, buttonDownEdit, buttonDownSave);
        }
        private void buttonDownSave_Click(object sender, EventArgs e)
        {
            (Hotkeys.modifierKeyScrollDown, Hotkeys.normalKeyScrollDown) = Hotkeys.HotkeySave(textBoxVoiceScrollDown1, textBoxVoiceScrollDown2, buttonDownEdit, buttonDownSave, 4);
        }
        private void rjToggleSwitchVoicePresetsBind_CheckedChanged(object sender, EventArgs e)
        {
            Hotkeys.HotkeyEnableChanged(rjToggleSwitchVoicePresetsBind, Hotkeys.modifierKeyScrollUp, Hotkeys.normalKeyScrollUp, 3);
            Hotkeys.HotkeyEnableChanged(rjToggleSwitchVoicePresetsBind, Hotkeys.modifierKeyScrollDown, Hotkeys.normalKeyScrollDown, 4);
        }
        private void textBoxVoiceScrollUp1_KeyDown(object sender, KeyEventArgs e)
        {
            Hotkeys.HotkeyKeyDown(textBoxVoiceScrollUp1, e, true);
        }
        private void textBoxVoiceScrollUp2_KeyDown(object sender, KeyEventArgs e)
        {
            Hotkeys.HotkeyKeyDown(textBoxVoiceScrollUp2, e, false);
        }
        private void textBoxVoiceScrollDown1_KeyDown(object sender, KeyEventArgs e)
        {
            Hotkeys.HotkeyKeyDown(textBoxVoiceScrollDown1, e, true);
        }
        private void textBoxVoiceScrollDown2_KeyDown(object sender, KeyEventArgs e)
        {
            Hotkeys.HotkeyKeyDown(textBoxVoiceScrollDown2, e, false);
        }
        public void ShowHidePassword(TextBox password, IconButton eye)
        {
            if (eye.IconChar == FontAwesome.Sharp.IconChar.EyeSlash)
            {
                password.PasswordChar = '\0';
                eye.IconChar = FontAwesome.Sharp.IconChar.Eye;
            }
            else
            {
                password.PasswordChar = '*';
                eye.IconChar = FontAwesome.Sharp.IconChar.EyeSlash;
            }
        }
        private void richTextBox3_TextChanged(object sender, EventArgs e)
        {
            int length = richTextBox3.Text.ToString().Length;
            if (rjToggleButtonChatBox.Checked  && (richTextBox3.Text.ToString().Length > length) && VoiceWizardWindow.MainFormGlobal.rjToggleButtonTypingIndicator.Checked )
            {
                var typingbubble = new CoreOSC.OscMessage("/chatbox/typing", true);
                OSC.OSCSender.Send(typingbubble);
            }
            labelCharCount.Text = length.ToString();
            if (rjToggleButtonAutoSend.Checked)
            {
                Task.Run(() => DoSpeech.TTSButonClick());
            }
        }
        public void logLine(string line, Color? color = null)
        {
                if (VoiceWizardWindow.MainFormGlobal.IsDisposed)
                {
                    return;
                }
                if (InvokeRequired)
                {
                    this.Invoke(new Action<string, Color?>(logLine), new object[] { line, color });
                    return;
                }
                this.Invoke((MethodInvoker)delegate ()
                {
                    richTextBox1.Select(0, 0);
                    if (rjToggleDarkMode.Checked ) { richTextBox1.SelectionColor = color.GetValueOrDefault(Color.White); }
                    else { richTextBox1.SelectionColor = color.GetValueOrDefault(Color.Black); }
                    richTextBox1.SelectedText = line + "\r\n";
                });
        }
        public void ClearTextBox()
        {
                if (InvokeRequired)
                {
                    this.Invoke(new Action(ClearTextBox));
                    return;
                }
                this.Invoke((MethodInvoker)delegate ()
                {
                    richTextBox1.Text = "";
                });
        }
        public void ClearTextBoxTTS()
        {
                if (InvokeRequired)
                {
                    this.Invoke(new Action(ClearTextBoxTTS));
                    return;
                }
                this.Invoke((MethodInvoker)delegate ()
                {
                    richTextBox3.Text = "";
                });
        }
        public void ClearTypingBox()
        {
                if (InvokeRequired)
                {
                    this.Invoke(new Action(ClearTextBox));
                    return;
                }
                richTextBox9.Text = "";
                if (rjToggleButtonOSC.Checked )
                {
                    var message0 = new CoreOSC.OscMessage("/avatar/parameters/KAT_Pointer", 255);
                    OSC.OSCSender.Send(message0);
                }
        }
        public static Color SelectedNavBar = Color.FromArgb(68, 72, 111);
        Color UnSelectedNavBar = Color.FromArgb(31, 30, 68);
        private void allButtonColorReset()
        {
            navbarTextToSpeech.BackColor = UnSelectedNavBar;
            navbarSpeechProvider.BackColor = UnSelectedNavBar;
            navbarSettings.BackColor = UnSelectedNavBar;
            navbarHome.BackColor = UnSelectedNavBar;
            navbarIntegrations.BackColor = UnSelectedNavBar;
            navbarTextToText.BackColor = UnSelectedNavBar;
        }
        private void iconButton1_Click(object sender, EventArgs e)
        {
            allButtonColorReset();
            navbarHome.BackColor = SelectedNavBar;
            mainTabControl.SelectTab(tabPage4);
            pictureBox5.Show();
        }
        private void iconButton2_Click(object sender, EventArgs e)
        {
            allButtonColorReset();
            navbarTextToSpeech.BackColor = SelectedNavBar;
            mainTabControl.SelectTab(tabPage1);
            pictureBox5.Hide();
        }
        private void iconButton23_Click(object sender, EventArgs e)
        {
            allButtonColorReset();
            navbarTextToText.BackColor = SelectedNavBar;
            mainTabControl.SelectTab(tabPage3);
            pictureBox5.Hide();
        }
        private void iconButton5_Click(object sender, EventArgs e)
        {
            allButtonColorReset();
            navbarSettings.BackColor = SelectedNavBar;
            mainTabControl.SelectTab(General);
            pictureBox5.Hide();
        }
        private void iconButton3_Click(object sender, EventArgs e)
        {
            allButtonColorReset();
            navbarIntegrations.BackColor = SelectedNavBar;
            mainTabControl.SelectTab(tabAddons);
            pictureBox5.Hide();
        }
        private void iconButton4_Click(object sender, EventArgs e)
        {
            allButtonColorReset();
            navbarSpeechProvider.BackColor = SelectedNavBar;
            mainTabControl.SelectTab(APIs);
            pictureBox5.Hide();
        }
        private void iconButton7_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void iconButton6_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void iconButton12_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void iconButton8_Click(object sender, EventArgs e)
        {
            Updater.UpdateButtonClicked();
        }
        private void versionLabel_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        public bool logPanelExtended = true;
        public bool logPanelExtended2 = true;
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (richTextBox1.Lines.Count() >= 2000)
            {
                ClearTextBox();
                OutputText.outputLog("Log exceeded limit and was automatically cleared");
            }
        }
        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", e.LinkText);
        }
        private void button45_Click(object sender, EventArgs e)
        {
            if (logPanelExtended )
            {
                logPanel.Size = new System.Drawing.Size(20, logPanel.Height);
                button45.Text = "<<<";
                logPanelExtended = false;
            }
            else
            {
                logPanel.Size = new System.Drawing.Size(300, logPanel.Height);
                button45.Text = ">>>";
                logPanelExtended = true;
            }
        }
        private void logTrash_Click(object sender, EventArgs e)
        {
            ClearTextBox();
        }
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void iconButton13_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void iconButton14_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void iconButton26_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void pictureBox5_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", HomeScreenBanner.websiteLink);
        }
        private void button10_Click(object sender, EventArgs e)
        {
            HomeScreenBanner.stopTimer();
            pictureBox5.Dispose();
            buttonPreviousBanner.Dispose();
            buttonNextBanner.Dispose();
            button10.Dispose();
        }
        private void buttonPreviousBanner_Click(object sender, EventArgs e)
        {
            HomeScreenBanner.previousBanner();
        }
        private void buttonNextBanner_Click(object sender, EventArgs e)
        {
            HomeScreenBanner.nextBanner();
        }
        private void iconButton33_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void iconButton34_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void iconButton17_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private async void TTSButton_Click(object sender, EventArgs e)
        {
            Task.Run(() => DoSpeech.TTSButonClick());
        }
        private void button38_Click(object sender, EventArgs e)
        {
            DoSpeech.MainDoStopTTS();
        }
        private void buttonQueueClear_Click(object sender, EventArgs e)
        {
            TTSMessageQueue.queueTTS.Clear();
            labelQueueSize.Text = TTSMessageQueue.queueTTS.Count.ToString();
        }
        private void speechTTSButton_Click(object sender, EventArgs e)
        {
            Task.Run(() => DoSpeech.MainDoSpeechTTS());
        }
        private void iconButton36_Click(object sender, EventArgs e)
        {
            StartUps.fontSize = Int32.Parse(richTextBox3.Font.Size.ToString()) + 1;
            richTextBox3.Font = new Font("Segoe UI", StartUps.fontSize);
        }
        private void iconButton37_Click(object sender, EventArgs e)
        {
            if (Int32.Parse(richTextBox3.Font.Size.ToString()) >= 1)
            {
                StartUps.fontSize = Int32.Parse(richTextBox3.Font.Size.ToString()) - 1;
                richTextBox3.Font = new Font("Segoe UI", StartUps.fontSize);
            }
        }
        private void ttsTrash_Click(object sender, EventArgs e)
        {
            ClearTextBoxTTS();
        }
        private void buttonImportVoicePresets_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                comboBoxPreset.Items.Clear();
                comboBoxPreset.Items.Add("- None Selected -");
                VoicePresets.presetsStored = Import_Export.importFile(openFileDialog1.FileName);
                VoicePresets.presetsLoad();
                comboBoxPreset.SelectedIndex = 0;
            }
        }
        private void buttonExportVoicePresets_Click(object sender, EventArgs e)
        {
            Import_Export.ExportList("Output\\Exports", "VoicePreset", VoicePresets.presetsStored);
        }
        private void comboBoxTTSMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxTTSMode.Text.ToString())
            {
                case "Moonbase":
                    DoSpeech.TTSModeSaved = "Moonbase";
                    MoonbaseTTS.SetVoices(comboBoxVoiceSelect, comboBoxStyleSelect, comboBoxAccentSelect);
                    comboBoxTranslationLanguage.Enabled = true;
                    comboBoxAccentSelect.Enabled = false;
                    trackBarPitch.Enabled = true;
                    trackBarVolume.Enabled = true;
                    trackBarSpeed.Enabled = true;
                    OutputText.outputLog("[Make sure you have downloaded the Moonbase Voice dependencies: https:
                    break;
                case "TikTok":
                    DoSpeech.TTSModeSaved = "TikTok";
                    TikTokTTS.SetVoices(comboBoxVoiceSelect, comboBoxStyleSelect, comboBoxAccentSelect);
                    comboBoxTranslationLanguage.Enabled = true;
                    comboBoxAccentSelect.Enabled = false;
                    trackBarPitch.Enabled = true;
                    trackBarVolume.Enabled = true;
                    trackBarSpeed.Enabled = true;
                    break;
                case "TTSMonster":
                    DoSpeech.TTSModeSaved = "TTSMonster";
                    TTSMonsterTTS.SetVoices(comboBoxVoiceSelect, comboBoxStyleSelect, comboBoxAccentSelect);
                    comboBoxTranslationLanguage.Enabled = true;
                    comboBoxAccentSelect.Enabled = false;
                    trackBarPitch.Enabled = true;
                    trackBarVolume.Enabled = true;
                    trackBarSpeed.Enabled = true;
                    break;
                case "System Speech":
                    DoSpeech.TTSModeSaved = "System Speech";
                    SystemSpeechTTS.SetVoices(comboBoxVoiceSelect, comboBoxStyleSelect, comboBoxAccentSelect);
                    comboBoxTranslationLanguage.Enabled = true;
                    comboBoxAccentSelect.Enabled = false;
                    trackBarPitch.Enabled = true;
                    trackBarVolume.Enabled = true;
                    trackBarSpeed.Enabled = true;
                    break;
                case "Azure":
                    DoSpeech.TTSModeSaved = "Azure";
                    AzureTTS.SetVoices(comboBoxVoiceSelect, comboBoxStyleSelect, comboBoxAccentSelect);
                    comboBoxTranslationLanguage.Enabled = true;
                    comboBoxAccentSelect.Enabled = true;
                    trackBarPitch.Enabled = true;
                    trackBarVolume.Enabled = true;
                    trackBarSpeed.Enabled = true;
                    if (textBoxAzureKey.Text.ToString() == "" && rjToggleButtonUsePro.Checked == false)
                    {
                        OutputText.outputLog("[You appear to be missing an Azure Key, follow the steps here to get an Azure key or become a member of VoiceWizardPro: https:
                        OutputText.outputLog("[You appear to be missing a VoiceWizardPro Key, consider becoming a member: https:
                    }
                    break;
                case "Google (Pro Only)":
                    DoSpeech.TTSModeSaved = "Google (Pro Only)";
                    GoogleTTS.SetVoices(comboBoxVoiceSelect, comboBoxStyleSelect, comboBoxAccentSelect);
                    comboBoxTranslationLanguage.Enabled = true;
                    comboBoxAccentSelect.Enabled = true;
                    trackBarPitch.Enabled = true;
                    trackBarVolume.Enabled = true;
                    trackBarSpeed.Enabled = true;
                    if (textBoxWizardProKey.Text.ToString() == "")
                    {
                        OutputText.outputLog("[You appear to be missing an VoiceWizardPro Key, consider becoming a member: https:
                    }
                    break;
                case "IBM Watson (Pro Only)":
                    DoSpeech.TTSModeSaved = "IBM Watson (Pro Only)";
                    IBMWatsonTTS.SetVoices(comboBoxVoiceSelect, comboBoxStyleSelect, comboBoxAccentSelect);
                    comboBoxTranslationLanguage.Enabled = true;
                    comboBoxAccentSelect.Enabled = true;
                    trackBarPitch.Enabled = true;
                    trackBarVolume.Enabled = true;
                    trackBarSpeed.Enabled = true;
                    if (textBoxWizardProKey.Text.ToString() == "")
                    {
                        OutputText.outputLog("[You appear to be missing an VoiceWizardPro Key, consider becoming a member: https:
                    }
                    break;
                case "Deepgram Aura (Pro Only)":
                    DoSpeech.TTSModeSaved = "Deepgram Aura (Pro Only)";
                    DeepgramAuraTTS.SetVoices(comboBoxVoiceSelect, comboBoxStyleSelect, comboBoxAccentSelect);
                    comboBoxTranslationLanguage.Enabled = true;
                    comboBoxAccentSelect.Enabled = true;
                    trackBarPitch.Enabled = true;
                    trackBarVolume.Enabled = true;
                    trackBarSpeed.Enabled = true;
                    if (textBoxWizardProKey.Text.ToString() == "")
                    {
                        OutputText.outputLog("[You appear to be missing an VoiceWizardPro Key, consider becoming a member: https:
                    }
                    break;
                case "OpenAI":
                    DoSpeech.TTSModeSaved = "OpenAI";
                    OpenAITTS.SetVoices(comboBoxVoiceSelect, comboBoxStyleSelect, comboBoxAccentSelect);
                    comboBoxTranslationLanguage.Enabled = true;
                    comboBoxAccentSelect.Enabled = false;
                    trackBarPitch.Enabled = true;
                    trackBarVolume.Enabled = true;
                    trackBarSpeed.Enabled = true;
                    if (textBoxWizardProKey.Text.ToString() == "" && textBoxChatGPT.Text.ToString() == "")
                    {
                        OutputText.outputLog("[You appear to be missing an OpenAI API key, get an OpenAI API key or become a member of member of VoiceWizardPro]", Color.DarkOrange);
                        OutputText.outputLog("[You appear to be missing an VoiceWizardPro Key, consider becoming a member: https:
                    }
                    break;
                case "Uberduck":
                    DoSpeech.TTSModeSaved = "Uberduck";
                    UberDuckTTS.SetVoices(comboBoxVoiceSelect, comboBoxStyleSelect, comboBoxAccentSelect);
                    comboBoxTranslationLanguage.Enabled = true;
                    comboBoxAccentSelect.Enabled = true;
                    trackBarPitch.Enabled = true;
                    trackBarVolume.Enabled = true;
                    trackBarSpeed.Enabled = true;
                    if (textBoxUberKey.Text.ToString() == "")
                    {
                        OutputText.outputLog("[You appear to be missing an Uberduck Key: https:
                    }
                    break;
                case "VoiceForge":
                    DoSpeech.TTSModeSaved = "VoiceForge";
                    VoiceForgeTTS.SetVoices(comboBoxVoiceSelect, comboBoxStyleSelect, comboBoxAccentSelect);
                    comboBoxTranslationLanguage.Enabled = true;
                    comboBoxAccentSelect.Enabled = false;
                    trackBarPitch.Enabled = true;
                    trackBarVolume.Enabled = true;
                    trackBarSpeed.Enabled = true;
                    break;
                case "Locally Hosted":
                    DoSpeech.TTSModeSaved = "Locally Hosted";
                    GladosTTS.SetVoices(comboBoxVoiceSelect, comboBoxStyleSelect, comboBoxAccentSelect);
                    comboBoxTranslationLanguage.Enabled = true;
                    comboBoxAccentSelect.Enabled = false;
                    trackBarPitch.Enabled = true;
                    trackBarVolume.Enabled = true;
                    trackBarSpeed.Enabled = true;
                    OutputText.outputLog("[Learn more about the locally hosted option here: https:
                    break;
                case "ElevenLabs":
                    DoSpeech.TTSModeSaved = "ElevenLabs";
                    ElevenLabsTTS.SetVoices(comboBoxVoiceSelect, comboBoxStyleSelect, comboBoxAccentSelect);
                    comboBoxTranslationLanguage.Enabled = true;
                    comboBoxAccentSelect.Enabled = false;
                    trackBarPitch.Enabled = true;
                    trackBarVolume.Enabled = true;
                    trackBarSpeed.Enabled = true;
                    if (textBoxElevenLabsKey.Text.ToString() == "")
                    {
                        OutputText.outputLog("[You appear to be missing an ElevenLabs Key, make sure to follow the setup guide: https:
                    }
                    break;
                case "Amazon Polly":
                    DoSpeech.TTSModeSaved = "Amazon Polly";
                    AmazonPollyTTS.SetVoices(comboBoxVoiceSelect, comboBoxStyleSelect, comboBoxAccentSelect);
                    comboBoxTranslationLanguage.Enabled = true;
                    comboBoxAccentSelect.Enabled = true;
                    trackBarPitch.Enabled = true;
                    trackBarVolume.Enabled = true;
                    trackBarSpeed.Enabled = true;
                    if (textBoxAmazonKey.Text.ToString() == "" && rjToggleButtonUsePro.Checked == false)
                    {
                        OutputText.outputLog("[You appear to be missing an Amazon Polly Key, for the steps here to get an Amazon Polly key or become a VoiceWizardPro member: https:
                        OutputText.outputLog("[You appear to be missing a VoiceWizardPro Key, consider becoming a member: https:
                    }
                    break;
                default:
                    DoSpeech.TTSModeSaved = "No TTS";
                    comboBoxVoiceSelect.Items.Clear();
                    comboBoxVoiceSelect.Items.Add("no voice");
                    comboBoxAccentSelect.Items.Clear();
                    comboBoxAccentSelect.Items.Add("default");
                    comboBoxAccentSelect.SelectedIndex = 0;
                    comboBoxVoiceSelect.SelectedIndex = 0;
                    comboBoxStyleSelect.Items.Clear();
                    comboBoxStyleSelect.Items.Add("default");
                    comboBoxStyleSelect.SelectedIndex = 0;
                    comboBoxStyleSelect.Enabled = false;
                    comboBoxVoiceSelect.Enabled = false;
                    comboBoxTranslationLanguage.Enabled = true;
                    comboBoxAccentSelect.Enabled = false;
                    trackBarPitch.Enabled = false;
                    trackBarVolume.Enabled = false;
                    trackBarSpeed.Enabled = false;
                    break;
            }
            updateAllTrackBarLabels();
        }
        private void button50_Click(object sender, EventArgs e)
        {
            if (logPanelExtended2 )
            {
                panelCustomize.Size = new System.Drawing.Size(20, logPanel.Height);
                button50.Text = "<<<";
                logPanelExtended2 = false;
            }
            else
            {
                panelCustomize.Size = new System.Drawing.Size(315, logPanel.Height);
                button50.Text = ">>>";
                logPanelExtended2 = true;
            }
        }
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxTranslationLanguage.SelectedItem != null && comboBoxSpokenLanguage.SelectedItem != null)
            {
                string spokenLanguageCode = comboBoxSpokenLanguage.SelectedItem.ToString().Substring(0, comboBoxSpokenLanguage.SelectedItem.ToString().IndexOf(' '));
                string translationLanguageCode = comboBoxTranslationLanguage.SelectedItem.ToString().Substring(0, comboBoxTranslationLanguage.SelectedItem.ToString().IndexOf(' '));
                if (!rjToggleTranslateSameLanguage.Checked)
                {
                    if (spokenLanguageCode == translationLanguageCode)
                    {
                        comboBoxTranslationLanguage.SelectedIndex = 0;
                    }
                }
                switch (comboBoxSTT.Text.ToString())
                {
                    case "Whisper":
                        string language = "";
                        VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                        {
                            language = comboBoxSpokenLanguage.SelectedItem.ToString();
                        });
                        Task.Run(() => WhisperRecognition.setLanguage(language));
                        break;
                }
            }
        }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxTranslationLanguage.SelectedItem != null && comboBoxSpokenLanguage.SelectedItem != null)
            {
                string spokenLanguageCode = comboBoxSpokenLanguage.SelectedItem.ToString().Substring(0, comboBoxSpokenLanguage.SelectedItem.ToString().IndexOf(' '));
                string translationLanguageCode = comboBoxTranslationLanguage.SelectedItem.ToString().Substring(0, comboBoxTranslationLanguage.SelectedItem.ToString().IndexOf(' '));
                if (!rjToggleTranslateSameLanguage.Checked)
                {
                    if (spokenLanguageCode == translationLanguageCode)
                    {
                        comboBoxTranslationLanguage.SelectedIndex = 0;
                    }
                }
            }
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxTTSMode.Text.ToString())
            {
                case "Moonbase": break;
                case "ElevenLabs": break;
                case "TikTok": break;
                case "TTSMonster": break;
                case "NovelAI": break;
                case "System Speech":
                    break;
                case "Azure":
                    comboBoxStyleSelect.Items.Clear();
                    comboBoxStyleSelect.Items.Add("normal");
                    foreach (string style in AzureTTS.AllVoices4Language[comboBoxVoiceSelect.Text.ToString()])
                    {
                        comboBoxStyleSelect.Items.Add(style);
                    }
                    comboBoxStyleSelect.SelectedIndex = 0; break;
                case "Amazon Polly":
                    comboBoxStyleSelect.Items.Clear();
                    comboBoxStyleSelect.Items.Add("normal");
                    if (!VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Text.ToString().EndsWith("($Neural)"))
                    {
                        comboBoxStyleSelect.Items.Add("auto-breaths");
                        comboBoxStyleSelect.Items.Add("soft");
                        comboBoxStyleSelect.Items.Add("whispered");
                    }
                    comboBoxStyleSelect.SelectedIndex = 0;
                    break;
                case "Google (Pro Only)": break;
                case "IBM Watson (Pro Only)":
                    comboBoxStyleSelect.Items.Clear();
                    comboBoxStyleSelect.Items.Add("normal");
                    if (comboBoxVoiceSelect.SelectedItem.ToString().Contains("Expressive"))
                    {
                        comboBoxStyleSelect.Items.Add("cheerful");
                        comboBoxStyleSelect.Items.Add("empathetic");
                        comboBoxStyleSelect.Items.Add("neutral");
                        comboBoxStyleSelect.Items.Add("uncertain");
                    }
                    comboBoxStyleSelect.SelectedIndex = 0;
                    break;
                default:
                    break;
            }
        }
        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (DoSpeech.TTSModeSaved)
            {
                case "Azure": AzureTTS.SynthesisGetAvailableVoicesAsync(comboBoxAccentSelect.Text.ToString()); break;
                case "Amazon Polly": AmazonPollyTTS.SynthesisGetAvailableVoices(comboBoxAccentSelect.Text.ToString()); break;
                case "Google (Pro Only)": GoogleTTS.SynthesisGetAvailableVoicesAsync(comboBoxAccentSelect.Text.ToString()); break;
                case "Uberduck": UberDuckTTS.SynthesisGetAvailableVoicesAsync(comboBoxAccentSelect.Text.ToString(), false); break;
                case "IBM Watson (Pro Only)": IBMWatsonTTS.SynthesisGetAvailableVoicesAsync(comboBoxVoiceSelect, comboBoxAccentSelect.Text.ToString()); break;
                case "Deepgram Aura (Pro Only)": DeepgramAuraTTS.SynthesisGetAvailableVoicesAsync(comboBoxVoiceSelect, comboBoxAccentSelect.Text.ToString()); break;
            }
        }
        private void trackBarPitch_Scroll(object sender, EventArgs e)
        {
            updateAllTrackBarLabels();
        }
        private void trackBarSpeed_Scroll(object sender, EventArgs e)
        {
            updateAllTrackBarLabels();
        }
        private void trackBarVolume_Scroll(object sender, EventArgs e)
        {
            updateAllTrackBarLabels();
        }
        public void updateAllTrackBarLabels()
        {
            labelPitchNum.Text = trackBarPitch.Value + "%";
            labelSpeedNum.Text = trackBarSpeed.Value + "%";
            float value3 = trackBarVolume.Value * 0.1f;
            labelVolumeNum.Text = (Math.Round(value3, 1) * 100).ToString("0.#") + "%";
            labelStability.Text = trackBarStability.Value + "%";
            labelSimboost.Text = trackBarSimilarity.Value + "%";
            labelStyleExagg.Text = trackBarStyleExaggeration.Value + "%";
        }
        private void comboBoxPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxPreset.SelectedIndex == 0)
            {
                buttonEditPreset.Enabled = false;
                buttonDeletePreset.Enabled = false;
            }
            else
            {
                buttonEditPreset.Enabled = true;
                buttonDeletePreset.Enabled = true;
                Task.Run(() => VoicePresets.setPreset());
            }
            Settings1.Default.saveVoicePresetIndex = comboBoxPreset.SelectedIndex;
            Settings1.Default.Save();
        }
        private void button15_Click(object sender, EventArgs e)
        {
            VoicePresets.presetSaveButton();
        }
        private void button19_Click(object sender, EventArgs e)
        {
            VoicePresets.presetEditButton();
        }
        private void button25_Click_1(object sender, EventArgs e)
        {
            VoicePresets.presetDeleteButton();
        }
        private void richTextBox9_TextChanged(object sender, EventArgs e)
        {
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked  && VoiceWizardWindow.MainFormGlobal.rjToggleButtonTypingIndicator.Checked )
            {
                OutputText.typingBox = true;
                var typingbubble = new CoreOSC.OscMessage("/chatbox/typing", true);
                OSC.OSCSender.Send(typingbubble);
            }
        }
        private void iconButton22_Click(object sender, EventArgs e)
        {
            ClearTypingBox();
        }
        private void iconButton2_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void button14_Click_2(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", "Output\\Exports");
        }
        private void rjToggleButtonLog_CheckedChanged(object sender, EventArgs e)
        {
        }
        private void rjToggleButtonClear_CheckedChanged(object sender, EventArgs e)
        {
        }
        private void rjToggleButtonSystemTray_CheckedChanged(object sender, EventArgs e)
        {
        }
        private void rjToggleButtonStopCurrentTTS_CheckedChanged(object sender, EventArgs e)
        {
        }
        private void textBoxQueueDelayBeforeNext_TextChanged(object sender, EventArgs e)
        {
        }
        private void textBoxDelayAfterNoTTS_TextChanged(object sender, EventArgs e)
        {
        }
        private void rjToggleButtonQueueTypedText_CheckedChanged(object sender, EventArgs e)
        {
        }
        private void rjToggleButtonRefocus_CheckedChanged(object sender, EventArgs e)
        {
        }
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }
        private void rjToggleButtonQueueSystem_CheckedChanged(object sender, EventArgs e)
        {
            if (rjToggleButtonQueueSystem.Checked )
            {
                label81.Visible = true;
                labelQueueSize.Visible = true;
                buttonQueueClear.Visible = true;
            }
            else
            {
                label81.Visible = false;
                labelQueueSize.Visible = false;
                buttonQueueClear.Visible = false;
            }
        }
        private void rjToggleButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (rjToggleButton6.Checked )
            {
                panel1.SetBounds(0, 0, 65, 731);
                panel2Logo.SetBounds(0, 0, 220, 55);
                pictureBox1.SetBounds(0, 0, 55, 55);
                navbarHome.Text = "";
                navbarTextToSpeech.Text = "";
                navbarTextToText.Text = "";
                navbarSettings.Text = "";
                navbarIntegrations.Text = "";
                navbarDiscord.Text = "";
                navbarGithub.Text = "";
                navbarDonate.Text = "";
                navbarUpdates.Text = "";
                navbarSpeechProvider.Text = "";
            }
            if (rjToggleButton6.Checked == false)
            {
                panel1.SetBounds(0, 0, 159, 731);
                panel2Logo.SetBounds(0, 0, 159, 105);
                pictureBox1.SetBounds(12, -8, 129, 113);
                navbarHome.Text = "Home";
                navbarTextToSpeech.Text = "Text to Speech";
                navbarTextToText.Text = "Text to Text";
                navbarSpeechProvider.Text = "Speech Provider";
                navbarSettings.Text = "Settings";
                navbarIntegrations.Text = "Addon";
                navbarDiscord.Text = "Discord";
                navbarGithub.Text = "Github";
                navbarDonate.Text = "Donate";
                navbarUpdates.Text = "Update";
                navbarUpdates.Text = "Speech Provider";
            }
        }
        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            if (rjToggleButtonOnTop2.Checked )
            {
                TopMost = true;
            }
            if (rjToggleButtonOnTop2.Checked == false)
            {
                TopMost = false;
            }
        }
        private void button47_Click(object sender, EventArgs e)
        {
            var appPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TTSVoiceWizard");
            Process.Start("explorer.exe", appPath);
        }
        private void button38_Click_1(object sender, EventArgs e)
        {
            SaveSettings.SavingSettings();
        }
        private void rjToggleButtonDisableWindowsMedia_CheckedChanged(object sender, EventArgs e)
        {
            Settings1.Default.WindowsMediaDisable = rjToggleButtonDisableWindowsMedia.Checked;
            Settings1.Default.Save();
            OutputText.outputLog("Restart required for changes to take effect (Disabling Windows Media may solve 'random' crashing). If you just restarted, Windows Media mode is already disabled so disregard this message.", Color.DarkOrange);
        }
        Color DarkModeColor = Color.FromArgb(31, 30, 68);
        Color LightModeColor = Color.FromArgb(68, 72, 111);
        private void rjToggleButton7_CheckedChanged_1(object sender, EventArgs e)
        {
            if (rjToggleDarkMode.Checked )
            {
                DarkTitleBarClass.UseImmersiveDarkMode(Handle, true);
                foreach (var thisControl in GetAllChildren(this).OfType<TextBox>())
                {
                    thisControl.BackColor = DarkModeColor;
                    thisControl.ForeColor = Color.White;
                }
                foreach (var thisControl in GetAllChildren(this).OfType<RichTextBox>())
                {
                    thisControl.BackColor = DarkModeColor;
                    thisControl.ForeColor = Color.White;
                }
                foreach (var thisControl in GetAllChildren(this).OfType<ComboBox>())
                {
                    thisControl.BackColor = DarkModeColor;
                    thisControl.ForeColor = Color.White;
                }
                foreach (var thisControl in GetAllChildren(this).OfType<CheckedListBox>())
                {
                    thisControl.BackColor = DarkModeColor;
                    thisControl.ForeColor = Color.White;
                }
                labelCharCount.BackColor = DarkModeColor;
                ttsTrash.BackColor = DarkModeColor;
                logTrash.BackColor = DarkModeColor;
                iconButton22.BackColor = DarkModeColor;
                iconButton36.BackColor = DarkModeColor;
                iconButton37.BackColor = DarkModeColor;
                hidePasswordChatGPT.BackColor = DarkModeColor;
                ShowAmazonKeyPassword.BackColor = DarkModeColor;
                ShowDeepLPassword.BackColor = DarkModeColor;
                ShowAmazonSecretPassword.BackColor = DarkModeColor;
                ShowAzurePassword.BackColor = DarkModeColor;
                ShowElevenLabsPassword.BackColor = DarkModeColor;
                ShowSpotifyPassword.BackColor = DarkModeColor;
                ProShowKey.BackColor = DarkModeColor;
                UberDuckShowPassword.BackColor = DarkModeColor;
                UberDuckShowSecretPassword.BackColor = DarkModeColor;
                iconButtonPulsoidHideKey.BackColor = DarkModeColor;
                iconButton7.BackColor = DarkModeColor;
                labelCharCount.ForeColor = Color.White;
                ttsTrash.IconColor = Color.White;
                logTrash.IconColor = Color.White;
                iconButton22.IconColor = Color.White;
                iconButton36.IconColor = Color.White;
                iconButton37.IconColor = Color.White;
                iconButton36.ForeColor = Color.White;
                iconButton37.ForeColor = Color.White;
                hidePasswordChatGPT.IconColor = Color.White;
                ShowAmazonKeyPassword.IconColor = Color.White;
                ShowDeepLPassword.IconColor = Color.White;
                ShowAmazonSecretPassword.IconColor = Color.White;
                ShowAzurePassword.IconColor = Color.White;
                ShowElevenLabsPassword.IconColor = Color.White;
                ShowSpotifyPassword.IconColor = Color.White;
                ProShowKey.IconColor = Color.White;
                UberDuckShowPassword.IconColor = Color.White;
                UberDuckShowSecretPassword.IconColor = Color.White;
                iconButtonPulsoidHideKey.IconColor = Color.White;
                iconButton7.IconColor = Color.White;
            }
            if (rjToggleDarkMode.Checked == false)
            {
                DarkTitleBarClass.UseImmersiveDarkMode(Handle, false);
                foreach (var thisControl in GetAllChildren(this).OfType<TextBox>())
                {
                    thisControl.BackColor = Color.White;
                    thisControl.ForeColor = Color.Black;
                }
                foreach (var thisControl in GetAllChildren(this).OfType<RichTextBox>())
                {
                    thisControl.BackColor = Color.White;
                    thisControl.ForeColor = Color.Black;
                }
                foreach (var thisControl in GetAllChildren(this).OfType<ComboBox>())
                {
                    thisControl.BackColor = Color.White;
                    thisControl.ForeColor = Color.Black;
                }
                foreach (var thisControl in GetAllChildren(this).OfType<CheckedListBox>())
                {
                    thisControl.BackColor = Color.White;
                    thisControl.ForeColor = Color.Black;
                }
                labelCharCount.BackColor = Color.White;
                ttsTrash.BackColor = Color.White;
                logTrash.BackColor = Color.White;
                iconButton22.BackColor = Color.White;
                iconButton36.BackColor = Color.White;
                iconButton37.BackColor = Color.White;
                hidePasswordChatGPT.BackColor = Color.White;
                ShowAmazonKeyPassword.BackColor = Color.White;
                ShowDeepLPassword.BackColor = Color.White;
                ShowAmazonSecretPassword.BackColor = Color.White;
                ShowAzurePassword.BackColor = Color.White;
                ShowElevenLabsPassword.BackColor = Color.White;
                ShowSpotifyPassword.BackColor = Color.White;
                ProShowKey.BackColor = Color.White;
                UberDuckShowPassword.BackColor = Color.White;
                UberDuckShowSecretPassword.BackColor = Color.White;
                iconButtonPulsoidHideKey.BackColor = Color.White;
                iconButton7.BackColor = Color.White;
                labelCharCount.ForeColor = LightModeColor;
                ttsTrash.IconColor = LightModeColor;
                logTrash.IconColor = LightModeColor;
                iconButton22.IconColor = LightModeColor;
                iconButton36.IconColor = LightModeColor;
                iconButton37.IconColor = LightModeColor;
                iconButton36.ForeColor = LightModeColor;
                iconButton37.ForeColor = LightModeColor;
                richTextBox4.BackColor = LightModeColor;
                richTextBox4.ForeColor = Color.White;
                hidePasswordChatGPT.IconColor = LightModeColor;
                ShowAmazonKeyPassword.IconColor = LightModeColor;
                ShowDeepLPassword.IconColor = LightModeColor;
                ShowAmazonSecretPassword.IconColor = LightModeColor;
                ShowAzurePassword.IconColor = LightModeColor;
                ShowElevenLabsPassword.IconColor = LightModeColor;
                ShowSpotifyPassword.IconColor = LightModeColor;
                ProShowKey.IconColor = LightModeColor;
                UberDuckShowPassword.IconColor = LightModeColor;
                UberDuckShowSecretPassword.IconColor = LightModeColor;
                iconButtonPulsoidHideKey.IconColor = LightModeColor;
                iconButton7.IconColor = LightModeColor;
            }
        }
        public static IEnumerable<Control> GetAllChildren(Control root)
        {
            var stack = new Stack<Control>();
            stack.Push(root);
            while (stack.Any())
            {
                var next = stack.Pop();
                foreach (Control child in next.Controls)
                    stack.Push(child);
                yield return next;
            }
        }
        private void iconButton41_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void iconButton16_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void button43_Click(object sender, EventArgs e)
        {
                AudioDevices.NAudioSetupInputDevices();
                AudioDevices.NAudioSetupOutputDevices();
        }
        private void comboBoxInput_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            AudioDevices.currentInputDevice = AudioDevices.micIDs[comboBoxInput.SelectedIndex];
            AudioDevices.currentInputDeviceName = comboBoxInput.SelectedItem.ToString();
            System.Diagnostics.Debug.WriteLine("mic changed", AudioDevices.currentInputDevice);
        }
        private void comboBoxOutput_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            AudioDevices.currentOutputDevice = AudioDevices.speakerIDs[comboBoxOutput.SelectedIndex];
            AudioDevices.currentOutputDeviceName = comboBoxOutput.SelectedItem.ToString();
            System.Diagnostics.Debug.WriteLine("speaker changed");
        }
        private void comboBoxOutput2_SelectedIndexChanged(object sender, EventArgs e)
        {
            AudioDevices.currentOutputDevice2nd = AudioDevices.speakerIDs[comboBoxOutput2.SelectedIndex];
            AudioDevices.currentOutputDeviceName2nd = comboBoxOutput2.SelectedItem.ToString();
            System.Diagnostics.Debug.WriteLine("speaker changed");
        }
        public static void StopAllRecogntion()
        {
            Task.Run(() => SystemSpeechRecognition.AutoStopSystemSpeechRecog());
            Task.Run(() => WebCaptionerRecognition.autoStopWebCap());
            Task.Run(() => AzureRecognition.stopContinuousListeningNow());
            Task.Run(() => VoskRecognition.AutoStopVoskRecog());
            Task.Run(() => WhisperRecognition.autoStopWhisper());
            VoiceWizardProRecognition.deepgramCt.Cancel();
            ElevenLabsRecognition.elevenCt.Cancel();
        }
        private void comboBoxSTT_SelectedIndexChanged(object sender, EventArgs e)
        {
                StopAllRecogntion();
            if (comboBoxSTT.Text.ToString() == "Whisper" || comboBoxSTT.Text.ToString() == "Deepgram (Pro Only)")
            {
                labelVADIndicator.Visible = true;
                if (comboBoxSTT.Text.ToString() == "Whisper")
                {
                    WhisperDebugLabel.Visible = true;
                }
            }
            else
            {
                labelVADIndicator.Visible = false;
                WhisperDebugLabel.Visible = false;
            }
            comboBoxTTSMode.Enabled = true;
            switch (comboBoxSTT.Text.ToString())
            {
                case "Whisper":
                    if (whisperModelTextBox.Text.ToString() == "no model selected")
                    {
                        OutputText.outputLog("[Whisper selected for Speech to Text (Voice Recognition). SETUP GUIDE: https:
                    }
                    break;
                case "Web Captioner": OutputText.outputLog("[Web Captioner selected for Speech to Text (Voice Recognition). SETUP GUIDE: https:
                case "Vosk":
                    if (modelTextBox.Text.ToString() == "no folder selected")
                    {
                        OutputText.outputLog("[Vosk selected for Speech to Text (Voice Recognition). SETUP GUIDE: https:
                    }
                    break;
                case "Azure":
                    if (textBoxAzureKey.Text.ToString() == "")
                    {
                        OutputText.outputLog("[Azure selected for Speech to Text (Voice Recognition). SETUP GUIDE: https:
                    }
                    break;
                case "ElevenLabs STS":
                    comboBoxPreset.SelectedItem = "-None Selected-";
                    Task.Delay(1000).Wait();
                    comboBoxTTSMode.SelectedItem = "ElevenLabs";
                    comboBoxTTSMode.Enabled = false;
                    break;
                default:
                    break;
            }
        }
        private void rjToggleButtonSounds_CheckedChanged(object sender, EventArgs e)
        {
        }
        private void rjToggleButtonUse2ndOutput_CheckedChanged(object sender, EventArgs e)
        {
        }
        private void rjToggleButtonHideDelay2_CheckedChanged(object sender, EventArgs e)
        {
                if (OutputText.katRefreshTimer != null)
                {
                    if (rjToggleButtonHideDelay2.Checked == false && rjToggleButtonAutoRefreshKAT.Checked )
                    {
                        OutputText.katRefreshTimer.Change(2000, 0);
                    }
                }
        }
        private void button46_Click(object sender, EventArgs e)
        {
            var appPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).Replace("Roaming", "LocalLow") + @"\VRChat\VRChat", "OSC");
            Process.Start("explorer.exe", appPath);
            Debug.WriteLine(appPath);
        }
        private void buttonErase_Click(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                OutputText.eraseDelay = Int32.Parse(textBoxErase.Text.ToString());
                Settings1.Default.hideDelayValue = textBoxErase.Text.ToString();
                Settings1.Default.Save();
            });
        }
        private void button16_Click(object sender, EventArgs e)
        {
            OSC.ChangeAddressAndPort(textBoxOSCAddress.Text.ToString(), textBoxOSCPort.Text.ToString());
            Settings1.Default.rememberPort = textBoxOSCPort.Text.ToString();
            Settings1.Default.Save();
        }
        private void button17_Click(object sender, EventArgs e)
        {
            OSC.ChangeAddressAndPort(textBoxOSCAddress.Text.ToString(), textBoxOSCPort.Text.ToString());
            Settings1.Default.rememberAddress = textBoxOSCAddress.Text.ToString();
            Settings1.Default.Save();
        }
        private void rjToggleButtonChatBox_CheckedChanged(object sender, EventArgs e)
        {
        }
        private void rjToggleButtonShowKeyboard_CheckedChanged(object sender, EventArgs e)
        {
        }
        private void rjToggleSoundNotification_CheckedChanged(object sender, EventArgs e)
        {
        }
        private void rjToggleButtonChatBoxUseDelay_CheckedChanged(object sender, EventArgs e)
        {
        }
        private void rjToggleButtonAutoRefreshKAT_CheckedChanged(object sender, EventArgs e)
        {
                if (OutputText.katRefreshTimer != null)
                {
                    if (rjToggleButtonHideDelay2.Checked == false && rjToggleButtonAutoRefreshKAT.Checked )
                    {
                        OutputText.katRefreshTimer.Change(2000, 0);
                    }
                }
        }
        private void comboBoxPara_SelectedIndexChanged(object sender, EventArgs e)
        {
            OutputText.numKATSyncParameters = comboBoxPara.SelectedItem.ToString();
            Settings1.Default.SyncParaValue = comboBoxPara.SelectedIndex;
        }
        private void textBoxDelay_TextChanged(object sender, EventArgs e)
        {
        }
        private void rjToggleButtonOSC_CheckedChanged(object sender, EventArgs e)
        {
        }
        private void buttonDelayHere_Click(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                OutputText.debugDelayValue = Int32.Parse(textBoxDelay.Text.ToString());
                Settings1.Default.delayDebugValueSetting = textBoxDelay.Text.ToString();
                Settings1.Default.Save();
            });
        }
        private void button12_Click(object sender, EventArgs e)
        {
            var message0 = new CoreOSC.OscMessage("/avatar/parameters/KAT_Pointer", 255);
            OutputText.lastKatString = "";
            OSC.OSCSender.Send(message0);
        }
        private void hideVRCTextButton_Click(object sender, EventArgs e)
        {
            var message0 = new CoreOSC.OscMessage("/avatar/parameters/KAT_Visible", false);
            OSC.OSCSender.Send(message0);
        }
        private void button2_Click_1(object sender, EventArgs e)
        {
            OutputText.outputVRChat(OutputText.lastKatString, OutputText.DisplayTextType.UpdateText);
        }
        private void iconButtonAudioFiles_Click(object sender, EventArgs e)
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string relativePath = "Output\\AudioOutput";
            string fullPath = Path.Combine(basePath, relativePath);
            Process.Start("explorer.exe", fullPath);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string relativePath = "Output\\TextOutput";
            string fullPath = Path.Combine(basePath, relativePath);
            OutputText.outputLog(fullPath);
            Process.Start("explorer.exe", fullPath);
        }
        private void OBSLink_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void rjToggleButtonOBSText_CheckedChanged(object sender, EventArgs e)
        {
        }
        private void rjToggleButtonSaveToWav_CheckedChanged(object sender, EventArgs e)
        {
        }
        private void iconButton9_Click(object sender, EventArgs e)
        {
            mainTabControl.SelectTab(tabSpotify);
        }
        private void iconButton10_Click(object sender, EventArgs e)
        {
            mainTabControl.SelectTab(tabHeartBeat);
        }
        private void iconButton24_Click(object sender, EventArgs e)
        {
            mainTabControl.SelectTab(VRCOSC);
        }
        private void iconButton27_Click_1(object sender, EventArgs e)
        {
            mainTabControl.SelectTab(tabPage2);
        }
        private void iconButton42_Click(object sender, EventArgs e)
        {
            mainTabControl.SelectTab(Replacements);
        }
        private void iconButton25_Click(object sender, EventArgs e)
        {
            mainTabControl.SelectTab(discordTab);
        }
        private void iconButton11_Click(object sender, EventArgs e)
        {
            mainTabControl.SelectTab(tabEmoji);
        }
        private void button14_Click_1(object sender, EventArgs e)
        {
            WindowsMedia.addSoundPad();
        }
        private void iconButton1_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void ShowSpotifyPassword_Click(object sender, EventArgs e)
        {
            ShowHidePassword(textBoxSpotKey, ShowSpotifyPassword);
        }
        private void iconButton31_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void rjToggleSpotLegacy_CheckedChanged(object sender, EventArgs e)
        {
            SpotifyAddon.legacyState = rjToggleSpotLegacy.Checked;
        }
        private void textBoxSpotKey_TextChanged(object sender, EventArgs e)
        {
            Settings1.Default.SpotifyKey = textBoxSpotKey.Text.ToString();
            Settings1.Default.Save();
        }
        private void rjToggleButtonCurrentSong_CheckedChanged(object sender, EventArgs e)
        {
            if (rjToggleButtonCurrentSong.Checked )
            {
                rjToggleButtonWindowsMedia.Checked = false;
            }
            if (rjToggleButtonCurrentSong.Checked == false)
            {
                VoiceWizardWindow.MainFormGlobal.buttonSpotify.ForeColor = Color.White;
            }
        }
        private void buttonSpotify_Click(object sender, EventArgs e)
        {
            Settings1.Default.SpotifyKey = textBoxSpotKey.Text.ToString();
            Settings1.Default.Save();
            SpotifyAddon.SpotifyConnect();
            VoiceWizardWindow.MainFormGlobal.buttonSpotify.ForeColor = Color.Green;
            OutputText.outputLog("[IMPORTANT]", Color.Red);
            OutputText.outputLog("IMPORTANT: As of v1.7.5 the redirect URI for Spotify API Mode has been changed. You will need to update your app from the Spotify Dashboard. The new redirect URI is http:
            OutputText.outputLog("[IMPORTANT]", Color.Red);
        }
        private void button44_Click(object sender, EventArgs e)
        {
            var currentTime = "Current Time ⏰: " + DateTime.Now.ToString("h:mm tt");
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonSpotifySpam.Checked )
            {
                Task.Run(() => OutputText.outputLog(currentTime));
            }
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked  && VoiceWizardWindow.MainFormGlobal.rjToggleButtonSpotifyKatDisable.Checked == false)
            {
                Task.Run(() => OutputText.outputVRChat(currentTime, OutputText.DisplayTextType.Time));
            }
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked  && VoiceWizardWindow.MainFormGlobal.rjToggleButtonSpotifyChatboxDisable.Checked == false)
            {
                Task.Run(() => OutputText.outputVRChatSpeechBubbles(currentTime, OutputText.DisplayTextType.Time));
            }
        }
        private void buttonMediaPresetSaveNew_Click(object sender, EventArgs e)
        {
            MediaPresets.presetSaveButton();
        }
        private void buttonMediaPresetEditNew_Click(object sender, EventArgs e)
        {
            MediaPresets.presetEditButton();
        }
        private void buttonMediaPresetDeleteNew_Click(object sender, EventArgs e)
        {
            MediaPresets.presetDeleteButton();
        }
        private void button4_Click_1(object sender, EventArgs e)
        {
            SpotifyAddon.ChangeMediaUpdateInterval();
        }
        private void rjToggleButton10_CheckedChanged(object sender, EventArgs e)
        {
            if (rjToggleButtonWindowsMedia.Checked )
            {
                rjToggleButtonCurrentSong.Checked = false;
            }
        }
        private void button20_Click_1(object sender, EventArgs e)
        {
            string currentText = textBoxCustomSpot.Text.ToString();
            currentText = currentText + "{pause} {title} - {artist} ";
            textBoxCustomSpot.Text = currentText;
        }
        private void button21_Click_1(object sender, EventArgs e)
        {
            string currentText = textBoxCustomSpot.Text.ToString();
            currentText = currentText + "『{progressMinutes}/{durationMinutes}』 ";
            textBoxCustomSpot.Text = currentText;
        }
        private void button23_Click_1(object sender, EventArgs e)
        {
            string currentText = textBoxCustomSpot.Text.ToString();
            currentText = currentText + "『🎮{averageControllerBattery}%』『🔋{averageTrackerBattery}%{TCharge}』 ";
            textBoxCustomSpot.Text = currentText;
        }
        private void button22_Click(object sender, EventArgs e)
        {
            string currentText = textBoxCustomSpot.Text.ToString();
            currentText = currentText + "『💓{bpm}{bpmStats}』 ";
            textBoxCustomSpot.Text = currentText;
        }
        private void button14_Click(object sender, EventArgs e)
        {
            string currentText = textBoxCustomSpot.Text.ToString();
            currentText = currentText + "📢{spotifyVolume}% ";
            textBoxCustomSpot.Text = currentText;
        }
        private void button21_Click(object sender, EventArgs e)
        {
            string currentText = textBoxCustomSpot.Text.ToString();
            currentText = currentText + "{nline}";
            textBoxCustomSpot.Text = currentText;
        }
        private void button9_Click(object sender, EventArgs e)
        {
            string currentText = textBoxCustomSpot.Text.ToString();
            currentText = currentText + "{progressBar E:◯ L:13}";
            textBoxCustomSpot.Text = currentText;
        }
        private void button43_Click_2(object sender, EventArgs e)
        {
            string currentText = textBoxCustomSpot.Text.ToString();
            currentText = currentText + "{time}";
            textBoxCustomSpot.Text = currentText;
        }
        private void comboBoxMediaPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxMediaPreset.SelectedIndex == 0)
            {
                buttonMediaPresetEditNew.Enabled = false;
                buttonMediaPresetDeleteNew.Enabled = false;
            }
            else
            {
                buttonMediaPresetEditNew.Enabled = true;
                buttonMediaPresetDeleteNew.Enabled = true;
                Task.Run(() => MediaPresets.setPreset());
            }
        }
        private void buttonImportMedia_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                comboBoxMediaPreset.Items.Clear();
                comboBoxMediaPreset.Items.Add("- None Selected -");
                MediaPresets.mediaPresetsStored = Import_Export.importFile(openFileDialog1.FileName);
                MediaPresets.presetsLoad();
                comboBoxMediaPreset.SelectedIndex = 0;
            }
        }
        private void buttonExportMedia_Click(object sender, EventArgs e)
        {
            Import_Export.ExportList("Output\\Exports", "MediaPreset", MediaPresets.mediaPresetsStored);
        }
        private void button22_Click_1(object sender, EventArgs e)
        {
            SpotifyAddon.printMediaOnce();
        }
        private void button20_Click(object sender, EventArgs e)
        {
            WebSocketServer.ToggleServer();
        }
        private void label195_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void iconButton5_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void button3_Click_1(object sender, EventArgs e)
        {
            WebSocketServer.WebSocketServerPort = WebsocketServerPortText.Text.ToString();
        }
        private void buttonPulsoidConnect_Click(object sender, EventArgs e)
        {
            if (!HeartratePulsoid.pulsoidEnabled)
            {
                HeartratePulsoid.ConnectToPulsoid(VoiceWizardWindow.MainFormGlobal.pulsoidAuthToken.Text.ToString());
            }
            else
            {
                HeartratePulsoid.PulsoidStop();
            }
        }
        private void iconButtonPulsoidHideKey_Click(object sender, EventArgs e)
        {
            ShowHidePassword(pulsoidAuthToken, iconButtonPulsoidHideKey);
        }
        private void iconButton4_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void buttonPulsoidInterval_Click(object sender, EventArgs e)
        {
                HeartratePulsoid.heartrateIntervalPulsoid = textBoxPulsoidInterval.Text;
        }
        private void label192_Click(object sender, EventArgs e)
        {
            string url = "https:
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
        private void rjToggleButton8_CheckedChanged(object sender, EventArgs e)
        {
        }
        private void rjToggleButtonForwardData_CheckedChanged(object sender, EventArgs e)
        {
        }
        private void iconButton39_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void rjToggleButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (rjToggleOutputHeartrateDirect.Checked )
            {
                OSCListener.stopBPM = false;
            }
            if (rjToggleOutputHeartrateDirect.Checked == false)
            {
                OSCListener.stopBPM = true;
            }
        }
        private void button7_Click(object sender, EventArgs e)
        {
                Task.Run(() => OSCListener.OSCRecieveHeartRate());
        }
        private void rjToggleButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (rjToggleOSCListenerSpamLog.Checked )
            {
                OSCListener.OSCReceiveSpamLog = true;
            }
            if (rjToggleOSCListenerSpamLog.Checked == false)
            {
                OSCListener.OSCReceiveSpamLog = false;
            }
        }
        private void HRIntervalChange_Click(object sender, EventArgs e)
        {
            OSCListener.HRInternalValue = Int32.Parse(HRInterval.Text.ToString());
        }
        private void button8_Click(object sender, EventArgs e)
        {
            OSCListener.OSCReceiveport = Convert.ToInt32(textBoxHRPort.Text.ToString());
        }
        private void iconButton47_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void rjToggleButtonResetButtonsCounter_CheckedChanged(object sender, EventArgs e)
        {
            if (rjToggleButtonResetButtonsCounter.Checked )
            {
                buttonResetCounter1.Enabled = true;
                buttonResetCounter2.Enabled = true;
                buttonResetCounter3.Enabled = true;
                buttonResetCounter4.Enabled = true;
                buttonResetCounter5.Enabled = true;
                buttonResetCounter6.Enabled = true;
                buttonResetCounter7.Enabled = true;
                buttonResetCounter8.Enabled = true;
                buttonResetCounter9.Enabled = true;
                buttonResetCounter10.Enabled = true;
                buttonResetCounterAll.Enabled = true;
            }
            else
            {
                buttonResetCounter1.Enabled = false;
                buttonResetCounter2.Enabled = false;
                buttonResetCounter3.Enabled = false;
                buttonResetCounter4.Enabled = false;
                buttonResetCounter5.Enabled = false;
                buttonResetCounter6.Enabled = false;
                buttonResetCounter7.Enabled = false;
                buttonResetCounter8.Enabled = false;
                buttonResetCounter9.Enabled = false;
                buttonResetCounter10.Enabled = false;
                buttonResetCounterAll.Enabled = false;
            }
        }
        private void buttonResetCounter1_Click(object sender, EventArgs e)
        {
            VRChatListener.counter1 = 0;
            VRChatListener.prevCounter1 = 0;
            Settings1.Default.Counter1 = VRChatListener.counter1;
            Settings1.Default.Save();
        }
        private void buttonResetCounter2_Click(object sender, EventArgs e)
        {
            VRChatListener.counter2 = 0;
            VRChatListener.prevCounter2 = 0;
            Settings1.Default.Counter2 = VRChatListener.counter2;
            Settings1.Default.Save();
        }
        private void buttonResetCounter3_Click(object sender, EventArgs e)
        {
            VRChatListener.counter3 = 0;
            VRChatListener.prevCounter3 = 0;
            Settings1.Default.Counter3 = VRChatListener.counter3;
            Settings1.Default.Save();
        }
        private void buttonResetCounter4_Click(object sender, EventArgs e)
        {
            VRChatListener.counter4 = 0;
            VRChatListener.prevCounter4 = 0;
            Settings1.Default.Counter4 = VRChatListener.counter4;
            Settings1.Default.Save();
        }
        private void buttonResetCounter5_Click(object sender, EventArgs e)
        {
            VRChatListener.counter5 = 0;
            VRChatListener.prevCounter5 = 0;
            Settings1.Default.Counter5 = VRChatListener.counter5;
            Settings1.Default.Save();
        }
        private void buttonResetCounter6_Click(object sender, EventArgs e)
        {
            VRChatListener.counter6 = 0;
            VRChatListener.prevCounter6 = 0;
            Settings1.Default.Counter6 = VRChatListener.counter6;
            Settings1.Default.Save();
        }
        private void buttonResetCounter7_Click(object sender, EventArgs e)
        {
            VRChatListener.counter7 = 0;
            VRChatListener.prevCounter7 = 0;
            Settings1.Default.Counter7 = VRChatListener.counter7;
            Settings1.Default.Save();
        }
        private void buttonResetCounter8_Click(object sender, EventArgs e)
        {
            VRChatListener.counter8 = 0;
            VRChatListener.prevCounter8 = 0;
            Settings1.Default.Counter8 = VRChatListener.counter8;
            Settings1.Default.Save();
        }
        private void buttonResetCounter9_Click(object sender, EventArgs e)
        {
            VRChatListener.counter9 = 0;
            VRChatListener.prevCounter9 = 0;
            Settings1.Default.Counter9 = VRChatListener.counter9;
            Settings1.Default.Save();
        }
        private void buttonResetCounter10_Click(object sender, EventArgs e)
        {
            VRChatListener.counter10 = 0;
            VRChatListener.prevCounter10 = 0;
            Settings1.Default.Counter10 = VRChatListener.counter10;
            Settings1.Default.Save();
        }
        private void button32_Click(object sender, EventArgs e)
        {
            VRChatListener.FromVRChatPort = textBoxVRChatOSCPort.Text.ToString();
        }
        private void button33_Click(object sender, EventArgs e)
        {
                Task.Run(() => VRChatListener.OSCLegacyVRChatListener());
            VoiceWizardWindow.MainFormGlobal.button33.Enabled = false;
        }
        private void button36_Click(object sender, EventArgs e)
        {
            VRChatListener.counter1 = 0;
            VRChatListener.counter2 = 0;
            VRChatListener.counter3 = 0;
            VRChatListener.counter4 = 0;
            VRChatListener.counter5 = 0;
            VRChatListener.counter6 = 0;
            VRChatListener.counter7 = 0;
            VRChatListener.counter8 = 0;
            VRChatListener.counter9 = 0;
            VRChatListener.counter10 = 0;
            VRChatListener.prevCounter1 = 0;
            VRChatListener.prevCounter2 = 0;
            VRChatListener.prevCounter3 = 0;
            VRChatListener.prevCounter4 = 0;
            VRChatListener.prevCounter5 = 0;
            VRChatListener.prevCounter6 = 0;
            VRChatListener.prevCounter7 = 0;
            VRChatListener.prevCounter8 = 0;
            VRChatListener.prevCounter9 = 0;
            VRChatListener.prevCounter10 = 0;
            Settings1.Default.Counter1 = VRChatListener.counter1;
            Settings1.Default.Counter2 = VRChatListener.counter2;
            Settings1.Default.Counter3 = VRChatListener.counter3;
            Settings1.Default.Counter4 = VRChatListener.counter4;
            Settings1.Default.Counter5 = VRChatListener.counter5;
            Settings1.Default.Counter6 = VRChatListener.counter6;
            Settings1.Default.Counter7 = VRChatListener.counter7;
            Settings1.Default.Counter8 = VRChatListener.counter8;
            Settings1.Default.Counter9 = VRChatListener.counter9;
            Settings1.Default.Counter10 = VRChatListener.counter10;
            Settings1.Default.Save();
        }
        private void buttonAddVoiceCommand_Click(object sender, EventArgs e)
        {
            VoiceCommands.clearVoiceCommands();
            VoiceCommands.voiceCommandsStored += $"{textBox1Spoken.Text.ToString()}:{textBox2Address.Text.ToString()}:{comboBox3Type.SelectedItem.ToString()}:{textBox4Value.Text.ToString()};";
            VoiceCommands.voiceCommands();
            VoiceCommands.refreshCommandList();
        }
        private void button24_Click(object sender, EventArgs e)
        {
            if (deleteCommandsToggle.Checked )
            {
                VoiceCommands.clearVoiceCommands();
                VoiceCommands.refreshCommandList();
            }
        }
        private void checkedListBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (deleteCommandsToggle.Checked )
            {
                    VoiceCommands.removeVoiceCommandsAt(checkedListBox1.SelectedIndex);
                    VoiceCommands.refreshCommandList();
            }
        }
        private void iconButton40_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void buttonImportVC_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                VoiceCommands.voiceCommandsStored = Import_Export.importFile(openFileDialog1.FileName);
                VoiceCommands.voiceCommands();
                VoiceCommands.refreshCommandList();
            }
        }
        private void buttonExportVC_Click(object sender, EventArgs e)
        {
            Import_Export.ExportList("Output\\Exports", "VoiceCommand", VoiceCommands.voiceCommandsStored);
        }
        private void buttonImportWR_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                WordReplacements.wordReplacemntsStored = Import_Export.importFile(openFileDialog1.FileName);
                WordReplacements.replacementsLoad();
            }
        }
        private void buttonExportWR_Click(object sender, EventArgs e)
        {
            Import_Export.ExportList("Output\\Exports", "WordReplacement", WordReplacements.wordReplacemntsStored);
        }
        private void checkedListBoxReplacements_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (rjToggleButton7.Checked )
            {
                    WordReplacements.removeWordReplacementAt(checkedListBoxReplacements.SelectedIndex);
            }
        }
        private void buttonReplaceAdd_Click(object sender, EventArgs e)
        {
            WordReplacements.addWordReplacement(textBoxOriginalWord.Text.ToString(), textBoxReplaceWord.Text.ToString());
        }
        private void button19_Click_1(object sender, EventArgs e)
        {
            if (rjToggleButton7.Checked )
            {
                WordReplacements.clearWordReplacement();
            }
        }
        private void iconButton45_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void button15_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void iconButton44_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void buttonImportEmoji_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                checkedListBox2.Items.Clear();
                EmojiAddon.emojiReplacemntsStored = Import_Export.importFile(openFileDialog1.FileName);
                EmojiAddon.emojiReplacementsLoad();
            }
        }
        private void buttonExportEmoji_Click(object sender, EventArgs e)
        {
            Import_Export.ExportList("Output\\Exports", "Emojis", EmojiAddon.emojiReplacemntsStored);
        }
        private void iconButton32_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void button26_Click(object sender, EventArgs e)
        {
            EmojiAddon.emojiEdit(Int32.Parse(textBox7.Text.ToString()), textBox6.Text.ToString());
        }
        private void button25_Click_2(object sender, EventArgs e)
        {
            checkedListBox2.Items.Clear();
            EmojiAddon.ReplacePhraseList.Clear();
            System.Windows.Forms.MessageBox.Show("Restart App");
        }
        private void checkedListBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = checkedListBox2.SelectedIndex + 1;
            textBox7.Text = index.ToString();
        }
        private void iconButton20_Click(object sender, EventArgs e)
        {
            mainTabControl.SelectTab(AzureSet);
        }
        private void iconButton50_Click(object sender, EventArgs e)
        {
            mainTabControl.SelectTab(VoiceWizPro);
        }
        private void iconButton52_Click(object sender, EventArgs e)
        {
            mainTabControl.SelectTab(uberduck);
        }
        private void iconButton21_Click(object sender, EventArgs e)
        {
            mainTabControl.SelectTab(DeepLTab);
        }
        private void iconButton19_Click(object sender, EventArgs e)
        {
            mainTabControl.SelectTab(AmazonPolly);
        }
        private void iconButton30_Click_1(object sender, EventArgs e)
        {
            mainTabControl.SelectTab(LocalSpeech);
        }
        private void iconButton28_Click_2(object sender, EventArgs e)
        {
            mainTabControl.SelectTab(elevenLabs);
        }
        private void iconButton6_Click_1(object sender, EventArgs e)
        {
            mainTabControl.SelectTab(tabChatGPT);
        }
        private void buttonSilenceCalibrate_Click(object sender, EventArgs e)
        {
            Task.Run(async () => await VoiceWizardProRecognition.doRecognition(VoiceWizardWindow.MainFormGlobal.textBoxWizardProKey.Text.ToString(), true));
        }
        private void textBoxSilence_TextChanged_1(object sender, EventArgs e)
        {
            if (textBoxSilence.Text != trackBarSilence.Value.ToString())
            {
                int value = Int16.Parse(textBoxSilence.Text);
                if (value > 2000)
                {
                    value = 2000;
                }
                trackBarSilence.Value = value;
            }
        }
        private void ProShowKey_Click(object sender, EventArgs e)
        {
            ShowHidePassword(textBoxWizardProKey, ProShowKey);
        }
        private void trackBarSilence_Scroll(object sender, EventArgs e)
        {
            if (textBoxSilence.Text != trackBarSilence.Value.ToString())
            {
                textBoxSilence.Text = trackBarSilence.Value.ToString();
            }
        }
        private void iconButton55_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void buttonImportDict_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                richTextBoxAzureDict.Text = Import_Export.importFile(openFileDialog1.FileName);
            }
        }
        private void buttonExportDict_Click(object sender, EventArgs e)
        {
            Import_Export.ExportList("Output\\Exports", "AzureDictionary", richTextBoxAzureDict.Text);
        }
        private void ShowAzurePassword_Click(object sender, EventArgs e)
        {
            ShowHidePassword(textBoxAzureKey, ShowAzurePassword);
        }
        private void rjToggleButton4_CheckedChanged(object sender, EventArgs e)
        {
            AzureRecognition.stopContinuousListeningNow();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            string text = "";
            this.Invoke((MethodInvoker)delegate ()
            {
                text = textBoxAzureKey.Text.ToString();
                AzureRecognition.YourSubscriptionKey = text;
                if (rjToggleButtonKeyRegion2.Checked )
                {
                    Settings1.Default.yourKey = text;
                    Settings1.Default.Save();
                }
            });
        }
        private void button6_Click(object sender, EventArgs e)
        {
            string text = "";
            this.Invoke((MethodInvoker)delegate ()
            {
                text = textBox3.Text.ToString();
                AzureRecognition.YourServiceRegion = text;
                if (rjToggleButtonKeyRegion2.Checked )
                {
                    Settings1.Default.yourRegion = text;
                    Settings1.Default.Save();
                }
            });
        }
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Settings1.Default.remember = rjToggleButtonKeyRegion2.Checked;
            Settings1.Default.Save();
        }
        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (rjToggleButtonProfan.Checked )
            {
                AzureRecognition.profanityFilter = true;
            }
            if (rjToggleButtonProfan.Checked == false)
            {
                AzureRecognition.profanityFilter = false;
            }
        }
        private void button13_Click(object sender, EventArgs e)
        {
            AzureTTS.SynthesisGetAvailableVoicesAsync(comboBoxAccentSelect.Text.ToString());
        }
        private void iconButton29_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void ShowAmazonSecretPassword_Click(object sender, EventArgs e)
        {
            ShowHidePassword(textBoxAmazonSecret, ShowAmazonSecretPassword);
        }
        private void ShowAmazonKeyPassword_Click(object sender, EventArgs e)
        {
            ShowHidePassword(textBoxAmazonKey, ShowAmazonKeyPassword);
        }
        private void iconButton18_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void button31_Click(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                var text = textBoxAmazonKey.Text.ToString();
                Settings1.Default.yourAWSKey = text;
                Settings1.Default.Save();
            });
        }
        private void button29_Click(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                var text = textBoxAmazonSecret.Text.ToString();
                Settings1.Default.yourAWSSecret = text;
                Settings1.Default.Save();
            });
        }
        private void button30_Click(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                var text = textBox8.Text.ToString();
                Settings1.Default.yourAWSRegion = text;
                Settings1.Default.Save();
            });
        }
        private void ShowDeepLPassword_Click(object sender, EventArgs e)
        {
            ShowHidePassword(textBoxDeepLKey, ShowDeepLPassword);
        }
        private void iconButton43_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void button18_Click_1(object sender, EventArgs e)
        {
            Settings1.Default.deepLKeysave = textBoxDeepLKey.Text.ToString();
            Settings1.Default.Save();
        }
        private void ShowElevenLabsPassword_Click(object sender, EventArgs e)
        {
            ShowHidePassword(textBoxElevenLabsKey, ShowElevenLabsPassword);
        }
        private void iconButton54_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void trackBarStability_Scroll(object sender, EventArgs e)
        {
            labelStability.Text = trackBarStability.Value + "%";
        }
        private void trackBarSimilarity_Scroll(object sender, EventArgs e)
        {
            labelSimboost.Text = trackBarSimilarity.Value + "%";
        }
        private void trackBarStyleExaggeration_Scroll(object sender, EventArgs e)
        {
            labelStyleExagg.Text = trackBarStyleExaggeration.Value + "%";
        }
        private void button51_Click(object sender, EventArgs e)
        {
            comboBoxLabsModelID.SelectedIndex = 0;
            comboBoxLabsOptimize.SelectedIndex = 0;
            trackBarStability.Value = 50;
            trackBarSimilarity.Value = 75;
            trackBarStyleExaggeration.Value = 0;
            labelStability.Text = trackBarStability.Value + "%";
            labelSimboost.Text = trackBarSimilarity.Value + "%";
            labelStyleExagg.Text = trackBarStyleExaggeration.Value + "%";
            rjToggleSpeakerBoost.Checked = true;
        }
        private void iconButton35_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void button37_Click(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                var text = textBoxElevenLabsKey.Text.ToString();
                Settings1.Default.elevenLabsAPIKey = text;
                Settings1.Default.Save();
            });
        }
        private void button35_Click(object sender, EventArgs e)
        {
            ElevenLabsTTS.CallElevenVoices();
            if (comboBoxTTSMode.SelectedItem.ToString() == "ElevenLabs")
            {
                comboBoxVoiceSelect.Items.Clear();
                    if (ElevenLabsTTS.elevenFirstLoad )
                    {
                        ElevenLabsTTS.CallElevenVoices();
                    }
                    if (ElevenLabsTTS.voiceDict != null)
                    {
                        foreach (KeyValuePair<string, string> kvp in ElevenLabsTTS.voiceDict)
                        {
                            comboBoxVoiceSelect.Items.Add(kvp.Value);
                        }
                    }
                    else
                    {
                        comboBoxVoiceSelect.Items.Add("error");
                    }
                comboBoxVoiceSelect.SelectedIndex = 0;
                comboBoxStyleSelect.SelectedIndex = 0;
                comboBoxStyleSelect.Enabled = false;
                comboBoxVoiceSelect.Enabled = true;
                comboBoxTranslationLanguage.Enabled = true;
                comboBoxAccentSelect.Enabled = false;
                trackBarPitch.Enabled = false;
                trackBarVolume.Enabled = false;
                trackBarSpeed.Enabled = false;
                DoSpeech.TTSModeSaved = "ElevenLabs";
            }
        }
        private void UberDuckShowPassword_Click(object sender, EventArgs e)
        {
            ShowHidePassword(textBoxUberKey, UberDuckShowPassword);
        }
        private void UberDuckShowSecretPassword_Click(object sender, EventArgs e)
        {
            ShowHidePassword(textBoxUberSecret, UberDuckShowSecretPassword);
        }
        private void iconButton53_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void button48_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBoxReadFromTXTFile.Text = openFileDialog1.FileName;
            }
        }
        private void button49_Click(object sender, EventArgs e)
        {
            string path = VoiceWizardWindow.MainFormGlobal.textBoxReadFromTXTFile.Text.ToString();
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string relativePath = path;
            string absPath = Path.Combine(basePath, relativePath);
            TextFileReader.FileToTTS(absPath);
        }
        private void rjToggleButton14_CheckedChanged(object sender, EventArgs e)
        {
            if (rjToggleButtonReadFromFile.Checked )
            {
                TextFileReader.ReadFromFile();
            }
            else
            {
                TextFileReader.StopWatcher();
            }
        }
        private void button11_Click_1(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                modelTextBox.Text = folderBrowserDialog1.SelectedPath;
            }
        }
        private void button41_Click(object sender, EventArgs e)
        {
            VoiceWizardWindow.MainFormGlobal.textBoxWhisperMinDuration.Text = "1.0";
            VoiceWizardWindow.MainFormGlobal.textBoxWhisperMaxDuration.Text = "8.0";
            VoiceWizardWindow.MainFormGlobal.textBoxWhisperDropSilence.Text = "0.25";
            VoiceWizardWindow.MainFormGlobal.textBoxWhisperPauseDuration.Text = "1.0";
        }
        private void button42_Click(object sender, EventArgs e)
        {
            WhisperRecognition.downloadWhisperModel();
        }
        private void comboBoxWhisperModelDownload_SelectedIndexChanged(object sender, EventArgs e)
        {
            string path = "Assets/models/";
            switch (comboBoxWhisperModelDownload.Text.ToString())
            {
                case "ggml-tiny.bin (75 MB)":
                    path += "ggml-tiny.bin";
                    break;
                case "ggml-base.bin (142 MB)":
                    path += "ggml-base.bin";
                    break;
                case "ggml-small.bin (466 MB)":
                    path += "ggml-small.bin";
                    break;
                case "ggml-medium.bin (1.5 GB)":
                    path += "ggml-medium.bin";
                    break;
                default: break;
            }
            if (System.IO.File.Exists(path))
            {
                modelLabel.ForeColor = Color.Green;
                modelLabel.Text = "model downloaded";
            }
            else
            {
                modelLabel.ForeColor = Color.Red;
                modelLabel.Text = "model not downloaded";
            }
        }
        private void button34_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                whisperModelTextBox.Text = openFileDialog1.FileName;
            }
        }
        private void voskLink_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void whisperLink_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void hidePasswordChatGPT_Click(object sender, EventArgs e)
        {
            ShowHidePassword(textBoxChatGPT, hidePasswordChatGPT);
        }
        private void buttonApplyChatGPT_Click(object sender, EventArgs e)
        {
            string key = VoiceWizardWindow.MainFormGlobal.textBoxChatGPT.Text.ToString();
            string model = VoiceWizardWindow.MainFormGlobal.textBoxGPTModel.Text.ToString();
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(model))
            {
                OutputText.outputLog("[ChatGPT Error: key or model text field is blank ]", Color.Red);
            }
            else
            {
                ChatGPTAPI.ChatGPTMode = "Key";
                ChatGPTAPI.OfficialBotSetAPIKey(key, model);
                OutputText.outputLog("[ChatGPT loaded with API key]", Color.Green);
                Settings1.Default.ChatGPTAPIKey = VoiceWizardWindow.MainFormGlobal.textBoxChatGPT.Text;
                Settings1.Default.ChatGPTModel = VoiceWizardWindow.MainFormGlobal.textBoxGPTModel.Text;
                Settings1.Default.Save();
            }
        }
        private void button23_Click(object sender, EventArgs e)
        {
                ChatGPTAPI.chatSession.Messages.Clear();
                ChatGPTAPI.messagesInHistory = ChatGPTAPI.chatSession.Messages.Count;
                OutputText.outputLog($"[ChatGPT message history cleared]");
        }
        private void iconButton12_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void button36_Click_1(object sender, EventArgs e)
        {
            string key = VoiceWizardWindow.MainFormGlobal.textBoxChatGPT.Text.ToString();
            string model = VoiceWizardWindow.MainFormGlobal.textBoxGPTModel.Text.ToString();
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(model))
            {
                OutputText.outputLog("[ChatGPT Error: key or model text field is blank ]", Color.Red);
            }
            else
            {
                ChatGPTAPI.ChatGPTMode = "Key";
                ChatGPTAPI.OfficialBotSetAPIKey(key, model);
                OutputText.outputLog("[ChatGPT loaded with API key]", Color.Green);
                Settings1.Default.ChatGPTAPIKey = VoiceWizardWindow.MainFormGlobal.textBoxChatGPT.Text;
                Settings1.Default.ChatGPTModel = VoiceWizardWindow.MainFormGlobal.textBoxGPTModel.Text;
                Settings1.Default.Save();
            }
        }
        private void button43_Click_1(object sender, EventArgs e)
        {
            VRChatListener.setValues();
            VoiceWizardWindow.MainFormGlobal.buttonCountersApplyChanges.Enabled = false;
            VoiceWizardWindow.MainFormGlobal.buttonCountersApplyChanges.ForeColor = Color.White;
        }
        private static void OnBoxesChanged(object sender, EventArgs e)
        {
            if (VoiceWizardWindow.MainFormGlobal.buttonCountersApplyChanges.Enabled == false && settingsLoaded )
            {
                OutputText.outputLog("[Counter Debug: Click the apply changes button for any changes to take effect]", Color.DarkOrange);
                VoiceWizardWindow.MainFormGlobal.buttonCountersApplyChanges.Enabled = true;
                VoiceWizardWindow.MainFormGlobal.buttonCountersApplyChanges.ForeColor = Color.Gold;
            }
        }
        private void label251_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https:
        }
        private void iconButton7_Click_1(object sender, EventArgs e)
        {
            ShowHidePassword(textBoxTikTokSessionID, iconButton7);
        }
    }
}
