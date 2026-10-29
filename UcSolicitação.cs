using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Text;
using System.Windows.Forms;

namespace Gerenciador_de_NF_e
{
    public partial class UcSolicitação : UserControl
    {
        //alteração do matheus pro node.js
        private readonly System.Windows.Forms.Timer _timerLimitador =
            new System.Windows.Forms.Timer();

        private DateTime _liberacaoLimitador;
        private Label _lblTempoLimitador;
        private int _consultaStatusId = 0;
        private bool _limiteVerificado = false;
        //fim da alteração :)

        private readonly ClienteListagem _repositorio = new ClienteListagem(Conexao.ConnectionString);
        private readonly SefazServico _sefazServico = new SefazServico();
        private Cliente _clienteAtual;

        public UcSolicitação()
        {
            InitializeComponent();

            //outra alteração do matheus :o
            _lblTempoLimitador = new Label
            {
                AutoSize = true,
                Text = "Limite de consultas disponível",
                Location = new Point(330, 595),
                ForeColor = Color.DarkOrange,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            Controls.Add(_lblTempoLimitador);

            _timerLimitador.Interval = 1000;
            _timerLimitador.Tick += TimerLimitador_Tick;
            //fim da alteração vei
        }

        private void UcSolicitação_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;
            CarregarClientes();
        }

        private void CarregarClientes()
        {
            cmbClientes.DisplayMember = "Nome";
            cmbClientes.ValueMember = "Cnpj";
            cmbClientes.DataSource = _repositorio.Listar();
            cmbClientes.SelectedIndex = -1;   // começa sem ninguém selecionado

            btnSolicitar.Enabled = false;
        }


        private async void cmbClientes_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Invalida consultas de status anteriores.
            int consultaAtual = ++_consultaStatusId;

            _limiteVerificado = false;

            _timerLimitador.Stop();
            btnSolicitar.Enabled = false;

            if (!(cmbClientes.SelectedItem is Cliente selecionado))
            {
                _clienteAtual = null;
                _lblTempoLimitador.Text = "Selecione um cliente";
                _lblTempoLimitador.ForeColor = Color.DarkOrange;
                return;
            }

            _clienteAtual = _repositorio.BuscarPorCnpj(selecionado.Cnpj);

            _clienteAtual = _repositorio.BuscarPorCnpj(selecionado.Cnpj);

