using FirebirdSql.Data.FirebirdClient;
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
        private string _cnpjOriginal;
        private bool mostrar;
        private string caminhoPfx;
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
            pnlEdicao.Visible = false;
        }

        private void cmbSelecionador_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                if (!(cmbSelecionador.SelectedItem is Cliente selecionado)) return;

                _clienteAtual = _repositorio.BuscarPorCnpj(selecionado.Cnpj);

                if (_clienteAtual == null)
                {
                    pnlEdicao.Visible = false;
                    MessageBox.Show("O cliente selecionado não possui credenciais cadastrados em nosso sistema.");
                    return;
                }

                _cnpjOriginal = _clienteAtual.Cnpj;
                MostrarPnlEdicao(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro ao carregar o cliente");
            }
        }


        private void MostrarPnlEdicao(bool mostrar)
        {
            pnlEdicao.Visible = mostrar;
            if (mostrar) pnlEdicao.BringToFront();

            txtNome.Text = _clienteAtual.Nome;
            txtCnpj.Text = _clienteAtual.Cnpj;
            txtUf.Text = _clienteAtual.UfCodigo;
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            string nomeEditado = txtNome.Text;
            string cnpjEditado = txtCnpj.Text;
            string ufEditado = txtUf.Text;
            byte[] certificadoEditado = File.ReadAllBytes(txtCertificado.Text);
            string senhaEditada = txtSenhaCertificado.Text;

            using (FbConnection conn = Conexao.AbrirConexao())
            {
                try
                {
                    string sql = "UPDATE CLIENTES_CREDENCIAIS SET NOME_CLIENTE = @nome, CNPJ_CLIENTE = @cnpj, UF_CLIENTE = @uf, CERTIFICADO_CLIENTE = @certificado, SENHA_CERTIFICADO = @senha WHERE CNPJ_CLIENTE = @cnpjAntigo";
                    using (FbCommand cmd = new FbCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@nome", nomeEditado);
                        cmd.Parameters.AddWithValue("@cnpj", cnpjEditado);
                        cmd.Parameters.AddWithValue("@uf", ufEditado);
                        cmd.Parameters.AddWithValue("@certificado", certificadoEditado);
                        cmd.Parameters.AddWithValue("@senha", senhaEditada);
                        cmd.Parameters.AddWithValue("@cnpjAntigo", _clienteAtual.Cnpj);
                        cmd.ExecuteNonQuery();
                        CarregarClientes();
                    }
                    MessageBox.Show("Credenciais editadas com suceeso!");

                }
                
                catch (Exception ex)
                {
                    MessageBox.Show("Erro: " + ex.Message);
                }
            }
        }

        private void btnSelectCertificado_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Certificado Digital(*.pfx;*.p12)|*.pfx;*.p12";
            openFileDialog1.Title = "Selecione o Certificado Digital";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                caminhoPfx = openFileDialog1.FileName;
                txtCertificado.Text = caminhoPfx;
            }
        }


        private void btnViewSenha_Click(object sender, EventArgs e)
        {
            if (txtSenhaCertificado.PasswordChar == '●')
            {
                txtSenhaCertificado.PasswordChar = '\0';
                btnViewSenha.Image = Properties.Resources.eye_open;
            }
            else
            {
                txtSenhaCertificado.PasswordChar = '●';
                btnViewSenha.Image = Properties.Resources.eye_hidden;
        }

        }


        private void pnlEdicao_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnDeletar_Click(object sender, EventArgs e)
        {
            DialogResult resposta = MessageBox.Show("Deseja realmente apagar este usuário?",
                "Confirmar",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2
                );

            if (resposta != DialogResult.Yes) return;

            try
            {
                _repositorio.Excluir(_clienteAtual.Cnpj);
                MessageBox.Show("Cliente excluído com sucesso!");
                pnlEdicao.Visible = false;
                CarregarClientes();
            }

            catch (Exception ex)
        private void cmbSelecionador_SelectedIndexChanged(object sender, EventArgs e)
        {
                MessageBox.Show("Erro ao excluir: " + ex.Message);
            }

        }

        
    }
}