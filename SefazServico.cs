using System.IO.Compression;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;

namespace Gerenciador_de_NF_e
{
    public class RespostaSefaz
    {
        public string CStat { get; set; } = "";
        public string Motivo { get; set; } = "";
        public string UltNsu { get; set; } = "";
        public string MaxNsu { get; set; } = "";
        public List<string> Chaves { get; set; } = new();
        public string XmlBruto { get; set; } = "";

        // Regra da Sefaz: sem documentos (137), consumo indevido (656)
        // ou fim da fila (ultNSU == maxNSU) => aguardar 1 hora
        public bool AguardarUmaHora =>
            CStat is "137" or "656" || (CStat == "138" && UltNsu == MaxNsu);
    }

    public class SefazServico
    {
        private const string UrlProducao =
            "https://www1.nfe.fazenda.gov.br/NFeDistribuicaoDFe/NFeDistribuicaoDFe.asmx";
        private const string UrlHomologacao =
            "https://hom1.nfe.fazenda.gov.br/NFeDistribuicaoDFe/NFeDistribuicaoDFe.asmx";
        private const string SoapAction =
            "http://www.portalfiscal.inf.br/nfe/wsdl/NFeDistribuicaoDFe/nfeDistDFeInteresse";

        private static readonly Dictionary<string, string> CodigosUf =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["RO"] = "11",
                ["AC"] = "12",
                ["AM"] = "13",
                ["RR"] = "14",
                ["PA"] = "15",
                ["AP"] = "16",
                ["TO"] = "17",
                ["MA"] = "21",
                ["PI"] = "22",
                ["CE"] = "23",
                ["RN"] = "24",
                ["PB"] = "25",
                ["PE"] = "26",
                ["AL"] = "27",
                ["SE"] = "28",
                ["BA"] = "29",
                ["MG"] = "31",
                ["ES"] = "32",
                ["RJ"] = "33",
                ["SP"] = "35",
                ["PR"] = "41",
                ["SC"] = "42",
                ["RS"] = "43",
                ["MS"] = "50",
                ["MT"] = "51",
                ["GO"] = "52",
                ["DF"] = "53"
            };

        private readonly bool _producao;

        public SefazServico(bool producao = false)
        {
            _producao = producao;
        }

        public async Task<RespostaSefaz> SolicitarAsync(Cliente cliente)
        {
            if (cliente.CertificadoBytes == null || cliente.CertificadoBytes.Length == 0)
                throw new Exception("O cliente não tem certificado cadastrado.");

            X509Certificate2 certificado;
            try
            {
                certificado = X509CertificateLoader.LoadPkcs12(
                    cliente.CertificadoBytes, cliente.Senha_Certificado);
            }
            catch (System.Security.Cryptography.CryptographicException)
            {
                throw new Exception(
                    "Não foi possível abrir o certificado do cliente. " +
                    "Confira a senha cadastrada e se o arquivo é um .pfx válido.");
            }

            using (certificado)
            using (var handler = new HttpClientHandler())
            {
                handler.ClientCertificates.Add(certificado);
                using var client = new HttpClient(handler);

                string ultNsu = (cliente.UltNsu ?? "0").PadLeft(15, '0');
                string envelope = MontarEnvelope(cliente.Cnpj, ConverterUf(cliente.UfCodigo), ultNsu);

                var conteudo = new StringContent(envelope, Encoding.UTF8);
                conteudo.Headers.ContentType = MediaTypeHeaderValue.Parse(
                    $"application/soap+xml; charset=utf-8; action=\"{SoapAction}\"");

                string url = _producao ? UrlProducao : UrlHomologacao;
                using var resposta = await client.PostAsync(url, conteudo);
                string xml = await resposta.Content.ReadAsStringAsync();

                XDocument doc;
                try { doc = XDocument.Parse(xml); }
                catch
                {
                    throw new Exception(
                        $"Resposta inválida da Sefaz (HTTP {(int)resposta.StatusCode}): {xml}");
                }

                var ret = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "retDistDFeInt");
                if (ret == null)
                    throw new Exception(
                        $"Resposta inesperada da Sefaz (HTTP {(int)resposta.StatusCode}): {xml}");

                var r = new RespostaSefaz
                {
                    CStat = Ler(ret, "cStat"),
                    Motivo = Ler(ret, "xMotivo"),
                    UltNsu = Ler(ret, "ultNSU"),
                    MaxNsu = Ler(ret, "maxNSU"),
                    XmlBruto = xml
                };

                foreach (var docZip in ret.Descendants().Where(e => e.Name.LocalName == "docZip"))
                {
                    string schema = (string?)docZip.Attribute("schema") ?? "";
                    if (!schema.StartsWith("resNFe") && !schema.StartsWith("procNFe"))
                        continue;

                    var d = XDocument.Parse(Descompactar(docZip.Value));
                    string? chave =
                        d.Descendants().FirstOrDefault(e => e.Name.LocalName == "chNFe")?.Value
                        ?? d.Descendants().FirstOrDefault(e => e.Name.LocalName == "infNFe")
                             ?.Attribute("Id")?.Value?.Substring(3);

                    if (!string.IsNullOrEmpty(chave) && !r.Chaves.Contains(chave))
                        r.Chaves.Add(chave);
                }

                return r;
            }
        }

        // A Sefaz exige o código numérico do IBGE; se o cadastro guardar a sigla (CE), converte
        private static string ConverterUf(string uf)
        {
            uf = (uf ?? "").Trim();
            return CodigosUf.TryGetValue(uf, out var codigo) ? codigo : uf;
        }

        private string MontarEnvelope(string cnpj, string cUf, string ultNsu)
        {
            return
$@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap12:Envelope xmlns:soap12=""http://www.w3.org/2003/05/soap-envelope"">
  <soap12:Body>
    <nfeDistDFeInteresse xmlns=""http://www.portalfiscal.inf.br/nfe/wsdl/NFeDistribuicaoDFe"">
      <nfeDadosMsg>
        <distDFeInt xmlns=""http://www.portalfiscal.inf.br/nfe"" versao=""1.01"">
          <tpAmb>{(_producao ? 1 : 2)}</tpAmb>
          <cUFAutor>{cUf}</cUFAutor>
          <CNPJ>{cnpj}</CNPJ>
          <distNSU><ultNSU>{ultNsu}</ultNSU></distNSU>
        </distDFeInt>
      </nfeDadosMsg>
    </nfeDistDFeInteresse>
  </soap12:Body>
</soap12:Envelope>";
        }

        private static string Descompactar(string base64Gzip)
        {
            byte[] bytes = Convert.FromBase64String(base64Gzip);
            using var ms = new MemoryStream(bytes);
            using var gz = new GZipStream(ms, CompressionMode.Decompress);
            using var sr = new StreamReader(gz, Encoding.UTF8);
            return sr.ReadToEnd();
        }

        private static string Ler(XElement pai, string nome)
        {
            return pai.Elements().FirstOrDefault(e => e.Name.LocalName == nome)?.Value ?? "";
        }
    }
}