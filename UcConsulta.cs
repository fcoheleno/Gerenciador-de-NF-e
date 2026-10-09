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
    public partial class UcConsulta : UserControl
    {
        public UcConsulta()
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
            //aq eu vou colocar a logica de guardar algumas informações da empresas [Nome, Cnpj, Senha e o certificado]

            string tpAmb = "2";
            string codigoUf = "23";
            //string cnpj = Regex.Match(certificadoCliente.Subject, @"\d{14}").Value;
            //Console.WriteLine("Cnpj do certificado: " + cnpj);
            string Nsu = "000000000000000";
            //isso aq foi pego lá do codigo anterior

             situacao = ArmazenarBd();
        }



        //funções que vão ser utilizadas nop decorrer do codigo



        private bool ArmazenarBd() 
        {
            string nomeCliente = txtClienteNome.Text;
            string cnpjCliente = txtCnpj.Text;
            string senhaCertificado = txtSenhaCert.Text;
            string certificadoCaminho = caminhoPfx;

            using (FbConnection conn = Conexao.AbrirConexao())
            {
                try
                {
                    string query = "INSERT INTO CREDENCIAIS(NOME_CLIENTE, CNPJ_CLIENTE, CERTIFICADO_CAMINHO, SENHA_CERTIFICADO) VALUES(@NomeCliente, @CnpjCliente, @CertificadoCaminho, @SenhaCertificado)";
                    using (FbCommand cmd = new FbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@NomeCliente", nomeCliente);
                        cmd.Parameters.AddWithValue("@CnpjCliente", cnpjCliente);
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


        //private bool Verificador() { }

        static async RealizarSolicitacao(string CnpjCliente, string certificadoCaminho, string senhaCertificado)
        {
            var certificadoCliente = new X509Certificate2(certificadoCaminho);

            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(certificadoCliente);

            using var client = new HttpClient(handler);
            string url = "https://hom1.nfe.fazenda.gov.br/NFeDistribuicaoDFe/NFeDistribuicaoDFe.asmx";
            string tpAmb = "2";
            string codigoUf = "23";

            //tenho q fazer uma lógica: ver primeiramente se o cnpj q o cara digitou é igual ao do certificado que ele mandou!

            string nsu = "000000000000000";

            string xmlConsulta = $@"<distDFeInt xmlns=""http://www.portalfiscal.inf.br/nfe"" versao=""1.01"">
          <tpAmb>{tpAmb}</tpAmb>
          <cUFAutor>{codigoUf}</cUFAutor>
          <CNPJ>{CnpjCliente}</CNPJ>
          <distNSU>
            <ultNSU>{nsu}</ultNSU>
          </distNSU>
          </distDFeInt>";

            string envelope = $@"<soap12:Envelope xmlns:soap12=""http://www.w3.org/2003/05/soap-envelope"" xmlns:nfe=""http://www.portalfiscal.inf.br/nfe/wsdl/NFeDistribuicaoDFe"">
          <soap12:Header>
            <nfe:nfeCabecMsg>
              <nfe:cUFAutor>23</nfe:cUFAutor>
              <nfe:versaoDados>1.01</nfe:versaoDados>
            </nfe:nfeCabecMsg>
          </soap12:Header>
          <soap12:Body>
            <nfe:nfeDistDFeInteresse>
              <nfe:nfeDadosMsg>{xmlConsulta}</nfe:nfeDadosMsg>
            </nfe:nfeDistDFeInteresse>
          </soap12:Body>
        </soap12:Envelope>";


            var conteudo = new StringContent(envelope, System.Text.Encoding.UTF8,
            "application/soap+xml");
            var resposta = await client.PostAsync(url, conteudo);
            Console.WriteLine((int)resposta.StatusCode);
            Console.WriteLine(await resposta.Content.ReadAsStringAsync());

            var respostaXml = await resposta.Content.ReadAsStringAsync();
            var doc = XDocument.Parse(respostaXml);
            string novoNsu = doc.Descendants().First(e => e.Name.LocalName == "ultNSU").Value;
            string cStat = doc.Descendants().First(e => e.Name.LocalName == "cStat").Value;
            string maxNsu = doc.Descendants().First(e => e.Name.LocalName == "maxNSU").Value;


            Console.WriteLine($"cStat: {cStat}");
            Console.WriteLine($"Ultimo NSU: {novoNsu}");
            Console.WriteLine($"Máximo NSU: {maxNsu}");

            File.WriteAllText(@"C:\certificado\ultnsu.xml", novoNsu);
            string ultNSU = File.ReadAllText(@"C:\certificado\ultNSU.xml");
        }
    }
}
