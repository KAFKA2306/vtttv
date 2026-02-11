
using ChatGPT.Net;
using ChatGPT.Net.DTO.ChatGPT;
using OSCVRCWiz.Services.Text;

namespace OSCVRCWiz.Services.Integrations
{
    public class ChatGPTAPI
    {

        private static ChatGpt OfficialBot=null;

        public static ChatGptConversation chatSession;

        public static int messagesInHistory = 0;

        public static string ChatGPTMode = "";
        public static void OfficialBotSetAPIKey(string key,string model)
        {

            OfficialBot = new ChatGpt(key, new ChatGptOptions
            {
                Model = model,
            });

            chatSession = new ChatGptConversation();

        }

        public static async Task<string> GPTResponse(string input)
        {
            string response = "";
            try
            {
                if (ChatGPTMode == "")
                {
                    string key = VoiceWizardWindow.MainFormGlobal.textBoxChatGPT.Text.ToString();
                    string model = VoiceWizardWindow.MainFormGlobal.textBoxGPTModel.Text.ToString();
                    if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(model))
                    {
                        OutputText.outputLog("[ChatGPT Error: Key or model text field is blank]", Color.Red);
                        return response;
                    }
                    else
                    {

                            ChatGPTAPI.ChatGPTMode = "Key";
                            ChatGPTAPI.OfficialBotSetAPIKey(key, model);
                            OutputText.outputLog("[ChatGPT loaded with API key]", Color.Green);

                    }

                }
                if (ChatGPTMode == "Key")
                {
                    if (VoiceWizardWindow.MainFormGlobal.rjToggleGPTUsePrompt.Checked)
                    {
                        VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                        {
                            input = VoiceWizardWindow.MainFormGlobal.richTextBoxGPTPrompt.Text.ToString().Trim() + ": " + input;
                        });
                    }

                    if (VoiceWizardWindow.MainFormGlobal.rjToggleUseContextWithGPT.Checked)
                    {
                        response = await OfficialBot.Ask(input, chatSession.Id);

                        ChatGptMessage messageUser = new ChatGptMessage();
                        messageUser.Role = "user";
                        messageUser.Content = input;
                        chatSession.Messages.Add(messageUser);

                        ChatGptMessage messageAssistant = new ChatGptMessage();
                        messageAssistant.Role = "assistant";
                        messageAssistant.Content = response;
                        chatSession.Messages.Add(messageAssistant);

                        int messageCount = chatSession.Messages.Count;
                        int maxMessages = Int16.Parse(VoiceWizardWindow.MainFormGlobal.textBoxChatGPTMaxHistory.Text);

                        if (maxMessages < 6)
                        {
                            OutputText.outputLog("Max Message Context cannot be less than 6, the context was " + maxMessages, Color.DarkOrange);

                            maxMessages = 6;
                        }
                        if (maxMessages % 2 != 0)
                        {
                            OutputText.outputLog("Message Context cannot be odd the max context was " + maxMessages, Color.DarkOrange);
                            maxMessages += 1;
                        }

                        if (messageCount >= maxMessages)
                        {
                            int half = messageCount / 2;
                            if (half % 2 == 0)
                            {
                                int userIndex = half;
                                int assistantIndex = half + 1;

                                chatSession.Messages.RemoveAt(assistantIndex);
                                chatSession.Messages.RemoveAt(userIndex);

                            }
                            else
                            {
                                int assistantIndex = half;
                                int userIndex = half - 1;

                                chatSession.Messages.RemoveAt(assistantIndex);
                                chatSession.Messages.RemoveAt(userIndex);

                            }

                        }

                    }
                    else
                    {
                        response = await OfficialBot.Ask(input);
                    }

               }

            }
            catch (Exception ex)
            {
                OutputText.outputLog("[ChatGPT Error: " + ex.Message + "]", Color.Red);
            }
            return response;
        }

    }
}
