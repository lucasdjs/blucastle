namespace blucaste
{
    partial class Inicializer
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Inicializer));
            comboBoxSelectedOptionRouter = new ComboBox();
            checkBoxPreset = new CheckBox();
            comboBoxInterface = new ComboBox();
            buttonStart = new Button();
            textBoxUser = new TextBox();
            textBoxPassword = new TextBox();
            textBoxWifi24 = new TextBox();
            textBoxWifi5G = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            pictureBoxSpinner = new PictureBox();
            button1 = new Button();
            pictureBoxSpin = new PictureBox();
            label5 = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBoxSpinner).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxSpin).BeginInit();
            SuspendLayout();
            // 
            // comboBoxSelectedOptionRouter
            // 
            comboBoxSelectedOptionRouter.BackColor = SystemColors.Window;
            comboBoxSelectedOptionRouter.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxSelectedOptionRouter.ForeColor = Color.Black;
            comboBoxSelectedOptionRouter.FormattingEnabled = true;
            comboBoxSelectedOptionRouter.Location = new Point(226, 129);
            comboBoxSelectedOptionRouter.Name = "comboBoxSelectedOptionRouter";
            comboBoxSelectedOptionRouter.Size = new Size(82, 28);
            comboBoxSelectedOptionRouter.TabIndex = 0;
            comboBoxSelectedOptionRouter.SelectedIndexChanged += ComboBoxSelectedOptionRouter_SelectedIndexChanged;
            // 
            // checkBoxPreset
            // 
            checkBoxPreset.AutoSize = true;
            checkBoxPreset.ForeColor = Color.White;
            checkBoxPreset.Location = new Point(23, 263);
            checkBoxPreset.Name = "checkBoxPreset";
            checkBoxPreset.Size = new Size(71, 24);
            checkBoxPreset.TabIndex = 2;
            checkBoxPreset.Text = "Preset";
            checkBoxPreset.UseVisualStyleBackColor = true;
            checkBoxPreset.CheckedChanged += CheckBoxPreset_CheckedChanged;
            // 
            // comboBoxInterface
            // 
            comboBoxInterface.BackColor = SystemColors.Window;
            comboBoxInterface.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxInterface.ForeColor = Color.Black;
            comboBoxInterface.FormattingEnabled = true;
            comboBoxInterface.Location = new Point(98, 129);
            comboBoxInterface.Name = "comboBoxInterface";
            comboBoxInterface.Size = new Size(109, 28);
            comboBoxInterface.TabIndex = 3;
            comboBoxInterface.SelectedIndexChanged += ComboBoxSelectedOptionRouter_SelectedIndexChanged;
            // 
            // buttonStart
            // 
            buttonStart.BackColor = Color.FromArgb(64, 64, 64);
            buttonStart.Font = new Font("Segoe UI", 12F);
            buttonStart.ForeColor = Color.White;
            buttonStart.Location = new Point(126, 163);
            buttonStart.Name = "buttonStart";
            buttonStart.Size = new Size(159, 53);
            buttonStart.TabIndex = 4;
            buttonStart.Text = "START";
            buttonStart.UseVisualStyleBackColor = false;
            buttonStart.Click += ButtonStart_Click;
            // 
            // textBoxUser
            // 
            textBoxUser.Location = new Point(98, 305);
            textBoxUser.Name = "textBoxUser";
            textBoxUser.Size = new Size(125, 27);
            textBoxUser.TabIndex = 5;
            // 
            // textBoxPassword
            // 
            textBoxPassword.Location = new Point(98, 338);
            textBoxPassword.Name = "textBoxPassword";
            textBoxPassword.Size = new Size(125, 27);
            textBoxPassword.TabIndex = 6;
            // 
            // textBoxWifi24
            // 
            textBoxWifi24.Location = new Point(98, 380);
            textBoxWifi24.Name = "textBoxWifi24";
            textBoxWifi24.ReadOnly = true;
            textBoxWifi24.Size = new Size(125, 27);
            textBoxWifi24.TabIndex = 7;
            textBoxWifi24.Text = "Blucaste_2.4G";
            // 
            // textBoxWifi5G
            // 
            textBoxWifi5G.Location = new Point(98, 413);
            textBoxWifi5G.Name = "textBoxWifi5G";
            textBoxWifi5G.ReadOnly = true;
            textBoxWifi5G.Size = new Size(125, 27);
            textBoxWifi5G.TabIndex = 8;
            textBoxWifi5G.Text = "Blucaste_5G";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.FromArgb(64, 64, 64);
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.White;
            label1.Location = new Point(19, 308);
            label1.Name = "label1";
            label1.Size = new Size(67, 20);
            label1.TabIndex = 9;
            label1.Text = "Usuario:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label2.ForeColor = Color.White;
            label2.Location = new Point(19, 341);
            label2.Name = "label2";
            label2.Size = new Size(55, 20);
            label2.TabIndex = 10;
            label2.Text = "Senha:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            label3.ForeColor = Color.White;
            label3.Location = new Point(19, 385);
            label3.Name = "label3";
            label3.Size = new Size(78, 19);
            label3.TabIndex = 11;
            label3.Text = "WIFI-2.4G:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            label4.ForeColor = Color.White;
            label4.Location = new Point(19, 418);
            label4.Name = "label4";
            label4.Size = new Size(66, 19);
            label4.TabIndex = 12;
            label4.Text = "WIFI-5G:";
            // 
            // pictureBoxSpinner
            // 
            pictureBoxSpinner.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBoxSpinner.BackgroundImageLayout = ImageLayout.Center;
            pictureBoxSpinner.Image = (Image)resources.GetObject("pictureBoxSpinner.Image");
            pictureBoxSpinner.Location = new Point(110, 305);
            pictureBoxSpinner.Name = "pictureBoxSpinner";
            pictureBoxSpinner.Size = new Size(230, 0);
            pictureBoxSpinner.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxSpinner.TabIndex = 13;
            pictureBoxSpinner.TabStop = false;
            pictureBoxSpinner.Visible = false;
            // 
            // button1
            // 
            button1.BackColor = Color.FromArgb(64, 64, 64);
            button1.ForeColor = Color.White;
            button1.Image = (Image)resources.GetObject("button1.Image");
            button1.ImageAlign = ContentAlignment.MiddleLeft;
            button1.Location = new Point(279, 394);
            button1.Name = "button1";
            button1.Size = new Size(115, 43);
            button1.TabIndex = 14;
            button1.Text = "Suporte";
            button1.TextAlign = ContentAlignment.MiddleRight;
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // pictureBoxSpin
            // 
            pictureBoxSpin.BackColor = Color.Transparent;
            pictureBoxSpin.BackgroundImageLayout = ImageLayout.Center;
            pictureBoxSpin.Image = (Image)resources.GetObject("pictureBoxSpin.Image");
            pictureBoxSpin.Location = new Point(126, 163);
            pictureBoxSpin.Name = "pictureBoxSpin";
            pictureBoxSpin.Size = new Size(159, 112);
            pictureBoxSpin.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBoxSpin.TabIndex = 15;
            pictureBoxSpin.TabStop = false;
            pictureBoxSpin.Visible = false;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.BackColor = Color.Transparent;
            label5.Font = new Font("Segoe UI", 10.8F);
            label5.Location = new Point(342, 21);
            label5.Name = "label5";
            label5.Size = new Size(52, 25);
            label5.TabIndex = 16;
            label5.Text = "V 1.0";
            // 
            // Inicializer
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(64, 64, 64);
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            ClientSize = new Size(406, 451);
            Controls.Add(label5);
            Controls.Add(pictureBoxSpin);
            Controls.Add(button1);
            Controls.Add(pictureBoxSpinner);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(textBoxWifi5G);
            Controls.Add(textBoxWifi24);
            Controls.Add(textBoxPassword);
            Controls.Add(textBoxUser);
            Controls.Add(buttonStart);
            Controls.Add(comboBoxInterface);
            Controls.Add(checkBoxPreset);
            Controls.Add(comboBoxSelectedOptionRouter);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Inicializer";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Blucastle";
            ((System.ComponentModel.ISupportInitialize)pictureBoxSpinner).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxSpin).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox comboBoxSelectedOptionRouter;
        private CheckBox checkBoxPreset;
        private ComboBox comboBoxInterface;
        private Button buttonStart;
        private TextBox textBoxUser;
        private TextBox textBoxPassword;
        private TextBox textBoxWifi24;
        private TextBox textBoxWifi5G;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private PictureBox pictureBoxSpinner;
        private Button button1;
        private PictureBox pictureBoxSpin;
        private Label label5;
    }
}