            if (_clienteAtual == null ||
                string.IsNullOrWhiteSpace(_clienteAtual.Cnpj))
            {
                _clienteAtual = null;
                _limiteVerificado = false;
                btnSolicitar.Enabled = false;

                _lblTempoLimitador.Text =
                    "Cliente sem CNPJ cadastrado";

                _lblTempoLimitador.ForeColor = Color.Red;

                MessageBox.Show(
                    "Não foi possível obter o CNPJ deste cliente. " +
                    "Verifique o cadastro no banco de dados.",
                    "CNPJ inválido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            _clienteAtual.UltNsu = "000000000000000"; // provisório, manter como estava

            string cnpjSelecionado = _clienteAtual.Cnpj;

            _lblTempoLimitador.Text = "Verificando limite de consultas...";
            _lblTempoLimitador.ForeColor = Color.DarkOrange;

            try
            {
                ResultadoLimitador limite =
                    await _servicoLimitador.ConsultarStatusAsync(cnpjSelecionado);

                // Ignora a resposta se o usuário selecionou outro cliente.
                if (consultaAtual != _consultaStatusId ||
                    _clienteAtual == null ||
                    _clienteAtual.Cnpj != cnpjSelecionado)
                {
                    return;
                }

                if (!limite.Autorizada)
                {
                    _liberacaoLimitador = DateTime.UtcNow.AddSeconds(
                        Math.Max(0, limite.SegundosRestantes));

                    _lblTempoLimitador.Text =
                        $"Próxima consulta disponível em: {limite.TempoRestante}";

                    _lblTempoLimitador.ForeColor = Color.DarkOrange;
                    btnSolicitar.Enabled = false;

                    if (limite.SegundosRestantes > 0)
                        _timerLimitador.Start();

                    return;
                }

                _lblTempoLimitador.Text =
                    $"Limite disponível — {Math.Max(0, 20 - limite.Total)} tentativas restantes";

                _lblTempoLimitador.ForeColor = Color.DarkGreen;
                _limiteVerificado = true;
                btnSolicitar.Enabled = true;
            }
            catch (Exception ex)
            {
                if (consultaAtual != _consultaStatusId)
                    return;

                btnSolicitar.Enabled = false;
                _lblTempoLimitador.Text = "Não foi possível verificar o limite";
                _lblTempoLimitador.ForeColor = Color.Red;

                MessageBox.Show(
                    "A consulta à SEFAZ ficará bloqueada porque não foi possível " +
                    "confirmar o limite de consultas.\n\nDetalhes: " + ex.Message,
                    "Erro ao verificar limite",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }



        private readonly ServicoLimitador _servicoLimitador = new ServicoLimitador();

        private async void btnSolicitar_Click(object sender, EventArgs e)
        {

            var certTeste = new System.Security.Cryptography.X509Certificates.X509Certificate2(
                _clienteAtual.CertificadoBytes,
                _clienteAtual.Senha_Certificado);

            if (_clienteAtual == null)
            {
                MessageBox.Show("Selecione um cliente.");
                return;
            }

            MostrarLoading(true);

            try
            {
                // 1. Verificar o limite antes de chamar a SEFAZ.
                ResultadoLimitador limite =
                    await _servicoLimitador.AutorizarAsync(_clienteAtual.Cnpj);

                // 2. Se o limitador negar, não chamar a SEFAZ. modificado pro node.js do matheus tbm viu
                if (!limite.Autorizada)
                {
                    _liberacaoLimitador = DateTime.UtcNow.AddSeconds(
                        Math.Max(0, limite.SegundosRestantes));

                    _lblTempoLimitador.Text =
                        $"Próxima consulta disponível em: {limite.TempoRestante}";

                    _lblTempoLimitador.ForeColor = Color.DarkOrange;
                    btnSolicitar.Enabled = false;

                    if (limite.SegundosRestantes > 0)
                        _timerLimitador.Start();

                    MessageBox.Show(
                        limite.Mensagem ??
                        $"Limite de consultas atingido. Tempo restante: {limite.TempoRestante}",
                        "Consulta bloqueada",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }
                //fim da alter ;D

                // 3. Só chega aqui se a API autorizar a tentativa.

                RespostaSefaz resp =
                    await _sefazServico.SolicitarAsync(_clienteAtual);

                if (resp.CStat == "656")
                {
                    // Bloqueia imediatamente a interface, por segurança.
                    _limiteVerificado = false;
                    btnSolicitar.Enabled = false;

                    _liberacaoLimitador = DateTime.UtcNow.AddHours(1);
                    _lblTempoLimitador.Text =
                        "Consumo Indevido — bloqueado por 01:00:00";

                    _lblTempoLimitador.ForeColor = Color.Red;
                    _timerLimitador.Start();

                    try
                    {
                        // Registra o bloqueio persistente no Firebird pela API.
                        await _servicoLimitador.RegistrarBloqueioConsumoAsync(
                            _clienteAtual.Cnpj);

                        MessageBox.Show(
                            "A SEFAZ retornou cStat 656 (Consumo Indevido).\n\n" +
                            "O CNPJ foi bloqueado por uma hora. O bloqueio foi " +
                            "registrado no banco de dados e continuará ativo " +
                            "mesmo após fechar o programa.",
                            "Consumo Indevido",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                    catch (Exception erro)
                    {
                        // Mantém o bloqueio local se não conseguir gravá-lo.
                        // Não permite novas consultas nesta execução.
                        MessageBox.Show(
                            "A SEFAZ retornou cStat 656, mas não foi possível " +
                            "confirmar o registro do bloqueio no banco.\n\n" +
                            "O botão permanecerá bloqueado nesta execução. " +
                            "Não reinicie o programa até resolver o problema.\n\n" +
                            "Detalhes: " + erro.Message,
                            "Falha ao registrar bloqueio",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }

                    return;
                }

                MessageBox.Show(
                    $"cStat: {resp.CStat}\n{resp.Motivo}",
                    "Resposta da SEFAZ");

            }
            catch (Exception ex)
            {
                // Falha fechada: erro na API ou no banco impede a consulta.
                MessageBox.Show(
                    "A consulta à SEFAZ não foi realizada porque não foi possível " +
                    "confirmar a autorização do limitador.\n\nDetalhes: " + ex.Message,
                    "Erro na solicitação",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                MostrarLoading(false);
            }
        }

        //outra alteração pro node.js do matheus

        private async void TimerLimitador_Tick(object? sender, EventArgs e)
        {
            TimeSpan restante = _liberacaoLimitador - DateTime.UtcNow;

            if (restante > TimeSpan.Zero)
            {
                _lblTempoLimitador.Text =
                    "Próxima consulta disponível em: " +
                    $"{(int)restante.TotalHours:00}:" +
                    $"{restante.Minutes:00}:" +
                    $"{restante.Seconds:00}";

                _lblTempoLimitador.ForeColor = Color.DarkOrange;
                btnSolicitar.Enabled = false;
                return;
            }

            _timerLimitador.Stop();

            // Não libera o botão apenas porque o relógio chegou a zero.
            // Confirma novamente o limite no banco.
            btnSolicitar.Enabled = false;
            _lblTempoLimitador.Text = "Verificando limite de consultas...";
            _lblTempoLimitador.ForeColor = Color.DarkOrange;

            if (_clienteAtual == null)
                return;

            int consultaAtual = ++_consultaStatusId;
            string cnpjSelecionado = _clienteAtual.Cnpj;

            try
            {
                ResultadoLimitador limite =
                    await _servicoLimitador.ConsultarStatusAsync(cnpjSelecionado);

                if (consultaAtual != _consultaStatusId ||
                    _clienteAtual == null ||
                    _clienteAtual.Cnpj != cnpjSelecionado)
                {
                    return;
                }

                if (!limite.Autorizada)
                {
                    _liberacaoLimitador = DateTime.UtcNow.AddSeconds(
                        Math.Max(1, limite.SegundosRestantes));

                    _lblTempoLimitador.Text =
                        $"Próxima consulta disponível em: {limite.TempoRestante}";

                    _lblTempoLimitador.ForeColor = Color.DarkOrange;
                    _timerLimitador.Start();
                    return;
                }

                _lblTempoLimitador.Text =
                    $"Limite disponível — {Math.Max(0, 20 - limite.Total)} tentativas restantes";

                _lblTempoLimitador.ForeColor = Color.DarkGreen;
                _limiteVerificado = true;
                btnSolicitar.Enabled = true;
            }
            catch (Exception ex)
            {
                if (consultaAtual != _consultaStatusId)
                    return;

                btnSolicitar.Enabled = false;
                _lblTempoLimitador.Text = "Não foi possível verificar o limite";
                _lblTempoLimitador.ForeColor = Color.Red;

                MessageBox.Show(
                    "O botão Solicitar continuará bloqueado porque não foi possível " +
                    "confirmar o limite de consultas.\n\nDetalhes: " + ex.Message,
                    "Erro ao verificar limite",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        //fim


        private void MostrarLoading(bool mostrar)
        {
            pnlLoading.Visible = mostrar;

            if (mostrar)
                pnlLoading.BringToFront();

            cmbClientes.Enabled = !mostrar;

            btnSolicitar.Enabled =
                !mostrar &&
                _clienteAtual != null &&
                _limiteVerificado &&
                !_timerLimitador.Enabled;
        }

        private void pnlLoading_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}