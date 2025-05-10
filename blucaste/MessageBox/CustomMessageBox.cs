public class CustomMessageBox : Form
{
    private Label messageLabel;
    private Button okButton;
    private PictureBox pictureBoxFail;
    private PictureBox pictureBoxSucess;

    public CustomMessageBox(string message, bool isSuccess)
    {
        InitializeComponent(); // ESSENCIAL!

        // Exibir a imagem de acordo com sucesso ou falha
        pictureBoxSucess.Visible = isSuccess;
        pictureBoxFail.Visible = !isSuccess;

        // Label da mensagem
        messageLabel = new Label()
        {
            Text = message,
            AutoSize = true,
            Font = new Font("Arial", 10),
            MaximumSize = new Size(240, 0)
        };
        this.Controls.Add(messageLabel);

        // Centraliza verticalmente o texto em relação à imagem
        int messageX = pictureBoxFail.Right + 10;
        int messageY = pictureBoxFail.Top + (pictureBoxFail.Height - messageLabel.PreferredHeight) / 2;
        messageLabel.Location = new Point(messageX, messageY);

        // Botão OK
        okButton = new Button()
        {
            Text = "OK",
            Size = new Size(75, 30)
        };
        this.Controls.Add(okButton);

        // Posição do botão: abaixo do texto, centralizado na janela
        int buttonY = messageLabel.Bottom + 10;
        int formCenter = this.ClientSize.Width / 2;
        okButton.Location = new Point(formCenter - okButton.Width / 2, buttonY);

        // Ajustar altura do formulário dinamicamente
        int bottomPadding = 20;
        this.ClientSize = new Size(this.ClientSize.Width, okButton.Bottom + bottomPadding);

        okButton.Click += (sender, e) => this.Close();
    }

    public static void ShowMessage (string message, bool isSuccess) {
        var customMessageBox = new CustomMessageBox(message, isSuccess) {
            TopMost = true,
            ShowInTaskbar = false
        };

        // Garante que a janela esteja visível e com foco real
        customMessageBox.Load += (s, e) =>
        {
            customMessageBox.Activate();
            customMessageBox.BringToFront();
            customMessageBox.Focus();
        };

        customMessageBox.ShowDialog();
    }

    private void InitializeComponent () {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomMessageBox));
        pictureBoxFail = new PictureBox();
        pictureBoxSucess = new PictureBox();
        ((System.ComponentModel.ISupportInitialize)pictureBoxFail).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pictureBoxSucess).BeginInit();
        SuspendLayout();
        // 
        // pictureBoxFail
        // 
        pictureBoxFail.Image = (Image)resources.GetObject("pictureBoxFail.Image");
        pictureBoxFail.Location = new Point(12, 20);
        pictureBoxFail.Name = "pictureBoxFail";
        pictureBoxFail.Size = new Size(48, 46);
        pictureBoxFail.SizeMode = PictureBoxSizeMode.Zoom;
        pictureBoxFail.TabIndex = 0;
        pictureBoxFail.TabStop = false;
        pictureBoxFail.Visible = false;
        // 
        // pictureBoxSucess
        // 
        pictureBoxSucess.Image = (Image)resources.GetObject("pictureBoxSucess.Image");
        pictureBoxSucess.Location = new Point(12, 20);
        pictureBoxSucess.Name = "pictureBoxSucess";
        pictureBoxSucess.Size = new Size(48, 46);
        pictureBoxSucess.SizeMode = PictureBoxSizeMode.Zoom;
        pictureBoxSucess.TabIndex = 1;
        pictureBoxSucess.TabStop = false;
        pictureBoxSucess.Visible = false;
        // 
        // CustomMessageBox
        // 
        AutoValidate = AutoValidate.EnablePreventFocusChange;
        ClientSize = new Size(266, 115);
        ControlBox = false;
        Controls.Add(pictureBoxSucess);
        Controls.Add(pictureBoxFail);
        FormBorderStyle = FormBorderStyle.FixedToolWindow;
        MaximizeBox = false;
        MdiChildrenMinimizedAnchorBottom = false;
        MinimizeBox = false;
        Name = "CustomMessageBox";
        StartPosition = FormStartPosition.CenterScreen;
        ((System.ComponentModel.ISupportInitialize)pictureBoxFail).EndInit();
        ((System.ComponentModel.ISupportInitialize)pictureBoxSucess).EndInit();
        ResumeLayout(false);
        this.TopMost = true;
    }
}
