using System.CodeDom.Compiler;
using System.ComponentModel;

namespace Gerenciador_de_NF_e
{
    public partial class UcSolicitação : UserControl
    {
        // false = homologação; true = produção
        private const bool Producao = false;

        private readonly ClienteListagem _repositorio = new ClienteListagem(Conexao.ConnectionString);
        private readonly SolicitacaoExecutor _executor = SolicitacaoExecutor.Instancia;
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

            MostrarLoading(true);

            try
            {
                var res = await _executor.ExecutarAsync(_clienteAtual);

                if (res.Pulada)
                {
                    MessageBox.Show(
                        $"Não é necessário solicitar agora. Nova consulta liberada às {res.LiberadaEm:HH:mm}.",
                        "Aguarde");
                    return;
                }

                var resp = res.Resposta!;

                string texto = $"cStat: {resp.CStat}\n{resp.Motivo}";
                if (resp.CStat == "138")
                    texto += $"\n\nNotas nesta consulta: {resp.Chaves.Count}";

                if (resp.AguardarUmaHora)
                    texto += "\n\nNão é necessário solicitar novamente durante 1 hora.";
                else if (resp.CStat == "138")
                    texto += "\n\nHá mais documentos: solicite novamente para continuar.";

                MessageBox.Show(texto + res.AvisoGravacao, "Resposta da SEFAZ");
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