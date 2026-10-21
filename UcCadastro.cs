using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Gerenciador_de_NF_e
{
    public partial class UcCadastro : UserControl
    {
        public UcCadastro()
        {
            InitializeComponent();
        }

        private string caminhoPfx;
        bool situacao;

        private void FormPrincipal_Load(object sender, EventArgs e)
        {
            txtSenhaCert.UseSystemPasswordChar = false;
            txtSenhaCert.PasswordChar = '●';
            btnViewSenha.Image = Properties.Resources.eye_hidden;
        }

        private void btnSelectCertificado_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Certificado Digital(*.pfx;*.p12)|*.pfx;*.p12";
            openFileDialog1.Title = "Selecione o Certificado Digital";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                caminhoPfx = openFileDialog1.FileName;
                txtCaminho.Text = caminhoPfx;
            }
        }

        private void btnViewSenha_Click(object sender, EventArgs e)
        {
            if (txtSenhaCert.PasswordChar == '●')
            {
                txtSenhaCert.PasswordChar = '\0';
                btnViewSenha.Image = Properties.Resources.eye_open;
            }
            else
            {
                txtSenhaCert.PasswordChar = '●';
                btnViewSenha.Image = Properties.Resources.eye_hidden;
            }

        }

        private void txtClienteNome_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnEnviar_Click(object sender, EventArgs e)
        {
             situacao = ArmazenarBd();
        }



        //funções que vão ser utilizadas nop decorrer do codigo



        private bool ArmazenarBd() 
        {
            string nomeCliente = txtClienteNome.Text;
            string cnpjCliente = txtCnpj.Text;
            string senhaCertificado = txtSenhaCert.Text;
            byte[] certificadoCaminho = File.ReadAllBytes(txtCaminho.Text);
            string ufCliente = txtUf.Text;

            using (FbConnection conn = Conexao.AbrirConexao())
            {
                try
                {
                    string query = "INSERT INTO CLIENTES_CREDENCIAIS(CNPJ_CLIENTE, NOME_CLIENTE, UF_CLIENTE, CERTIFICADO_CLIENTE, SENHA_CERTIFICADO) VALUES(@CnpjCliente, @NomeCliente, @UfCliente,@CertificadoCaminho, @SenhaCertificado)";
                    using (FbCommand cmd = new FbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CnpjCliente", cnpjCliente);
                        cmd.Parameters.AddWithValue("@NomeCliente", nomeCliente);
                        cmd.Parameters.AddWithValue("@UfCliente", ufCliente);
                        cmd.Parameters.AddWithValue("@CertificadoCaminho", certificadoCaminho);
                        cmd.Parameters.AddWithValue("@SenhaCertificado", senhaCertificado);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Dados foram guardados no Banco");
                    return true;
                }

                catch (Exception ex)
                {
                    MessageBox.Show("Erro:" + ex.Message);
                    return false;
                }
            }
        }


        
        
    }
}
