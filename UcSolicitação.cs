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
        private readonly ClienteListagem _repositorio = new ClienteListagem(Conexao.ConnectionString);
        private readonly SefazServico _sefazServico = new SefazServico();
        private Cliente _clienteAtual;

        public UcSolicitação()
        {
            InitializeComponent();
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

        private void cmbClientes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(cmbClientes.SelectedItem is Cliente selecionado)) return;

            _clienteAtual = _repositorio.BuscarPorCnpj(selecionado.Cnpj);

            if (_clienteAtual == null)
            {
                MessageBox.Show("Este cliente não possui credenciais cadastradas no nosso sistema");
                btnSolicitar.Enabled = false;
                return;
            }

            _clienteAtual.UltNsu = "000000000000000"; // provisório, fazer alteração aqui depois
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
                RespostaSefaz resp = await _sefazServico.SolicitarAsync(_clienteAtual);

                MessageBox.Show($"cStat: {resp.CStat}\n{resp.Motivo}", "Resposta da SEFAZ");
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
