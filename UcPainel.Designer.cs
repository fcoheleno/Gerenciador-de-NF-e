
namespace Gerenciador_de_NF_e
{
    partial class UcPainel
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();

            base.Dispose(disposing);
        }

        #region Código gerado pelo Designer

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            panel1 = new Panel();
            pnlTopo = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            lblTitulo = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblResumo = new Guna.UI2.WinForms.Guna2HtmlLabel();
            btnAtualizar = new Guna.UI2.WinForms.Guna2Button();
            dgvClientes = new Guna.UI2.WinForms.Guna2DataGridView();
            timerAtualiza = new System.Windows.Forms.Timer(components);

            panel1.SuspendLayout();
            pnlTopo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvClientes).BeginInit();
            SuspendLayout();

            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(15, 18, 20);
            panel1.Dock = DockStyle.Fill;
            panel1.Margin = new Padding(0);
            panel1.Name = "panel1";
            panel1.Controls.Add(dgvClientes);
            panel1.Controls.Add(pnlTopo);
            panel1.TabIndex = 0;

            // 
            // pnlTopo
            // 
            pnlTopo.BackColor = Color.Transparent;
            pnlTopo.BorderRadius = 18;
            pnlTopo.FillColor = Color.Teal;
            pnlTopo.FillColor2 = Color.FromArgb(0, 105, 105);
            pnlTopo.FillColor3 = Color.FromArgb(0, 80, 85);
            pnlTopo.FillColor4 = Color.FromArgb(0, 128, 128);

            pnlTopo.Location = new Point(24, 22);
            pnlTopo.Name = "pnlTopo";
            pnlTopo.Size = new Size(1294, 130);

            pnlTopo.Anchor = AnchorStyles.Top
                | AnchorStyles.Left
                | AnchorStyles.Right;

            pnlTopo.TabIndex = 0;

            pnlTopo.Controls.Add(lblTitulo);
            pnlTopo.Controls.Add(lblResumo);
            pnlTopo.Controls.Add(btnAtualizar);

            // 
            // lblTitulo
            // 
            lblTitulo.BackColor = Color.Transparent;
            lblTitulo.Font = new Font(
                "Segoe UI", 25F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(28, 17);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(390, 48);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Painel de controle";

            // 
            // lblResumo
            // 
            lblResumo.BackColor = Color.Transparent;
            lblResumo.Font = new Font(
                "Segoe UI", 10.5F, FontStyle.Regular);
            lblResumo.ForeColor = Color.FromArgb(225, 245, 245);
            lblResumo.Location = new Point(32, 76);
            lblResumo.Name = "lblResumo";
            lblResumo.Size = new Size(850, 26);
            lblResumo.TabIndex = 1;
            lblResumo.Text = "Carregando informações dos clientes...";

            // 
            // btnAtualizar
            // 
            btnAtualizar.BorderRadius = 12;
            btnAtualizar.FillColor = Color.FromArgb(15, 45, 48);
            btnAtualizar.HoverState.FillColor =
                Color.FromArgb(20, 65, 68);

            btnAtualizar.Font = new Font(
                "Segoe UI", 11F, FontStyle.Bold);
            btnAtualizar.ForeColor = Color.White;
            btnAtualizar.Text = "Atualizar";

            btnAtualizar.Size = new Size(160, 46);
            btnAtualizar.Location = new Point(1100, 40);

            btnAtualizar.Anchor = AnchorStyles.Top
                | AnchorStyles.Right;

            btnAtualizar.Name = "btnAtualizar";
            btnAtualizar.TabIndex = 2;
            btnAtualizar.Cursor = Cursors.Hand;

            // 
            // dgvClientes
            // 
            dgvClientes.AllowUserToAddRows = false;
            dgvClientes.AllowUserToDeleteRows = false;
            dgvClientes.AllowUserToResizeRows = false;
            dgvClientes.AutoGenerateColumns = false;
            dgvClientes.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvClientes.BackgroundColor =
                Color.FromArgb(29, 36, 39);

            dgvClientes.BorderStyle = BorderStyle.None;
            dgvClientes.CellBorderStyle =
                DataGridViewCellBorderStyle.SingleHorizontal;

            dgvClientes.ColumnHeadersBorderStyle =
                DataGridViewHeaderBorderStyle.None;

            dgvClientes.ColumnHeadersHeight = 48;
            dgvClientes.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            dgvClientes.EnableHeadersVisualStyles = false;

            dgvClientes.GridColor =
                Color.FromArgb(48, 58, 61);

            dgvClientes.Location = new Point(24, 175);
            dgvClientes.Name = "dgvClientes";

            dgvClientes.ReadOnly = true;
            dgvClientes.RowHeadersVisible = false;
            dgvClientes.MultiSelect = false;
            dgvClientes.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvClientes.RowTemplate.Height = 42;

            dgvClientes.Size = new Size(1294, 495);

            dgvClientes.Anchor = AnchorStyles.Top
                | AnchorStyles.Bottom
                | AnchorStyles.Left
                | AnchorStyles.Right;

            dgvClientes.TabIndex = 1;

            // Cabeçalho
            dgvClientes.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(0, 105, 105);

            dgvClientes.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;

            dgvClientes.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 10F, FontStyle.Bold);

            dgvClientes.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleLeft;

            dgvClientes.ColumnHeadersDefaultCellStyle.Padding =
                new Padding(10, 0, 0, 0);

            // Células
            dgvClientes.DefaultCellStyle.BackColor =
                Color.FromArgb(29, 36, 39);

            dgvClientes.DefaultCellStyle.ForeColor =
                Color.FromArgb(235, 240, 240);

            dgvClientes.DefaultCellStyle.Font =
                new Font("Segoe UI", 10F);

            dgvClientes.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(0, 95, 95);

            dgvClientes.DefaultCellStyle.SelectionForeColor =
                Color.White;

            dgvClientes.DefaultCellStyle.Padding =
                new Padding(10, 0, 5, 0);

            // Linhas alternadas
            dgvClientes.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(35, 43, 46);

            dgvClientes.AlternatingRowsDefaultCellStyle.ForeColor =
                Color.White;

            // Estilo Guna
            dgvClientes.ThemeStyle.AlternatingRowsStyle.BackColor =
                Color.FromArgb(35, 43, 46);

            dgvClientes.ThemeStyle.AlternatingRowsStyle.ForeColor =
                Color.White;

            dgvClientes.ThemeStyle.RowsStyle.BackColor =
                Color.FromArgb(29, 36, 39);

            dgvClientes.ThemeStyle.RowsStyle.ForeColor =
                Color.White;

            dgvClientes.ThemeStyle.RowsStyle.Height = 42;

            dgvClientes.ThemeStyle.RowsStyle.SelectionBackColor =
                Color.FromArgb(0, 95, 95);

            dgvClientes.ThemeStyle.RowsStyle.SelectionForeColor =
                Color.White;

            dgvClientes.ThemeStyle.HeaderStyle.BackColor =
                Color.FromArgb(0, 105, 105);

            dgvClientes.ThemeStyle.HeaderStyle.ForeColor =
                Color.White;

            dgvClientes.ThemeStyle.HeaderStyle.Font =
                new Font("Segoe UI", 10F, FontStyle.Bold);

            dgvClientes.ThemeStyle.HeaderStyle.Height = 48;

            // 
            // timerAtualiza
            // 
            timerAtualiza.Interval = 30000;

            // 
            // UcPainel
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(15, 18, 20);

            Controls.Add(panel1);

            Name = "UcPainel";
            Size = new Size(1342, 697);

            panel1.ResumeLayout(false);
            pnlTopo.ResumeLayout(false);
            pnlTopo.PerformLayout();

            ((System.ComponentModel.ISupportInitialize)dgvClientes).EndInit();

            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel pnlTopo;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTitulo;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblResumo;
        private Guna.UI2.WinForms.Guna2Button btnAtualizar;
        private Guna.UI2.WinForms.Guna2DataGridView dgvClientes;
        private System.Windows.Forms.Timer timerAtualiza;
    }
}
