using Gerenciador_de_NF_e;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

public class RespostaSefaz
{
    public string CStat { get; set; }
    public string Motivo { get; set; }
    public string UltNsu { get; set; }
    public string MaxNsu { get; set; }
    public string XmlBruto { get; set; }
}

public class SefazServico
{
    private const string Url ="https://hom1.nfe.fazenda.gov.br/NFeDistribuicaoDFe/NFeDistribuicaoDFe.asmx";
    private const string TpAmb = "2"; // 2 = homologação, 1 = produção
    private string ultNsu = "000000000000000";
    

    public async Task<RespostaSefaz> SolicitarAsync(Cliente cliente)
    {

        if (cliente.CertificadoBytes == null || cliente.CertificadoBytes.Length == 0)
            throw new Exception("O cliente não tem certificado cadastrado.");
        var certificado = new X509Certificate2(cliente.CertificadoBytes, cliente.Senha_Certificado);

        var handler = new HttpClientHandler();
        handler.ClientCertificates.Add(certificado);

        using (var client = new HttpClient(handler))
        {
            string xmlConsulta = $@"<distDFeInt xmlns=""http://www.portalfiscal.inf.br/nfe"" versao=""1.01"">
              <tpAmb>{TpAmb}</tpAmb>
              <cUFAutor>{cliente.UfCodigo}</cUFAutor>
              <CNPJ>{cliente.Cnpj}</CNPJ>
              <distNSU>
                <ultNSU>{ultNsu}</ultNSU>
              </distNSU>
            </distDFeInt>";

            string envelope = $@"<soap12:Envelope xmlns:soap12=""http://www.w3.org/2003/05/soap-envelope"" xmlns:nfe=""http://www.portalfiscal.inf.br/nfe/wsdl/NFeDistribuicaoDFe"">
              <soap12:Header>
                <nfe:nfeCabecMsg>
                  <nfe:cUFAutor>{cliente.UfCodigo}</nfe:cUFAutor>
                  <nfe:versaoDados>1.01</nfe:versaoDados>
                </nfe:nfeCabecMsg>
              </soap12:Header>
              <soap12:Body>
                <nfe:nfeDistDFeInteresse>
                  <nfe:nfeDadosMsg>{xmlConsulta}</nfe:nfeDadosMsg>
                </nfe:nfeDistDFeInteresse>
              </soap12:Body>
            </soap12:Envelope>";

            var conteudo = new StringContent(envelope, Encoding.UTF8, "application/soap+xml");
            var resposta = await client.PostAsync(Url, conteudo);
            string xml = await resposta.Content.ReadAsStringAsync();

            var doc = XDocument.Parse(xml);

            return new RespostaSefaz
            {
                CStat = Ler(doc, "cStat"),
                Motivo = Ler(doc, "xMotivo"),
                UltNsu = Ler(doc, "ultNSU"),
                MaxNsu = Ler(doc, "maxNSU"),
                XmlBruto = xml
            };
        }
    }

    // Lê um campo do XML sem quebrar se ele não existir
    private static string Ler(XDocument doc, string nome)
    {
        return doc.Descendants()
                  .FirstOrDefault(e => e.Name.LocalName == nome)?.Value;
    }
}
