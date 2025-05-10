namespace blucaste.Forms
{
    partial class ActiveForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ActiveForm));
            textBoxSerial = new TextBox();
            textBoxIdentificador = new TextBox();
            buttonSalvar = new Button();
            labelSerial = new Label();
            label1 = new Label();
            label2 = new Label();
            SuspendLayout();
            // 
            // textBoxSerial
            // 
            textBoxSerial.BackColor = SystemColors.ButtonFace;
            textBoxSerial.Location = new Point(36, 176);
            textBoxSerial.Name = "textBoxSerial";
            textBoxSerial.ReadOnly = true;
            textBoxSerial.Size = new Size(413, 27);
            textBoxSerial.TabIndex = 0;
            // 
            // textBoxIdentificador
            // 
            textBoxIdentificador.Location = new Point(36, 246);
            textBoxIdentificador.Name = "textBoxIdentificador";
            textBoxIdentificador.Size = new Size(413, 27);
            textBoxIdentificador.TabIndex = 1;
            // 
            // buttonSalvar
            // 
            buttonSalvar.Location = new Point(133, 309);
            buttonSalvar.Name = "buttonSalvar";
            buttonSalvar.Size = new Size(167, 29);
            buttonSalvar.TabIndex = 2;
            buttonSalvar.Text = "Ativar";
            buttonSalvar.UseVisualStyleBackColor = true;
            buttonSalvar.Click += buttonSalvar_Click;
            // 
            // labelSerial
            // 
            labelSerial.AutoSize = true;
            labelSerial.BackColor = Color.Transparent;
            labelSerial.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelSerial.ForeColor = Color.White;
            labelSerial.Location = new Point(36, 145);
            labelSerial.Name = "labelSerial";
            labelSerial.Size = new Size(65, 28);
            labelSerial.TabIndex = 3;
            labelSerial.Text = "Serial";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.White;
            label1.Location = new Point(36, 215);
            label1.Name = "label1";
            label1.Size = new Size(69, 28);
            label1.TabIndex = 4;
            label1.Text = "Chave";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.White;
            label2.Location = new Point(99, 32);
            label2.Name = "label2";
            label2.Size = new Size(264, 41);
            label2.TabIndex = 5;
            label2.Text = "Ativação Blucaste";
            // 
            // ActiveForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(64, 64, 64);
            ClientSize = new Size(464, 450);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(labelSerial);
            Controls.Add(buttonSalvar);
            Controls.Add(textBoxIdentificador);
            Controls.Add(textBoxSerial);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ActiveForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Blucaste";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBoxSerial;
        private TextBox textBoxIdentificador;
        private Button buttonSalvar;
        private Label labelSerial;
        private Label label1;
        private Label label2;
    }
}