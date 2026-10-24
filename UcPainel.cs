
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

            Load += UcPainel_Load;
            btnAtualizar.Click += BtnAtualizar_Click;
            timerAtualiza.Tick += TimerAtualiza_Tick;
            dgvClientes.CellFormatting += DgvClientes_CellFormatting;
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

            timerAtualiza.Interval = 30_000;
            timerAtualiza.Start();
        }

        private void BtnAtualizar_Click(object? sender, EventArgs e)
        {
            Carregar();
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
                string? selecionado =
                    (dgvClientes.CurrentRow?.DataBoundItem
                    as ClienteStatus)?.Cnpj;

                var lista = _repo.Listar();

                dgvClientes.DataSource = null;
                dgvClientes.DataSource = lista;

                if (selecionado != null)
                {
                    foreach (DataGridViewRow row in dgvClientes.Rows)
                    {
                        if ((row.DataBoundItem as ClienteStatus)?.Cnpj
                            == selecionado)
                        {
                            dgvClientes.CurrentCell = row.Cells[0];
                            break;
                        }
                    }
                }

                int Qtd(SituacaoCliente situacao) =>
                    lista.Count(c => c.Situacao == situacao);

                lblResumo.Text =
                    $"{lista.Count} cliente(s)  •  " +
                    $"{Qtd(SituacaoCliente.HaNotas)} com notas  •  " +
                    $"{Qtd(SituacaoCliente.AguardandoUmaHora)} aguardando 1 h  •  " +
                    $"{Qtd(SituacaoCliente.Erro)} com erro  •  " +
                    $"{Qtd(SituacaoCliente.NuncaConsultado)} nunca consultado(s)";
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
    }
}
