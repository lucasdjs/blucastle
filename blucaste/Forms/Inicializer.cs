using System.Net.NetworkInformation;
using blucaste.Logger;
using blucaste.Models;
using blucaste.Services;

namespace blucaste
{
    public partial class Inicializer : Form
    {
        private System.Windows.Forms.Timer internetCheckTimer;
        private Usuario usuario;
        private bool conexaoPerdida = false;

        public Inicializer(Usuario usuario)
        {
            InitializeComponent();
            this.usuario = usuario;
            this.Text = usuario.display_name;
            this.Load += InicializerLoad;

            comboBoxSelectedOptionRouter.SelectedIndexChanged += ComboBoxSelectedOptionRouter_SelectedIndexChanged;
            checkBoxPreset.CheckedChanged += CheckBoxPreset_CheckedChanged;
            buttonStart.Click += ButtonStart_Click;

            if (usuario.tipoUso == 0)
                StartInternetMonitoring();
        }

        private void StartInternetMonitoring()
        {
            internetCheckTimer = new System.Windows.Forms.Timer();
            internetCheckTimer.Interval = 5000;
            internetCheckTimer.Tick += (s, e) =>
            {
                if (!IsInternetAvailable() && !conexaoPerdida)
                {
                    conexaoPerdida = true;
                    internetCheckTimer.Stop();
                    TelnetLogger.Log($"[ERRO DE CONEXÃO] Usuário: {usuario.display_name} perdeu a conexão com a internet.");

                    MessageBox.Show(
                        "Conexão com a internet perdida. Usuários com acesso por crédito precisam estar online.",
                        "Erro de conexão",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );

                    Application.Exit();
                }
            };

            internetCheckTimer.Start();
        }

        private bool IsInternetAvailable()
        {
            try
            {
                using (var ping = new Ping())
                {
                    var reply = ping.Send("8.8.8.8", 1000);
                    return reply.Status == IPStatus.Success;
                }
            }
            catch
            {
                return false;
            }
        }

        private void InicializerLoad(object sender, EventArgs e)
        {
            comboBoxInterface.Items.Clear();

            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var networkInterface in networkInterfaces)
            {
                if (networkInterface.OperationalStatus == OperationalStatus.Up &&
                    networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                {
                    comboBoxInterface.Items.Add(networkInterface.Name);
                }
            }

            if (comboBoxInterface.Items.Count > 1)
                comboBoxInterface.SelectedIndex = 1;
            else if (comboBoxInterface.Items.Count > 0)
                comboBoxInterface.SelectedIndex = 0;

            comboBoxSelectedOptionRouter.Items.Clear();
            comboBoxSelectedOptionRouter.Items.Add("Preta");
            comboBoxSelectedOptionRouter.Items.Add("Branca");
            comboBoxSelectedOptionRouter.SelectedIndex = 0;

            ApplyRouterPreset();
            UpdateTextBoxesEnabledState();
        }

        private void ComboBoxSelectedOptionRouter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyRouterPreset();
            UpdateTextBoxesEnabledState();
        }

        private void CheckBoxPreset_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTextBoxesEnabledState();
        }

        private void SetTextBoxState(TextBox textBox, bool enable)
        {
            textBox.ReadOnly = !enable;
            textBox.BackColor = enable ? Color.White : Color.LightGray;
            textBox.ForeColor = enable ? Color.Black : Color.DarkGray;
        }

        private void ApplyRouterPreset()
        {
            string selected = comboBoxSelectedOptionRouter.SelectedItem?.ToString()?.ToLower();

            if (selected == "preta")
            {
                textBoxUser.Text = "admin";
            }
            else if (selected == "branca")
            {
                textBoxUser.Text = "L1vt1m4eng";
            }

            textBoxPassword.Text = "admin";
        }

        private void UpdateTextBoxesEnabledState()
        {
            bool presetAtivo = checkBoxPreset.Checked;
            string selected = comboBoxSelectedOptionRouter.SelectedItem?.ToString()?.ToLower();

            bool podeEditarUser = presetAtivo && selected == "preta";
            bool podeEditarPassword = presetAtivo && (selected == "preta" || selected == "branca");
            bool podeEditarWifi = presetAtivo;

            SetTextBoxState(textBoxUser, podeEditarUser);
            SetTextBoxState(textBoxPassword, podeEditarPassword);
            SetTextBoxState(textBoxWifi24, podeEditarWifi);
            SetTextBoxState(textBoxWifi5G, podeEditarWifi);
        }

        private void BloquearControles()
        {
            pictureBoxSpin.Visible = true;
            buttonStart.Enabled = false;
            buttonStart.Visible = false;
            textBoxUser.Enabled = false;
            textBoxPassword.Enabled = false;
            textBoxWifi24.Enabled = false;
            textBoxWifi5G.Enabled = false;
            checkBoxPreset.Enabled = false;
            comboBoxInterface.Enabled = false;
            comboBoxSelectedOptionRouter.Enabled = false;
        }

        private void DesbloquearControles()
        {
            pictureBoxSpin.Visible = false;
            buttonStart.Enabled = true;
            buttonStart.Visible = true;
            textBoxUser.Enabled = true;
            textBoxPassword.Enabled = true;
            textBoxWifi24.Enabled = true;
            textBoxWifi5G.Enabled = true;
            checkBoxPreset.Enabled = true;
            comboBoxInterface.Enabled = true;
            comboBoxSelectedOptionRouter.Enabled = true;
        }

        private void ButtonStart_Click(object sender, EventArgs e)
        {
            BloquearControles();

            var interfaceName = comboBoxInterface.Text;
            var selectedRouter = comboBoxSelectedOptionRouter.SelectedItem?.ToString()?.ToLower();
            var user = textBoxUser.Text;
            var password = textBoxPassword.Text;
            var wifi24 = textBoxWifi24.Text;
            var wifi5G = textBoxWifi5G.Text;

            Task.Run(async () =>
            {
                var executor = new RouterExecutorAppService();
                await executor.ExecuteAsync(interfaceName, selectedRouter, user, password, wifi24, wifi5G);

                Invoke(new Action(() =>
                {
                    DesbloquearControles();
                }));
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://api.whatsapp.com/send?phone=5562982228917&text=Ol%C3%A1",
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(psi);
        }
    }
}
