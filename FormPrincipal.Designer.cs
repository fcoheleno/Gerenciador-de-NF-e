namespace Gerenciador_de_NF_e
{
    partial class FormPrincipal
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            btncertificados = new Guna.UI2.WinForms.Guna2Button();
            btnConsultar = new Guna.UI2.WinForms.Guna2Button();
            panelConteudo = new Guna.UI2.WinForms.Guna2Panel();
            label1 = new Label();
            guna2HtmlLabel1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            guna2Panel1.SuspendLayout();
            panelConteudo.SuspendLayout();
            SuspendLayout();
            // 
            // guna2Panel1
            // 
            guna2Panel1.BackColor = Color.Teal;
            guna2Panel1.Controls.Add(btncertificados);
            guna2Panel1.Controls.Add(btnConsultar);
            guna2Panel1.CustomizableEdges = customizableEdges5;
            guna2Panel1.Dock = DockStyle.Left;
            guna2Panel1.Location = new Point(0, 0);
            guna2Panel1.Name = "guna2Panel1";
            guna2Panel1.ShadowDecoration.CustomizableEdges = customizableEdges6;
            guna2Panel1.Size = new Size(226, 560);
            guna2Panel1.TabIndex = 0;
            // 
            // btncertificados
            // 
            btncertificados.BorderRadius = 20;
            btncertificados.CustomizableEdges = customizableEdges1;
            btncertificados.DisabledState.BorderColor = Color.DarkGray;
            btncertificados.DisabledState.CustomBorderColor = Color.DarkGray;
            btncertificados.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btncertificados.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btncertificados.FillColor = Color.Transparent;
            btncertificados.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btncertificados.ForeColor = Color.White;
            btncertificados.Location = new Point(25, 128);
            btncertificados.Name = "btncertificados";
            btncertificados.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btncertificados.Size = new Size(180, 45);
            btncertificados.TabIndex = 1;
            btncertificados.Text = "Certificados";
            // 
            // btnConsultar
            // 
            btnConsultar.BorderRadius = 20;
            btnConsultar.CustomizableEdges = customizableEdges3;
            btnConsultar.DisabledState.BorderColor = Color.DarkGray;
            btnConsultar.DisabledState.CustomBorderColor = Color.DarkGray;
            btnConsultar.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnConsultar.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnConsultar.FillColor = Color.Transparent;
            btnConsultar.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnConsultar.ForeColor = Color.White;
            btnConsultar.Location = new Point(25, 67);
            btnConsultar.Name = "btnConsultar";
            btnConsultar.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnConsultar.Size = new Size(180, 45);
            btnConsultar.TabIndex = 0;
            btnConsultar.Text = "Consultar NF-e";
            btnConsultar.Click += btnConsultar_Click;
            // 
            // panelConteudo
            // 
            panelConteudo.BackColor = Color.Black;
            panelConteudo.Controls.Add(label1);
            panelConteudo.Controls.Add(guna2HtmlLabel1);
            panelConteudo.CustomizableEdges = customizableEdges7;
            panelConteudo.Dock = DockStyle.Fill;
            panelConteudo.Location = new Point(226, 0);
            panelConteudo.Name = "panelConteudo";
            panelConteudo.ShadowDecoration.CustomizableEdges = customizableEdges8;
            panelConteudo.Size = new Size(757, 560);
            panelConteudo.TabIndex = 1;
            panelConteudo.Paint += panelConteudo_Paint;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.DarkGray;
            label1.Location = new Point(80, 208);
            label1.Name = "label1";
            label1.Size = new Size(183, 50);
            label1.TabIndex = 3;
            label1.Text = "Cltr + C para iniciar \r\numa nova consulta";
            // 
            // guna2HtmlLabel1
            // 
            guna2HtmlLabel1.BackColor = Color.Transparent;
            guna2HtmlLabel1.Font = new Font("Segoe UI", 21.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            guna2HtmlLabel1.ForeColor = Color.DarkCyan;
            guna2HtmlLabel1.Location = new Point(80, 163);
            guna2HtmlLabel1.Name = "guna2HtmlLabel1";
            guna2HtmlLabel1.Size = new Size(230, 42);
            guna2HtmlLabel1.TabIndex = 0;
            guna2HtmlLabel1.Text = "Vamos começar!";
            // 
            // FormPrincipal
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(983, 560);
            Controls.Add(panelConteudo);
            Controls.Add(guna2Panel1);
            Name = "FormPrincipal";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "FormPrincipal";
            guna2Panel1.ResumeLayout(false);
            panelConteudo.ResumeLayout(false);
            panelConteudo.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private Guna.UI2.WinForms.Guna2Panel panelConteudo;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel1;
        private Guna.UI2.WinForms.Guna2Button btnConsultar;
        private Guna.UI2.WinForms.Guna2Button btncertificados;
        private Label label1;
    }
}