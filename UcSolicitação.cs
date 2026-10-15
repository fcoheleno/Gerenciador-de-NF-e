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
            cmbClientes.ValueMember = "Cnpj";
            cmbClientes.DataSource = clientes;
            

            //btnSolicitar.Enabled = false;
        }

        public Cliente BuscarPorCnpj(string cnpj) 
        {
            using (var conn = new FbConnection(Conexao.ConnectionString))
            {
                conn.Open();
                string sql = @"SELECT CERTIFICADO_CLIENTE, SENHA_CERTIFICADO, UF_CLIENTE FROM CLIENTES_CREDENCIAIS WHERE CNPJ_CLIENTE = @cnpj 
            }
        }

    }
}
