
namespace Gerenciador_de_NF_e
{
    partial class UcPainel
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Código gerado pelo Designer

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
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
            panel1.BackColor = Color.FromArgb(15, 18, 21);
            panel1.Controls.Add(pnlTopo);
            panel1.Controls.Add(dgvClientes);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Margin = new Padding(0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1174, 592);
            panel1.TabIndex = 0;
            // 
            // pnlTopo
            // 
            pnlTopo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlTopo.BorderRadius = 16;
            pnlTopo.Controls.Add(lblTitulo);
            pnlTopo.Controls.Add(lblResumo);
            pnlTopo.Controls.Add(btnAtualizar);
            pnlTopo.CustomizableEdges = customizableEdges3;
            pnlTopo.FillColor = Color.RoyalBlue;
            pnlTopo.FillColor2 = Color.SlateBlue;
            pnlTopo.FillColor3 = Color.DarkBlue;
            pnlTopo.FillColor4 = Color.MidnightBlue;
            pnlTopo.Location = new Point(21, 19);
            pnlTopo.Name = "pnlTopo";
            pnlTopo.ShadowDecoration.BorderRadius = 16;
            pnlTopo.ShadowDecoration.CustomizableEdges = customizableEdges4;
            pnlTopo.Size = new Size(1132, 110);
            pnlTopo.TabIndex = 0;
            pnlTopo.Paint += pnlTopo_Paint;
            // 
            // lblTitulo
            // 
            lblTitulo.BackColor = Color.Transparent;
            lblTitulo.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(24, 14);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(274, 47);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Painel de controle";
            // 
            // lblResumo
            // 
            lblResumo.BackColor = Color.Transparent;
            lblResumo.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblResumo.ForeColor = Color.FromArgb(230, 245, 245);
            lblResumo.Location = new Point(28, 66);
            lblResumo.Name = "lblResumo";
            lblResumo.Size = new Size(236, 19);
            lblResumo.TabIndex = 1;
            lblResumo.Text = "Carregando informações dos clientes...";
            // 
            // btnAtualizar
            // 
            btnAtualizar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAtualizar.BorderRadius = 10;
            btnAtualizar.Cursor = Cursors.Hand;
            btnAtualizar.CustomizableEdges = customizableEdges1;
            btnAtualizar.DisabledState.BorderColor = Color.DarkGray;
            btnAtualizar.DisabledState.CustomBorderColor = Color.DarkGray;
            btnAtualizar.DisabledState.FillColor = Color.FromArgb(80, 90, 90);
            btnAtualizar.DisabledState.ForeColor = Color.FromArgb(180, 180, 180);
            btnAtualizar.FillColor = Color.White;
            btnAtualizar.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnAtualizar.ForeColor = Color.MidnightBlue;
            btnAtualizar.HoverState.FillColor = Color.FromArgb(30, 65, 68);
            btnAtualizar.Location = new Point(962, 35);
            btnAtualizar.Name = "btnAtualizar";
            btnAtualizar.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnAtualizar.Size = new Size(140, 39);
            btnAtualizar.TabIndex = 2;
            btnAtualizar.Text = "↻  Atualizar";
            btnAtualizar.Click += BtnAtualizar_Click;
            // 
            // dgvClientes
            // 
            dgvClientes.AllowUserToAddRows = false;
            dgvClientes.AllowUserToDeleteRows = false;
            dgvClientes.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(32, 39, 42);
            dataGridViewCellStyle1.ForeColor = Color.White;
            dgvClientes.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dgvClientes.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvClientes.BackgroundColor = Color.FromArgb(23, 28, 31);
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(0, 105, 105);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.Padding = new Padding(10, 0, 0, 0);
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dgvClientes.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvClientes.ColumnHeadersHeight = 46;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.FromArgb(25, 31, 34);
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 10F);
            dataGridViewCellStyle3.ForeColor = Color.White;
            dataGridViewCellStyle3.Padding = new Padding(10, 0, 5, 0);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(0, 95, 95);
            dataGridViewCellStyle3.SelectionForeColor = Color.White;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dgvClientes.DefaultCellStyle = dataGridViewCellStyle3;
            dgvClientes.GridColor = Color.FromArgb(48, 58, 61);
            dgvClientes.Location = new Point(21, 149);
            dgvClientes.MultiSelect = false;
            dgvClientes.Name = "dgvClientes";
            dgvClientes.ReadOnly = true;
            dgvClientes.RowHeadersVisible = false;
            dgvClientes.RowTemplate.Height = 40;
            dgvClientes.Size = new Size(1132, 421);
            dgvClientes.TabIndex = 1;
            dgvClientes.ThemeStyle.AlternatingRowsStyle.BackColor = Color.FromArgb(32, 39, 42);
            dgvClientes.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.White;
            dgvClientes.ThemeStyle.BackColor = Color.FromArgb(23, 28, 31);
            dgvClientes.ThemeStyle.GridColor = Color.FromArgb(48, 58, 61);
            dgvClientes.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(0, 105, 105);
            dgvClientes.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvClientes.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvClientes.ThemeStyle.HeaderStyle.Height = 46;
            dgvClientes.ThemeStyle.ReadOnly = true;
            dgvClientes.ThemeStyle.RowsStyle.BackColor = Color.FromArgb(25, 31, 34);
            dgvClientes.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 10F);
            dgvClientes.ThemeStyle.RowsStyle.ForeColor = Color.White;
            dgvClientes.ThemeStyle.RowsStyle.Height = 40;
            dgvClientes.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(0, 95, 95);
            dgvClientes.ThemeStyle.RowsStyle.SelectionForeColor = Color.White;
            // 
            // timerAtualiza
            // 
            timerAtualiza.Interval = 30000;
            // 
            // UcPainel
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(15, 18, 21);
            Controls.Add(panel1);
            Name = "UcPainel";
            Size = new Size(1174, 592);
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
