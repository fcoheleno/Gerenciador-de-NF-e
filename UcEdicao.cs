using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Gerenciador_de_NF_e
{
    public partial class UcEdicao : UserControl
    {

        private readonly ClienteListagem _repositorio = new ClienteListagem(Conexao.ConnectionString);
        private Cliente _clienteAtual;
        public UcEdicao()
        {
            InitializeComponent();
        }

        private void UcEdicao_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;
            CarregarClientes();
        }



        private void CarregarClientes()
        {
            cmbSelecionador.DisplayMember = "Nome";
            cmbSelecionador.ValueMember = "Cnpj";
            cmbSelecionador.DataSource = _repositorio.Listar();
            cmbSelecionador.SelectedIndex = -1;
        }

        private void cmbSelecionador_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
