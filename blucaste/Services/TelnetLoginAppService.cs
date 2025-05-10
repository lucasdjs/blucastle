using blucaste.Logger;
using blucaste.Services.NetworkServices.Telnet;
using System.Text.RegularExpressions;

namespace blucaste.Services {
    public class TelnetLoginAppService {
        private readonly string _username;
        private readonly string _password;

        public TelnetLoginAppService (string username, string password) {
            _username = username;
            _password = password;
        }

        public async Task<bool> PerformLoginAsync (StreamWriter writer, StreamReader reader) {
            bool userSent = false;
            bool passwordSent = false;

            Regex loginRegex = new Regex(@"\blogin\b\s*:", RegexOptions.IgnoreCase);
            Regex passwordRegex = new Regex(@"\bpassword\b\s*:", RegexOptions.IgnoreCase);

            while (true) {
                string prompt = await TelnetHelperAppService.ReadResponseAsync(reader);
                TelnetLogger.Log($"Prompt de resposta recebida: {prompt}");

                if (!userSent && loginRegex.IsMatch(prompt)) {
                    TelnetLogger.Log("Login requisitado. Enviando usuário.");
                    await writer.WriteLineAsync(_username);
                    await writer.FlushAsync();
                    userSent = true;
                    continue;
                }

                if (userSent && !passwordSent && passwordRegex.IsMatch(prompt)) {
                    TelnetLogger.Log("Senha requisitada. Enviando senha.");
                    await writer.WriteLineAsync(_password);
                    await writer.FlushAsync();
                    passwordSent = true;
                    continue;
                }

                if (userSent && passwordSent && loginRegex.IsMatch(prompt)) {
                    TelnetLogger.Log("Login incorreto. Reiniciando tentativa.");
                    return false;
                }

                if (IsSuccessPrompt(prompt)) {
                    TelnetLogger.Log("Login bem-sucedido.");
                    return true;
                }

                await Task.Delay(200);
            }
        }


        private bool IsSuccessPrompt (string prompt) {
            return prompt.Contains("WAP>") || prompt.Contains("#") || prompt.Contains(">");
        }

    }
}
