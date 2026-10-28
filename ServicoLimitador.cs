
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Gerenciador_de_NF_e
{
    public sealed class ResultadoLimitador
    {
        public bool Autorizada
        {
            get; set;
        }
        public int Total
        {
            get; set;
        }
        public int Restantes
        {
            get; set;
        }
        public int SegundosRestantes
        {
            get; set;
        }
        public string TempoRestante { get; set; } = "00:00:00";
        public string? Liberacao
        {
            get; set;
        }
        public string? Mensagem
        {
            get; set;
        }
        public string? Erro
        {
            get; set;
        }
        public bool BloqueioConsumo
        {
            get; set;
        }
    }

    public sealed class ServicoLimitador
    {
        private static readonly HttpClient ClienteHttp = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(10)
        };

        private readonly string _url;
        private readonly string _urlBase;

        public ServicoLimitador()
        {
            _url = Environment.GetEnvironmentVariable("LIMITADOR_API_URL")
                ?? "http://127.0.0.1:3000/api/consultas/autorizar";

            _urlBase = _url.EndsWith(
                "/api/consultas/autorizar",
                StringComparison.OrdinalIgnoreCase)
                ? _url[..^"/api/consultas/autorizar".Length]
                : "http://127.0.0.1:3000";
        }

        private static string ValidarCnpj(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                throw new ArgumentException(
                    "Selecione um cliente com CNPJ válido.");

            string numeros = new string(
                cnpj.Where(char.IsDigit).ToArray());

            if (numeros.Length != 14)
                throw new ArgumentException(
                    "O CNPJ deve conter 14 números.");

            return numeros;
        }

        private static async Task<ResultadoLimitador> EnviarAsync(
            string url,
            string cnpj)
        {
            using var resposta = await ClienteHttp.PostAsJsonAsync(
                url,
                new
                {
                    cnpj
                });

            string json = await resposta.Content.ReadAsStringAsync();

            ResultadoLimitador? resultado = null;

            try
            {
                resultado = JsonSerializer.Deserialize<ResultadoLimitador>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
            catch (JsonException)
            {
                // Uma resposta inválida nunca deve autorizar uma consulta.
            }

            if (!resposta.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    resultado?.Erro ??
                    "Não foi possível comunicar com a API do limitador.");
            }

            if (resultado == null)
            {
                throw new InvalidOperationException(
                    "A API retornou uma resposta inválida.");
            }

            return resultado;
        }

        public Task<ResultadoLimitador> ConsultarStatusAsync(string cnpj)
        {
            string cnpjNumeros = ValidarCnpj(cnpj);

            return EnviarAsync(
                _urlBase + "/api/consultas/status",
                cnpjNumeros);
        }

        public Task<ResultadoLimitador> AutorizarAsync(string cnpj)
        {
            string cnpjNumeros = ValidarCnpj(cnpj);

            return EnviarAsync(_url, cnpjNumeros);
        }

        public async Task RegistrarBloqueioConsumoAsync(string cnpj)
        {
            string cnpjNumeros = ValidarCnpj(cnpj);

            await EnviarAsync(
                _urlBase + "/api/consultas/bloqueio-consumo",
                cnpjNumeros);
        }
    }
}