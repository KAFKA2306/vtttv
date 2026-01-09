using System;
using System.Collections.Generic;
using System.Text;
using DeepL;
using OSCVRCWiz.Services.Text;
using OSCVRCWiz.Settings;

namespace OSCVRCWiz.Services.Speech.TranslationAPIs
{
    public class DeepLTranslate
    {

        public static async Task<string> translateTextDeepL(string text)
        {
            try
            {
                var translator = new Translator(Settings1.Default.deepLKeysave);
                var fullFromLanguage = "";
                var fullToLanguage = "";
                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    fullFromLanguage = VoiceWizardWindow.MainFormGlobal.comboBoxSpokenLanguage.SelectedItem.ToString();
                    fullToLanguage = VoiceWizardWindow.MainFormGlobal.comboBoxTranslationLanguage.SelectedItem.ToString();
                });

                var from = LanguageSelect.fromLanguageNew(fullFromLanguage, "sourceLanguage", "DeepL");
                var to = LanguageSelect.fromLanguageNew(fullToLanguage, "targetLanguage", "DeepL");

                switch (to)
                {

                    case "en": to = "en-US"; break;
                }

                var translatedText = await translator.TranslateTextAsync(text, from, to);
                System.Diagnostics.Debug.WriteLine(translatedText);

                return translatedText.ToString();

            }
            catch (Exception ex)
            {

                var errorMsg = ex.Message + "\n" + ex.TargetSite + "\n\nStack Trace:\n" + ex.StackTrace;

                try
                {
                    errorMsg += "\n\n" + ex.InnerException.Message + "\n" + ex.InnerException.TargetSite + "\n\nStack Trace:\n" + ex.InnerException.StackTrace;

                }
                catch { }
                OutputText.outputLog("[DeepL Translation Error: " + errorMsg + "]", Color.Red);
                OutputText.outputLog("[You are attempting to translate from one language to another. If this is not your intent then switch 'Translation Language' back to 'No Translation (Default)'. If you are intending to translate then get a VoiceWizardPro key or a DeepL key. ]", Color.DarkOrange);
                OutputText.outputLog("[Learn how to get a Language Translation Key: https://ttsvoicewizard.com/docs/Translation/DeepL ]", Color.DarkOrange);
                return "";
            }
        }

    }
}
