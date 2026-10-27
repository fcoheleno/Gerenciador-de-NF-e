using FirebirdSql.Data.FirebirdClient;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace Gerenciador_de_NF_e
{
    public partial class UcCadastro : UserControl
    {
        private string _caminhoPfx = "";

        public UcCadastro()
        {
            InitializeComponent();
        }

        private void btnSelectCertificado_Click(object? sender, EventArgs e)
        {
            openFileDialog1.Filter = "Certificado Digital (*.pfx;*.p12)|*.pfx;*.p12";
            openFileDialog1.Title = "Selecione o Certificado Digital";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                _caminhoPfx = openFileDialog1.FileName;
                txtCaminho.Text = _caminhoPfx;
            }
        }

        private void btnViewSenha_Click(object? sender, EventArgs e)
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

        private void txtClienteNome_TextChanged(object? sender, EventArgs e)
        {
        }

        private void btnEnviar_Click(object? sender, EventArgs e)
        {
            string nome = txtClienteNome.Text.Trim();
            string cnpj = new string(txtCnpj.Text.Where(char.IsDigit).ToArray());
            string uf = txtUf.Text.Trim().ToUpper();
            string senha = txtSenhaCert.Text;
            string caminho = txtCaminho.Text.Trim().Trim('"');

            if (nome == "" || uf == "" || senha == "" || caminho == "")
            {
                MessageBox.Show("Preencha todos os campos e selecione o certificado.");
                return;
            }

            if (cnpj.Length != 14)
            {
                MessageBox.Show("O CNPJ precisa ter 14 números.");
                return;
            }

            if (!File.Exists(caminho))
            {
                MessageBox.Show("Arquivo do certificado não encontrado.");
                return;
            }

            byte[] certificadoBytes;
            try
            {
                certificadoBytes = File.ReadAllBytes(caminho);
                using var teste = X509CertificateLoader.LoadPkcs12(certificadoBytes, senha);

                string cnpjCert = Regex.Match(teste.Subject, @"\d{14}").Value;
                if (cnpjCert != "" && cnpjCert != cnpj)
                {
                    var r = MessageBox.Show(
                        $"O CNPJ do certificado ({cnpjCert}) é diferente do informado ({cnpj}).\n\n" +
                        "Cadastrar mesmo assim?",
                        "Atenção", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (r != DialogResult.Yes) return;
                }
            }
            catch (CryptographicException)
            {
                MessageBox.Show("Não foi possível abrir o certificado. Confira a senha e se o arquivo é um .pfx válido.");
                return;
            }

            if (ArmazenarBd(nome, cnpj, uf, senha, certificadoBytes))
            {
                txtClienteNome.Clear();
                txtCnpj.Clear();
                txtUf.Clear();
                txtSenhaCert.Clear();
                txtCaminho.Clear();
                _caminhoPfx = "";
            }
        }

        private bool ArmazenarBd(string nome, string cnpj, string uf, string senha, byte[] certificadoBytes)
        {
            try
            {
                const string sql = @"INSERT INTO CLIENTES_CREDENCIAIS
                    (CNPJ_CLIENTE, NOME_CLIENTE, UF_CLIENTE, CERTIFICADO_CLIENTE, SENHA_CERTIFICADO)
                    VALUES (@cnpj, @nome, @uf, @cert, @senha)";

                using var conn = Conexao.AbrirConexao();
                using var cmd = new FbCommand(sql, conn);
                cmd.Parameters.AddWithValue("@cnpj", cnpj);
                cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@uf", uf);
                cmd.Parameters.Add("@cert", FbDbType.Binary).Value = certificadoBytes;
                cmd.Parameters.AddWithValue("@senha", senha);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Cliente cadastrado com sucesso.");
                return true;
            }
            catch (FbException ex) when (ex.Message.Contains("PRIMARY") || ex.Message.Contains("UNIQUE"))
            {
                MessageBox.Show("Já existe um cliente cadastrado com este CNPJ.");
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao cadastrar: " + ex.Message);
                return false;
            }
        }
    }
}