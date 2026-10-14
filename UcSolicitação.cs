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
        public UcSolicitação()
        {
            InitializeComponent();
        }

        private readonly ClienteListagem _repositorio = new ClienteListagem(Conexao.ConnectionString);

        private void cmbClientes_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void UcSolicitação_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;
            CarregarClientes();
            List<Cliente> cliente = _repositorio.Listar();
        }

        private void CarregarClientes()
        {
            var clientes = _repositorio.Listar();

            cmbClientes.DisplayMember = "Nome";
            cmbClientes.ValueMember = "Id";
            cmbClientes.DataSource = clientes;
            cmbClientes.SelectedIndex = -1;

            //btnSolicitar.Enabled = false;
        }

    }
}
