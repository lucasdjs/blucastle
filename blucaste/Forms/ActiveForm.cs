using blucaste.Data;
using Google.Cloud.Firestore;

namespace blucaste.Forms
{
    public partial class ActiveForm : Form
    {
        public ActiveForm()
        {
            InitializeComponent();
        }

        public string Uid => textBoxIdentificador.Text;

        public string Serial
        {
            set => textBoxSerial.Text = value;
        }

        private async void buttonSalvar_Click(object sender, EventArgs e)
        {
            string serial = textBoxSerial.Text.Trim();
            string identificador = textBoxIdentificador.Text.Trim();

            if (string.IsNullOrEmpty(identificador))
            {
                MessageBox.Show("Por favor, insira um identificador.");
                return;
            }

            var firestore = new FirestoreService();

            var usuario = await firestore.BuscarUsuarioPorIdentificadorAsync(identificador);

            if (usuario != null)
            {
                // Verifica status
                if (!usuario.status)
                {
                    MessageBox.Show("Usuário está desativado ou sem créditos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (usuario.tipoUso == 0)
                {
                    if (usuario.quantidadeUso <= 0)
                    {
                        MessageBox.Show("Usuário não possui mais créditos disponíveis.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
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
                        return;
                    }
                }

                bool atualizado = await firestore.AtualizarSerialDoUsuarioAsync(usuario.Uid, serial);

                if (atualizado)
                {
                    MessageBox.Show("Serial atualizado com sucesso!");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Falha ao atualizar o serial.");
                }
            }
            else
            {
                MessageBox.Show("Chave não encontrada.");
            }
        }

    }
}
