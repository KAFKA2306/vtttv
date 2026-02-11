using static OSCVRCWiz.VoiceWizardWindow;
using System.Text.RegularExpressions;
using OSCVRCWiz.Services.Text;
using System.Windows.Input;

namespace OSCVRCWiz.Services.Integrations
{
    public class WordReplacements
    {
        private static Dictionary<string, string> replaceDict = new Dictionary<string, string>();

        public static string wordReplacemntsStored = "";

        public static string MainDoWordReplacement(string text)
        {
            MainFormGlobal.Invoke((MethodInvoker)delegate ()
            {
                int index = 0;
                foreach (var kvp in replaceDict)
                {

                    if (text.Contains(kvp.Key.ToString(), StringComparison.OrdinalIgnoreCase))
                    {

                        if (MainFormGlobal.checkedListBoxReplacements.GetItemCheckState(index) == CheckState.Checked)
                        {

                            string pattern = Regex.Escape(kvp.Key.ToString());
                            string key = kvp.Key.ToString();
                            if (VoiceWizardWindow.MainFormGlobal.rjToggleUseWordBoundaries.Checked)
                            {

                                if ((char.IsLetterOrDigit(key[0]) || key[0] == '_') && (char.IsLetterOrDigit(key[^1]) || key[^1] == '_'))
                                {

                                    pattern = $@"\b{Regex.Escape(key)}\b";
                                }
                                else
                                {

                                    pattern = $@"(?<![\w]){Regex.Escape(key)}(?![\w])";
                                }
                            }
                            text = Regex.Replace(text, pattern, kvp.Value.ToString(), RegexOptions.IgnoreCase);
                        }

                    }
                    index++;
                }

            });
            return text;
        }

        public static void addWordReplacement(string wordKey, string wordValue)
        {

            try
            {

                replaceDict.Add(wordKey, wordValue);
                MainFormGlobal.checkedListBoxReplacements.Items.Add($"{MainFormGlobal.checkedListBoxReplacements.Items.Count + 1} | {wordKey} ---> {wordValue}", true);
                replacementSave();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        public static void removeWordReplacementAt(int index)
        {
            replaceDict.Remove(replaceDict.ElementAt(index).Key);
            MainFormGlobal.checkedListBoxReplacements.Items.RemoveAt(index);

        }

        public static void clearWordReplacement()
        {
            replaceDict.Clear();
            MainFormGlobal.checkedListBoxReplacements.Items.Clear();

        }

        public static void replacementsLoad()
        {

            string words = wordReplacemntsStored;
            string[] split = words.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in split)
            {
                if (s.Trim() != "")
                {
                    string words2 = s;
                    int count = 1;
                    string wordKey = "";
                    string wordValue = "";
                    string[] split2 = words2.Split(new char[] { '¬' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string s2 in split2)
                    {

                        if (count == 1)
                        {
                            wordKey = s2;
                            System.Diagnostics.Debug.WriteLine("Phrase Added: " + s2);

                        }
                        if (count == 2)
                        {
                            wordValue = s2;
                            System.Diagnostics.Debug.WriteLine("address added: " + s2);

                        }

                        count++;
                    }
                    try
                    {

                        MainFormGlobal.checkedListBoxReplacements.Items.Add($"{MainFormGlobal.checkedListBoxReplacements.Items.Count + 1} | {wordKey} ---> {wordValue}", true);
                        replaceDict.Add(wordKey, wordValue);
                    }
                    catch (Exception ex)
                    {
                        OutputText.outputLog("Error Loading Word Replacements / No Word Replacements Found", Color.DarkOrange);
                    }
                }
            }

        }
        public static void replacementSave()
        {
            wordReplacemntsStored = "";
            foreach (var kvp in replaceDict)
            {

                wordReplacemntsStored += $"{kvp.Key}¬{kvp.Value};";
            }
        }
    }
}
