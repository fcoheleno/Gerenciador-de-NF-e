using System.Collections.Generic;
using System.Globalization;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Gerenciador_de_NF_e
{
    public partial class UcPainel : UserControl
    {
        private readonly PainelRepositorio _repo = new PainelRepositorio();
        private bool _carregando;

        public UcPainel()
        {
            InitializeComponent();

            ConfigurarGrade();
            ConfigurarFiltros();   // <- esta linha precisa existir

            Load += UcPainel_Load;
            btnAtualizar.Click += BtnAtualizar_Click;
            timerAtualiza.Tick += TimerAtualiza_Tick;
            dgvClientes.CellFormatting += DgvClientes_CellFormatting;
            Disposed += (_, _) =>
    SolicitacaoAutomatica.Instancia.RodadaConcluida -= Auto_RodadaConcluida;

        }
        private void ConfigurarFiltros()
        {
            var painel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = true,
                Padding = new Padding(6, 6, 6, 0),
                BackColor = Color.FromArgb(20, 25, 28)
            };

            txtNome = NovoTexto(painel, "Cliente", 200);
            txtCnpj = NovoTexto(painel, "CNPJ", 150);
            cboUf = NovoCombo(painel, "UF", 70);
            cboSituacao = NovoCombo(painel, "Situação", 160);

            btnLimpar = new Button
            {
                Text = "Limpar",
                AutoSize = true,
                Margin = new Padding(6, 18, 3, 3),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(0, 105, 105)
            };
            btnLimpar.FlatAppearance.BorderSize = 0;
            btnLimpar.Click += (_, _) => LimparFiltros();
            painel.Controls.Add(btnLimpar);

            var pai = dgvClientes.Parent!;

            painel.Dock = DockStyle.None;
            painel.AutoSize = false;
            painel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            painel.BackColor = Color.Transparent;
            painel.Width = dgvClientes.Width;

            int altura = painel.GetPreferredSize(new Size(painel.Width, 0)).Height;
            painel.Height = altura;
            painel.Location = dgvClientes.Location;   // começa onde a grade começava

            pai.Controls.Add(painel);
            painel.BringToFront();

            // Empurra a grade para baixo, abrindo espaço para a barra.
            dgvClientes.Top += altura;
            dgvClientes.Height -= altura;
        }

        private TextBox NovoTexto(Control pai, string titulo, int largura)
        {
            var txt = new TextBox { Width = largura };
            txt.TextChanged += (_, _) => AplicarFiltro();
            pai.Controls.Add(Agrupar(titulo, txt));
            return txt;
        }

        private ComboBox NovoCombo(Control pai, string titulo, int largura)
        {
            var cbo = new ComboBox
            {
                Width = largura,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cbo.SelectedIndexChanged += (_, _) =>
            {
                if (!_montandoFiltro) AplicarFiltro();
            };
            pai.Controls.Add(Agrupar(titulo, cbo));
            return cbo;
        }

        private static Control Agrupar(string titulo, Control campo)
        {
            var lbl = new Label
            {
                Text = titulo,
                AutoSize = true,
                ForeColor = Color.Gainsboro,
                Margin = new Padding(0, 0, 0, 2)
            };
            var box = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                WrapContents = false,
                Margin = new Padding(3, 3, 3, 3)
            };
            box.Controls.Add(lbl);
            box.Controls.Add(campo);
            return box;
        }

        private void LimparFiltros()
        {
            _montandoFiltro = true;
            txtNome.Clear(); txtCnpj.Clear();
            if (cboUf.Items.Count > 0) cboUf.SelectedIndex = 0;
            if (cboSituacao.Items.Count > 0) cboSituacao.SelectedIndex = 0;
            _montandoFiltro = false;
            AplicarFiltro();
        }

        private void PreencherCombo(ComboBox? cbo, IEnumerable<string> valores)
        {
            if (cbo == null) return;

            string? atual = cbo.SelectedItem as string;

            _montandoFiltro = true;
            cbo.Items.Clear();
            cbo.Items.Add("(Todos)");
            foreach (var v in valores.Where(v => !string.IsNullOrWhiteSpace(v))
                                     .Distinct().OrderBy(v => v))
                cbo.Items.Add(v);

            int idx = atual == null ? 0 : cbo.Items.IndexOf(atual);
            cbo.SelectedIndex = idx >= 0 ? idx : 0;
            _montandoFiltro = false;
        }

        // Contém, ignorando maiúsculas/minúsculas e acentos.
        private static bool Contem(string? texto, string filtro)
        {
            filtro = filtro.Trim();
            if (filtro.Length == 0) return true;

            return CultureInfo.CurrentCulture.CompareInfo.IndexOf(
                texto ?? "", filtro,
                CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0;
        }

        private static string Digitos(string? s) =>
            new string((s ?? "").Where(char.IsDigit).ToArray());

        private static bool CombinaCombo(ComboBox cbo, string? valor) =>
            cbo.SelectedIndex <= 0 ||
            string.Equals(cbo.SelectedItem as string, valor,
                          StringComparison.CurrentCultureIgnoreCase);

        private bool PassaNoFiltro(ClienteStatus c)
        {
            if (!Contem(c.Nome, txtNome.Text)) return false;

            // CNPJ: aceita digitar com ou sem máscara.
            string fCnpj = txtCnpj.Text.Trim();
            if (fCnpj.Length > 0)
            {
                string soNum = Digitos(fCnpj);
                bool ok = soNum.Length > 0
                    ? Digitos(c.Cnpj).Contains(soNum)
                    : Contem(c.CnpjFormatado, fCnpj);
                if (!ok) return false;
            }

            if (!CombinaCombo(cboUf, c.Uf)) return false;
            if (!CombinaCombo(cboSituacao, c.SituacaoTexto)) return false;
            return true;
        }

        private void AplicarFiltro()
        {
            if (_todos == null || txtNome == null || cboUf == null) return;
            string? selecionado =
                (dgvClientes.CurrentRow?.DataBoundItem as ClienteStatus)?.Cnpj;

            var lista = _todos.Where(PassaNoFiltro).ToList();

            dgvClientes.DataSource = null;
            dgvClientes.DataSource = lista;

            if (selecionado != null)
            {
                foreach (DataGridViewRow row in dgvClientes.Rows)
                {
                    if ((row.DataBoundItem as ClienteStatus)?.Cnpj == selecionado)
                    {
                        dgvClientes.CurrentCell = row.Cells[0];
                        break;
                    }
                }
            }

            int Qtd(SituacaoCliente s) => lista.Count(c => c.Situacao == s);

            string contagem = lista.Count == _todos.Count
                ? $"{lista.Count} cliente(s)"
                : $"{lista.Count} de {_todos.Count} cliente(s)";

            lblResumo.Text =
                $"{contagem}  •  " +
                $"{Qtd(SituacaoCliente.HaNotas)} com notas  •  " +
                $"{Qtd(SituacaoCliente.AguardandoUmaHora)} aguardando 1 h  •  " +
                $"{Qtd(SituacaoCliente.Erro)} com erro  •  " +
                $"{Qtd(SituacaoCliente.NuncaConsultado)} nunca consultado(s)";
        }
        private void UcPainel_Load(object? sender, EventArgs e)
        {
            if (DesignMode ||
                System.ComponentModel.LicenseManager.UsageMode ==
                System.ComponentModel.LicenseUsageMode.Designtime)
            {
                return;
            }
            Carregar();

            SolicitacaoAutomatica.Instancia.RodadaConcluida += Auto_RodadaConcluida;
            SolicitacaoAutomatica.Instancia.ForcarConferencia();
            timerAtualiza.Interval = 30_000;
            timerAtualiza.Start();
        }

        private void BtnAtualizar_Click(object? sender, EventArgs e)
        {
            Carregar();
            SolicitacaoAutomatica.Instancia.ForcarConferencia();
        }

        private void Auto_RodadaConcluida(object? sender, EventArgs e)
        {
            if (!IsDisposed) Carregar();
        }

        private void TimerAtualiza_Tick(object? sender, EventArgs e)
        {
            Carregar();
        }

        private void ConfigurarGrade()
        {
            var g = dgvClientes;

            g.AutoGenerateColumns = false;
            g.AllowUserToAddRows = false;
            g.AllowUserToDeleteRows = false;
            g.AllowUserToResizeRows = false;
            g.ReadOnly = true;
            g.RowHeadersVisible = false;
            g.MultiSelect = false;

            g.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            g.BackgroundColor = Color.FromArgb(20, 25, 28);
            g.BorderStyle = BorderStyle.None;
            g.GridColor = Color.FromArgb(48, 58, 61);

            g.EnableHeadersVisualStyles = false;

            g.ColumnHeadersHeight = 46;
            g.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            g.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(0, 105, 105);

            g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            g.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 10F, FontStyle.Bold);

            g.ColumnHeadersDefaultCellStyle.Padding =
                new Padding(8, 0, 0, 0);

            g.DefaultCellStyle.BackColor =
                Color.FromArgb(25, 31, 34);

            g.DefaultCellStyle.ForeColor =
                Color.White;

            g.DefaultCellStyle.Font =
                new Font("Segoe UI", 10F);

            g.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(0, 95, 95);

            g.DefaultCellStyle.SelectionForeColor =
                Color.White;

            g.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(32, 39, 42);

            g.RowTemplate.Height = 40;

            g.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            // Evita adicionar as mesmas colunas mais de uma vez.
            g.Columns.Clear();

            Coluna("colNome", "Cliente",
                nameof(ClienteStatus.Nome), 28);

            Coluna("colCnpj", "CNPJ",
                nameof(ClienteStatus.CnpjFormatado), 18);

            Coluna("colUf", "UF",
                nameof(ClienteStatus.Uf), 5);

            Coluna("colSituacao", "Situação",
                nameof(ClienteStatus.SituacaoTexto), 21);

            Coluna("colUltima", "Última consulta",
                nameof(ClienteStatus.UltimaConsultaTexto), 15);

            Coluna("colProxima", "Próxima consulta",
                nameof(ClienteStatus.ProximaConsultaTexto), 13);
        }

        private void Coluna(
            string nome,
            string titulo,
            string propriedade,
            float peso)
        {
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = nome,
                HeaderText = titulo,
                DataPropertyName = propriedade,
                FillWeight = peso,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
        }

        private void Carregar()
        {
            if (_carregando || IsDisposed)
                return;

            _carregando = true;
            btnAtualizar.Enabled = false;

            try
            {
                _todos = _repo.Listar();

                PreencherCombo(cboUf, _todos.Select(c => c.Uf));
                PreencherCombo(cboSituacao, _todos.Select(c => c.SituacaoTexto));

                AplicarFiltro();
            }
            catch (Exception ex)
            {
                lblResumo.Text = "Erro ao carregar os clientes: " + ex.Message;
            }
            finally
            {
                btnAtualizar.Enabled = true;
                _carregando = false;
            }
        }

        private void DgvClientes_CellFormatting(
            object? sender,
            DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (dgvClientes.Columns[e.ColumnIndex].Name != "colSituacao")
                return;

            if (dgvClientes.Rows[e.RowIndex].DataBoundItem
                is not ClienteStatus cliente)
                return;

            Color cor = cliente.Situacao switch
            {
                SituacaoCliente.HaNotas =>
                    Color.SeaGreen,

                SituacaoCliente.AguardandoUmaHora =>
                    Color.DarkOrange,

                SituacaoCliente.SemNotasLiberado =>
                    Color.SteelBlue,

                SituacaoCliente.Erro =>
                    Color.Firebrick,

                _ => Color.DimGray
            };

            e.CellStyle!.BackColor = cor;
            e.CellStyle.SelectionBackColor = cor;
            e.CellStyle.ForeColor = Color.White;
            e.CellStyle.SelectionForeColor = Color.White;
            e.CellStyle.Font =
                new Font("Segoe UI", 10F, FontStyle.Bold);
        }
        private List<ClienteStatus> _todos = new();
        private bool _montandoFiltro;

        private TextBox txtNome = null!, txtCnpj = null!;
        private ComboBox cboUf = null!, cboSituacao = null!;
        private Button btnLimpar = null!;

        private void pnlTopo_Paint(object sender, PaintEventArgs e)
        {

        }
    }

}
