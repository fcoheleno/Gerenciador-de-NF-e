namespace Gerenciador_de_NF_e
{
    public partial class UcPainel : UserControl
    {
        private readonly PainelRepositorio _repo = new PainelRepositorio();

        public UcPainel()
        {
            InitializeComponent();
            ConfigurarGrade();

            Load += UcPainel_Load;
            btnAtualizar.Click += (s, e) => Carregar();
            timerAtualiza.Tick += (s, e) => Carregar();
            dgvClientes.CellFormatting += dgvClientes_CellFormatting;
        }

        private void UcPainel_Load(object? sender, EventArgs e)
        {
            if (System.ComponentModel.LicenseManager.UsageMode ==
                System.ComponentModel.LicenseUsageMode.Designtime) return;

            Carregar();
            timerAtualiza.Interval = 30_000;   // atualiza a cada 30 segundos
            timerAtualiza.Start();
        }

        private void ConfigurarGrade()
        {
            var g = dgvClientes;
            g.AutoGenerateColumns = false;
            g.AllowUserToAddRows = false;
            g.AllowUserToDeleteRows = false;
            g.ReadOnly = true;
            g.RowHeadersVisible = false;
            g.MultiSelect = false;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.BackgroundColor = Color.Black;
            g.BorderStyle = BorderStyle.None;
            g.GridColor = Color.FromArgb(50, 50, 50);
            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            g.ColumnHeadersHeight = 42;
            g.ColumnHeadersDefaultCellStyle.BackColor = Color.Teal;
            g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            g.DefaultCellStyle.BackColor = Color.FromArgb(24, 24, 24);
            g.DefaultCellStyle.ForeColor = Color.White;
            g.DefaultCellStyle.Font = new Font("Segoe UI", 10.5F);
            g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 110, 110);
            g.DefaultCellStyle.SelectionForeColor = Color.White;
            g.RowTemplate.Height = 38;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            Coluna("colNome", "Cliente", nameof(ClienteStatus.Nome), 28);
            Coluna("colCnpj", "CNPJ", nameof(ClienteStatus.CnpjFormatado), 18);
            Coluna("colUf", "UF", nameof(ClienteStatus.Uf), 5);
            Coluna("colSituacao", "Situação", nameof(ClienteStatus.SituacaoTexto), 21);
            Coluna("colUltima", "Última consulta", nameof(ClienteStatus.UltimaConsultaTexto), 15);
            Coluna("colProxima", "Próxima consulta", nameof(ClienteStatus.ProximaConsultaTexto), 13);
        }

        private void Coluna(string nome, string titulo, string propriedade, float peso)
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
            try
            {
                string? selecionado = (dgvClientes.CurrentRow?.DataBoundItem as ClienteStatus)?.Cnpj;

                var lista = _repo.Listar();
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

                lblResumo.Text =
                    $"{lista.Count} cliente(s)  •  {Qtd(SituacaoCliente.HaNotas)} com notas  •  " +
                    $"{Qtd(SituacaoCliente.AguardandoUmaHora)} aguardando 1 h  •  " +
                    $"{Qtd(SituacaoCliente.Erro)} com erro  •  " +
                    $"{Qtd(SituacaoCliente.NuncaConsultado)} nunca consultado(s)";
            }
            catch (Exception ex)
            {
                lblResumo.Text = "Erro ao carregar: " + ex.Message;
            }
        }

        private void dgvClientes_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvClientes.Columns[e.ColumnIndex].Name != "colSituacao") return;
            if (dgvClientes.Rows[e.RowIndex].DataBoundItem is not ClienteStatus c) return;

            Color cor = c.Situacao switch
            {
                SituacaoCliente.HaNotas => Color.SeaGreen,
                SituacaoCliente.AguardandoUmaHora => Color.DarkOrange,
                SituacaoCliente.SemNotasLiberado => Color.SteelBlue,
                SituacaoCliente.Erro => Color.Firebrick,
                _ => Color.DimGray
            };

            e.CellStyle!.BackColor = cor;
            e.CellStyle.SelectionBackColor = cor;
            e.CellStyle.ForeColor = Color.White;
            e.CellStyle.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        }
    }
}