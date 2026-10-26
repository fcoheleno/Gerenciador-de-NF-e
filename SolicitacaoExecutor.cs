namespace Gerenciador_de_NF_e
{
    public class ResultadoSolicitacao
    {
        public bool Pulada { get; init; }              // ainda dentro da espera de 1 h
        public DateTime? LiberadaEm { get; init; }
        public RespostaSefaz? Resposta { get; init; }
        public string AvisoGravacao { get; init; } = "";
    }

    public class SolicitacaoExecutor
    {
        // false = homologação; true = produção
        public const bool Producao = false;

        public static SolicitacaoExecutor Instancia { get; } = new();

        private readonly SolicitacaoRepositorio _solicitacoes = new();
        private readonly SefazServico _sefaz = new(Producao);
        private readonly SemaphoreSlim _trava = new(1, 1);

        public async Task<ResultadoSolicitacao> ExecutarAsync(Cliente cliente)
        {
            await _trava.WaitAsync();
            try
            {
                var ultima = await Task.Run(() => _solicitacoes.BuscarUltima(cliente.Cnpj));

                if (ultima?.LiberadaEm is DateTime liberada && DateTime.Now < liberada)
                    return new ResultadoSolicitacao { Pulada = true, LiberadaEm = liberada };

                cliente.UltNsu = ultima?.Nsu ?? "000000000000000";

                RespostaSefaz resp = await _sefaz.SolicitarAsync(cliente);

                bool sucesso = resp.CStat is "137" or "138";
                string nsuGravar = sucesso && !string.IsNullOrWhiteSpace(resp.UltNsu)
                    ? resp.UltNsu
                    : cliente.UltNsu;

                string resultado = $"{resp.CStat} - {resp.Motivo}";
                if (resp.CStat == "138" && resp.AguardarUmaHora)
                    resultado += " [aguardar 1h]";
                if (resultado.Length > 100)
                    resultado = resultado[..100];

                string aviso = "";
                try
                {
                    await Task.Run(() => _solicitacoes.Registrar(cliente.Cnpj, resultado, nsuGravar));
                }
                catch (Exception ex)
                {
                    aviso = "\n\nAtenção: a consulta foi feita, mas não foi possível " +
                            "gravar o histórico: " + ex.Message;
                }

                return new ResultadoSolicitacao { Resposta = resp, AvisoGravacao = aviso };
            }
            finally
            {
                _trava.Release();
            }
        }
    }
}