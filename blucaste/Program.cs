using blucaste.Data;
using blucaste.Forms;
using blucaste.Gateway;
using blucaste.Logger;
using blucaste.Models;
using Google.Cloud.Firestore;
using System.Diagnostics;
using System.Security.Principal;

namespace blucaste
{
    public static class Program
    {
        public static Usuario Usuario;

        [STAThread]
        public static void Main()
        {
            RunMainAsync().GetAwaiter().GetResult();
        }

        private static async Task RunMainAsync()
        {
            ApplicationConfiguration.Initialize();
            var usuario = new Usuario();

            // Se quiser ativar novamente o controle de administrador, descomente:
            if (!IsRunningAsAdministrator())
            {
                RestartAsAdministrator();
                return;
            }

            var machineGuid = ConfigSettings.GetCpuAndMotherboardGuid();

            var (connection, user) = await VerificarOuAtivarUsuario(machineGuid.ToString());

            if (!connection)
            {
                MessageBox.Show("Erro na verificação ou ativação do usuário.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            usuario = user;
            Usuario = user;
            TelnetLogger.IsLoggingEnabled = usuario.log;

            Application.Run(new Inicializer(usuario));
        }

        private static async Task<(bool sucesso, Usuario user)> VerificarOuAtivarUsuario(string serial)
        {
            var firestore = new FirestoreService();
            var doc = await firestore.BuscarUsuarioPorSerialAsync(serial);

            if (doc != null)
            {
                var usuario = doc.ConvertTo<Usuario>();

                if (!usuario.status)
                {
                    MessageBox.Show("Usuário está desativado ou sem créditos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return (false, usuario);
                }

                if (usuario.tipoUso == 0)
                {
                    if (usuario.quantidadeUso <= 0)
                    {
                        MessageBox.Show("Usuário não possui mais créditos disponíveis.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return (false, usuario);
                    }
                }
                else if (usuario.tipoUso == 1)
                {
                    DateTime dataValidade;

                    if (usuario.validade is Timestamp ts)
                        dataValidade = ts.ToDateTime();
                    else if (DateTime.TryParse(usuario.validade.ToString(), out DateTime parsed))
                        dataValidade = parsed;
                    else
                        dataValidade = DateTime.MinValue;

                    if (dataValidade.Date < DateTime.Today)
                    {
                        MessageBox.Show("Licença expirada.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return (false, usuario);
                    }
                }

                return (true, usuario);
            }

            using (var form = new ActiveForm())
            {
                form.Serial = serial;

                if (form.ShowDialog() == DialogResult.OK)
                {
                  var novoDoc = await firestore.BuscarUsuarioPorSerialAsync(serial);
                    var user = new Usuario();
                    if (novoDoc != null)
                    {
                        user = novoDoc.ConvertTo<Usuario>();
                        return (true, user);
                    }

                    return (true, user);
                }

                return (false, null);
            }
        }


        private static bool IsRunningAsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        private static void RestartAsAdministrator()
        {
            bool runAsCO = Environment.GetEnvironmentVariable("ClickOnce_IsNetworkDeployed")?.ToLower() == "true";

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = Application.ExecutablePath,
                    UseShellExecute = true,
                    Verb = "runas",
                    Arguments = runAsCO ? "rasco" : ""
                };

                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao solicitar privilégios de administrador", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}