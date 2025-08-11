using blucaste.Logger;
using blucaste.Models;
using blucaste.Services;
using System.Net.NetworkInformation;
using System.Text.Json;

namespace blucaste
{
    public partial class Inicializer : Form
    {
        private int _contadorDeErros;
        private bool _jaRodou = false;
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
            button2.Click += button2_Click;
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
                    TelnetLogger.Log($"[ERRO DE CONEX?O] Usu?rio: {usuario.display_name} perdeu a conex?o com a internet.");

                    MessageBox.Show(
                        "Conex?o com a internet perdida. Usu?rios com acesso por cr?dito precisam estar online.",
                        "Erro de conex?o",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );

                    Application.Exit();
                }
            };

            internetCheckTimer.Start();
        }
        private void SetTextBoxState(TextBox textBox, bool enable)
        {
            textBox.ReadOnly = !enable;
            textBox.BackColor = enable ? Color.White : Color.LightGray;
            textBox.ForeColor = enable ? Color.Black : Color.FromArgb(30, 30, 30);
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

            if (checkBoxPreset.Checked && comboBoxSelectedOptionRouter.SelectedItem != null)
            {
                LoadPreset(comboBoxSelectedOptionRouter.SelectedItem.ToString());
            }
            else
            {
                ApplyRouterPreset();
            }
            UpdateTextBoxesEnabledState();
            button2.Visible = checkBoxPreset.Checked;
        }

        private void ComboBoxSelectedOptionRouter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyRouterPreset();
            UpdateTextBoxesEnabledState();
        }

        private void CheckBoxPreset_CheckedChanged(object sender, EventArgs e)
        {
            string selectedRouter = comboBoxSelectedOptionRouter.SelectedItem?.ToString();

            // Mostrar ou ocultar o botão de salvar preset com base na checkbox
            button2.Visible = checkBoxPreset.Checked;

            if (checkBoxPreset.Checked)
            {
                if (!string.IsNullOrEmpty(selectedRouter))
                {
                    LoadPreset(selectedRouter);
                }
            }
            else
            {
                ApplyRouterPreset();
            }

            UpdateTextBoxesEnabledState();
        }

        private void ApplyRouterPreset()
        {
            string selected = comboBoxSelectedOptionRouter.SelectedItem?.ToString()?.ToLower();

            if (selected == "preta")
            {
                textBoxUser.Text = "admin";
                textBoxPassword.Text = "admin";
                textBoxWifi24.Text = "Blucastle_2.4G";
                textBoxWifi5G.Text = "Blucastle_5G";
            }
            else if (selected == "branca")
            {
                textBoxUser.Text = "telecomadmin";
                textBoxPassword.Text = "admin";
                textBoxWifi24.Text = "Blucastle_2.4G";
                textBoxWifi5G.Text = "Blucastle_5G";
            }
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
        private void LoadPreset(string selectedRouterType)
        {
            try
            {
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string blucastlePath = Path.Combine(documentsPath, "Blucastle");
                string fileName = $"preset_{selectedRouterType.ToLower()}.json";
                string filePath = Path.Combine(blucastlePath, fileName);

                if (File.Exists(filePath))
                {
                    string jsonString = File.ReadAllText(filePath);
                    PresetData loadedPreset = JsonSerializer.Deserialize<PresetData>(jsonString);

                    if (loadedPreset != null)
                    {
                        textBoxUser.Text = loadedPreset.User;
                        textBoxPassword.Text = loadedPreset.Password;
                        textBoxWifi24.Text = loadedPreset.Wifi24;
                        textBoxWifi5G.Text = loadedPreset.Wifi5G;
                    }
                }
                else
                {
                    ApplyRouterPreset();
                }
            }
            catch (System.Exception ex)
            {
                CustomMessageBox.ShowMessage($"Erro ao carregar preset: {ex.Message}", isSuccess: false);
                TelnetLogger.Log($"Erro ao carregar preset: {ex.Message}");
                ApplyRouterPreset();
            }
        }
        private void SaveCurrentPreset(string routerTypeToSave)
        {
            try
            {
                _contadorDeErros++;
                if (_contadorDeErros == 2) { _contadorDeErros = 0; return; }

                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string blucastlePath = Path.Combine(documentsPath, "Blucastle");

                if (!Directory.Exists(blucastlePath))
                {
                    Directory.CreateDirectory(blucastlePath);
                }

                var presetData = new PresetData
                {
                    User = textBoxUser.Text,
                    Password = textBoxPassword.Text,
                    Wifi24 = textBoxWifi24.Text,
                    Wifi5G = textBoxWifi5G.Text,
                    SelectedRouter = routerTypeToSave
                };

                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(presetData, options);

                string fileName = $"preset_{routerTypeToSave.ToLower()}.json";
                string filePath = Path.Combine(blucastlePath, fileName);

                File.WriteAllText(filePath, jsonString);

                CustomMessageBox.ShowMessage($"Preset '{routerTypeToSave}' salvo com sucesso!", isSuccess: true);
                TelnetLogger.Log($"Preset '{routerTypeToSave}' salvo com sucesso!");
            }
            catch (System.Exception ex)
            {
                CustomMessageBox.ShowMessage($"Erro ao salvar preset: {ex.Message}", isSuccess: false);
                TelnetLogger.Log($"Erro ao salvar preset: {ex.Message}");
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            string selectedRouter = comboBoxSelectedOptionRouter.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(selectedRouter))
            {
                return;
            }

            SaveCurrentPreset(selectedRouter);
        }
    }
}