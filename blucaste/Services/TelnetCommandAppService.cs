using blucaste.Logger;
using blucaste.Services;
using System.Media;

namespace NetworkServices.Telnet
{
    public class TelnetCommandAppService
    {
        private readonly string _host;
        private readonly int _port;
        private readonly string _username;
        private readonly string _password;

        public TelnetCommandAppService(string host, int port, string username, string password)
        {
            _host = host;
            _port = port;
            _username = username;
            _password = password;
        }

        public async Task ExecuteCommandsAsync(List<string> commands)
        {
            try
            {
                var owner = Form.ActiveForm;
                using var connection = new TelnetConnectionAppService();
                await connection.ConnectAsync(_host, _port);

                var login = new TelnetLoginAppService(_username, _password);
                if (!await login.PerformLoginAsync(connection.Writer, connection.Reader))
                {
                    SystemSounds.Hand.Play();
                    CustomMessageBox.ShowMessage("Use o backup enviado, tente novamente.", false);
                    return;
                }

                var executor = new TelnetCommandExecutor();
                if (!await executor.ExecuteCommandsAsync(connection.Writer, connection.Reader, commands))
                {
                    CustomMessageBox.ShowMessage("Erro na execução de comandos.", false);
                    return;
                }

                TelnetLogger.Log("Todos os comandos foram executados com sucesso.");
                SystemSounds.Beep.Play();
                CustomMessageBox.ShowMessage("Concluído, reiniciando!", true);
            }
            catch (Exception ex)
            {
                var owner = Form.ActiveForm;
                TelnetLogger.Log($"Erro: {ex.Message}");
                SystemSounds.Hand.Play();
                CustomMessageBox.ShowMessage("Use o backup enviado, tente novamente.", false);
            }
        }

        public static void ShowAlert(string message)
        {
            var owner = Form.ActiveForm;
            SystemSounds.Hand.Play();
            CustomMessageBox.ShowMessage(message, false);
        }
    }
}