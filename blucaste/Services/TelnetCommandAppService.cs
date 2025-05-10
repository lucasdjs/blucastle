using blucaste.Data;
using blucaste;
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
                using var connection = new TelnetConnectionAppService();
                await connection.ConnectAsync(_host, _port);

                var login = new TelnetLoginAppService(_username, _password);
                if (!await login.PerformLoginAsync(connection.Writer, connection.Reader))
                {
                    SystemSounds.Hand.Play();
                    ShowAlert("Use o backup enviado, tente novamente.");
                    return;
                }

                var executor = new TelnetCommandExecutor();
                if (!await executor.ExecuteCommandsAsync(connection.Writer, connection.Reader, commands))
                {
                    ShowAlert("Erro na execução de comandos.");
                    return;
                }

                if (Program.Usuario != null && Program.Usuario.tipoUso == 0)
                {
                    Program.Usuario.quantidadeUso--;
                    await new FirestoreService().AtualizarQuantidadeUsoAsync(Program.Usuario.Uid, Program.Usuario.quantidadeUso);
                    TelnetLogger.Log($"Crédito decrementado. Novo saldo: {Program.Usuario.quantidadeUso}");
                }

                TelnetLogger.Log("Todos os comandos foram executados com sucesso.");
                SystemSounds.Beep.Play();
                CustomMessageBox.ShowMessage("Operação concluída com sucesso!", true);
            }
            catch (Exception ex)
            {
                TelnetLogger.Log($"Erro: {ex.Message}");
                SystemSounds.Hand.Play();
                CustomMessageBox.ShowMessage("Use o backup enviado, tente novamente.", false);
            }
        }

        public static void ShowAlert(string message)
        {
            SystemSounds.Hand.Play();
            CustomMessageBox.ShowMessage(message, false);
        }
    }
}
