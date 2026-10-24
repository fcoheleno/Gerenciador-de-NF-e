using System.ComponentModel;

namespace Gerenciador_de_NF_e
{
    public partial class UcSolicitação : UserControl
    {
        // false = homologação; true = produção
        private const bool Producao = false;

        private readonly ClienteListagem _repositorio = new ClienteListagem(Conexao.ConnectionString);
        private readonly SolicitacaoRepositorio _solicitacoes = new SolicitacaoRepositorio();
        private readonly SefazServico _sefazServico = new SefazServico(Producao);
        private Cliente? _clienteAtual;

        public UcSolicitação()
        {
            InitializeComponent();
        }

        private void UcSolicitação_Load(object sender, EventArgs e)
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;
            CarregarClientes();
        }

        private void CarregarClientes()
        {
            cmbClientes.DisplayMember = "Nome";
            cmbClientes.ValueMember = "Cnpj";
            cmbClientes.DataSource = _repositorio.Listar();
            cmbClientes.SelectedIndex = -1;

            btnSolicitar.Enabled = false;
        }

        private void cmbClientes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbClientes.SelectedItem is not Cliente selecionado) return;

            _clienteAtual = _repositorio.BuscarPorCnpj(selecionado.Cnpj);

            if (_clienteAtual == null)
            {
                MessageBox.Show("Este cliente não possui credenciais cadastradas no nosso sistema");
                btnSolicitar.Enabled = false;
                return;
            }

            btnSolicitar.Enabled = true;
        }

        private async void btnSolicitar_Click(object sender, EventArgs e)
        {
            if (_clienteAtual == null)
            {
                MessageBox.Show("Selecione um cliente.");
                return;
            }

            // 1) Lê o histórico: regra da espera de 1 hora e último NSU
            UltimaSolicitacao? ultima;
            try
            {
                ultima = _solicitacoes.BuscarUltima(_clienteAtual.Cnpj);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao ler o histórico de solicitações: " + ex.Message);
                return;
            }

            if (ultima?.LiberadaEm is DateTime liberada && DateTime.Now < liberada)
            {
                MessageBox.Show(
                    $"Não é necessário solicitar agora. Nova consulta liberada às {liberada:HH:mm}.",
                    "Aguarde");
                return;
            }

            _clienteAtual.UltNsu = ultima?.Nsu ?? "000000000000000";

            // 2) Consulta a Sefaz
            MostrarLoading(true);

            try
            {
                RespostaSefaz resp = await _sefazServico.SolicitarAsync(_clienteAtual);

                // 3) Grava no histórico (mantém o NSU anterior se a Sefaz devolveu erro)
                bool sucesso = resp.CStat is "137" or "138";
                string nsuGravar = sucesso && !string.IsNullOrWhiteSpace(resp.UltNsu)
                    ? resp.UltNsu
                    : _clienteAtual.UltNsu;

                string resultado = $"{resp.CStat} - {resp.Motivo}";
                if (resp.CStat == "138" && resp.AguardarUmaHora)
                    resultado += " [aguardar 1h]";
                if (resultado.Length > 100)
                    resultado = resultado[..100];

                string aviso = "";
                try
                {
                    _solicitacoes.Registrar(_clienteAtual.Cnpj, resultado, nsuGravar);
                }
                catch (Exception ex)
                {
                    aviso = "\n\nAtenção: a consulta foi feita, mas não foi possível " +
                            "gravar o histórico: " + ex.Message;
                }

                // 4) Mensagem para o usuário
                string texto = $"cStat: {resp.CStat}\n{resp.Motivo}";
                if (resp.CStat == "138")
                    texto += $"\n\nNotas nesta consulta: {resp.Chaves.Count}";

                if (resp.AguardarUmaHora)
                    texto += "\n\nNão é necessário solicitar novamente durante 1 hora.";
                else if (resp.CStat == "138")
                    texto += "\n\nHá mais documentos: solicite novamente para continuar.";

                MessageBox.Show(texto + aviso, "Resposta da SEFAZ");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro na solicitação");
            }
            finally
            {
                MostrarLoading(false);
            }
        }

        private void MostrarLoading(bool mostrar)
        {
            pnlLoading.Visible = mostrar;
            if (mostrar) pnlLoading.BringToFront();

            btnSolicitar.Enabled = !mostrar;
            cmbClientes.Enabled = !mostrar;
        }

        private void pnlLoading_Paint(object sender, PaintEventArgs e)
        {
        }
    }
}