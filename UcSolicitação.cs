using System.ComponentModel;

namespace Gerenciador_de_NF_e
{
    partial class UcSolicitação
    {
        /// <summary> 
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Designer de Componentes

        /// <summary> 
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            label1 = new Label();
            cmbClientes = new ComboBox();
            btnSolicitar = new Guna.UI2.WinForms.Guna2Button();
            pnlLoading = new Guna.UI2.WinForms.Guna2Panel();
            label2 = new Label();
            progressBar = new ProgressBar();
            pnlLoading.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Black;
            label1.ForeColor = Color.Teal;
            label1.Location = new Point(56, 48);
            label1.Name = "label1";
            label1.Size = new Size(430, 64);
            label1.TabIndex = 0;
            label1.Text = "Selecione qual o cliente você deseja \r\nfazer a solicitação de forma manual";
            // 
            // cmbClientes
            // 
            cmbClientes.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbClientes.FormattingEnabled = true;
            cmbClientes.Location = new Point(76, 149);
            cmbClientes.Name = "cmbClientes";
            cmbClientes.Size = new Size(941, 40);
            cmbClientes.TabIndex = 1;
            cmbClientes.SelectedIndexChanged += cmbClientes_SelectedIndexChanged;
            // 
            // btnSolicitar
            // 
            btnSolicitar.BorderRadius = 10;
            btnSolicitar.CustomizableEdges = customizableEdges1;
            btnSolicitar.DisabledState.BorderColor = Color.DarkGray;
            btnSolicitar.DisabledState.CustomBorderColor = Color.DarkGray;
            btnSolicitar.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnSolicitar.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnSolicitar.FillColor = Color.Teal;
            btnSolicitar.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSolicitar.ForeColor = Color.White;
            btnSolicitar.Location = new Point(94, 576);
            btnSolicitar.Name = "btnSolicitar";
            btnSolicitar.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnSolicitar.Size = new Size(205, 68);
            btnSolicitar.TabIndex = 2;
            btnSolicitar.Text = "Solicitar";
            btnSolicitar.Click += btnSolicitar_Click;
            // 
            // pnlLoading
            // 
            pnlLoading.Controls.Add(label2);
            pnlLoading.Controls.Add(progressBar);
            pnlLoading.CustomizableEdges = customizableEdges3;
            pnlLoading.Dock = DockStyle.Fill;
            pnlLoading.Location = new Point(0, 0);
            pnlLoading.Name = "pnlLoading";
            pnlLoading.ShadowDecoration.CustomizableEdges = customizableEdges4;
            pnlLoading.Size = new Size(1109, 755);
            pnlLoading.TabIndex = 3;
            pnlLoading.Visible = false;
            pnlLoading.Paint += pnlLoading_Paint;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = Color.White;
            label2.Location = new Point(316, 192);
            label2.Name = "label2";
            label2.Size = new Size(481, 32);
            label2.TabIndex = 1;
            label2.Text = "Fazendo a solicitação a SEFAZ, aguarde...";
            // 
            // progressBar
            // 
            progressBar.Location = new Point(237, 259);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(636, 33);
            progressBar.TabIndex = 0;
            // 
            // UcSolicitação
            // 
            AutoScaleDimensions = new SizeF(14F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            Controls.Add(pnlLoading);
            Controls.Add(btnSolicitar);
            Controls.Add(cmbClientes);
            Controls.Add(label1);
            Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Margin = new Padding(6);
            Name = "UcSolicitação";
            Size = new Size(1109, 755);
            Load += UcSolicitação_Load;
            pnlLoading.ResumeLayout(false);
            pnlLoading.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private ComboBox cmbClientes;
        private Guna.UI2.WinForms.Guna2Button btnSolicitar;
        private Guna.UI2.WinForms.Guna2Panel pnlLoading;
        private Label label2;
        private ProgressBar progressBar;
    }
}