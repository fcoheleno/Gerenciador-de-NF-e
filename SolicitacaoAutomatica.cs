namespace Gerenciador_de_NF_e
{
    public class SolicitacaoAutomatica
    {
        public static SolicitacaoAutomatica Instancia { get; } = new();

        // ===== Ajustes =====
        private static readonly TimeSpan IntervaloConferencia  = TimeSpan.FromMinutes(2);
        private static readonly TimeSpan PausaSemSolicitacao   = TimeSpan.FromMinutes(30);
        private static readonly TimeSpan IntervaloEntreClientes = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan EsperaAposFalha       = TimeSpan.FromMinutes(10);
        private const bool AjustarPausaPelaLiberacao = true;  // acorda antes dos 30 min se algum cliente liberar antes
        private const bool TentarNovamenteErros = false;      // cliente com "Erro" volta a ser consultado sozinho?
        // ===================

        private readonly PainelRepositorio _painel = new();
        private readonly ClienteListagem _clientes = new(Conexao.ConnectionString);
        private readonly Dictionary<string, DateTime> _falhas = new();

        private CancellationTokenSource? _parar;
        private CancellationTokenSource? _despertar;
        private bool _consultando;

        public string Status { get; private set; } = "Automático desligado";
        public event EventHandler? StatusAlterado;
        public event EventHandler? RodadaConcluida;   // para o painel recarregar a grade

        public void Iniciar()
        {
            if (_parar != null) return;
            _parar = new CancellationTokenSource();
            _ = LoopAsync(_parar.Token);
        }

        public void Parar()
        {
            _parar?.Cancel();
            _parar = null;
            DefinirStatus("Automático desligado");
        }

        // Interrompe a espera/pausa e confere agora. Se já estiver consultando, não faz nada.
        public void ForcarConferencia()
        {
            if (!_consultando)
                _despertar?.Cancel();
        }

        private async Task LoopAsync(CancellationToken ct)
        {
            try
            {
                while (!ct.IsCancellationRequested)
                {
                    TimeSpan espera = await ConferirAsync(ct);

                    string hora = DateTime.Now.Add(espera).ToString("HH:mm");
                    DefinirStatus(espera > IntervaloConferencia
                        ? $"Automático em pausa até {hora}"
                        : $"Automático: próxima conferência às {hora}");

                    await AguardarAsync(espera, ct);
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                DefinirStatus("Automático parou: " + ex.Message);
            }

        }

      


        private async Task AguardarAsync(TimeSpan tempo, CancellationToken ct)
        {
            using var despertar = CancellationTokenSource.CreateLinkedTokenSource(ct);
            _despertar = despertar;
            try
            {
                await Task.Delay(tempo, despertar.Token);
            }
            catch (OperationCanceledException)
            {
                ct.ThrowIfCancellationRequested();   // parou de vez? sai. Senão foi só o "atualizar".
            }
            
            finally
            {
                _despertar = null;
            }
        }

        // Retorna quanto tempo esperar até a próxima conferência.
        private async Task<TimeSpan> ConferirAsync(CancellationToken ct)
        {
            _consultando = true;
            try
            {
                DefinirStatus("Automático: conferindo clientes...");

                List<ClienteStatus> lista;
                try
                {
                    lista = await Task.Run(() => _painel.Listar(), ct);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    DefinirStatus("Automático: erro ao ler clientes (" + ex.Message + ")");
                    return IntervaloConferencia;
                }

                var disponiveis = lista.Where(Disponivel).ToList();

                if (disponiveis.Count == 0)
                {
                    RodadaConcluida?.Invoke(this, EventArgs.Empty);
                    return CalcularPausa(lista);
                }

                for (int i = 0; i < disponiveis.Count; i++)
                {
                    ct.ThrowIfCancellationRequested();
                    var c = disponiveis[i];

                    DefinirStatus($"Automático: consultando {c.Nome} ({i + 1}/{disponiveis.Count})...");

                    try
                    {
                        var cliente = await Task.Run(() => _clientes.BuscarPorCnpj(c.Cnpj), ct);

                        if (cliente == null)
                            _falhas[c.Cnpj] = DateTime.Now + EsperaAposFalha;   // sem credenciais
                        else
                        {
                            await SolicitacaoExecutor.Instancia.ExecutarAsync(cliente);
                            _falhas.Remove(c.Cnpj);
                        }
                    }
                    catch (Exception ex) when (ex is not OperationCanceledException)
                    {
                        // Falha de rede/certificado etc.: não insiste a cada 2 min.
                        _falhas[c.Cnpj] = DateTime.Now + EsperaAposFalha;
                        DefinirStatus($"Automático: falha em {c.Nome} ({ex.Message})");
                    }

                    RodadaConcluida?.Invoke(this, EventArgs.Empty);

                    if (i < disponiveis.Count - 1)
                        await Task.Delay(IntervaloEntreClientes, ct);
                }

                return IntervaloConferencia;
            }
            finally
            {
                _consultando = false;
            }
        }

        private bool Disponivel(ClienteStatus c)
        {
            if (_falhas.TryGetValue(c.Cnpj, out var ate) && DateTime.Now < ate)
                return false;

            return c.Situacao switch
            {
                SituacaoCliente.NuncaConsultado => true,
                SituacaoCliente.HaNotas => true,
                SituacaoCliente.SemNotasLiberado => true,
                SituacaoCliente.Erro => TentarNovamenteErros,
                _ => false   // AguardandoUmaHora
            };
        }

        private TimeSpan CalcularPausa(List<ClienteStatus> lista)
        {
            TimeSpan pausa = PausaSemSolicitacao;
            if (!AjustarPausaPelaLiberacao) return pausa;

            var agora = DateTime.Now;

            var liberacoes = lista
                .Where(c => c.Situacao == SituacaoCliente.AguardandoUmaHora && c.UltimaConsulta != null)
                .Select(c => c.UltimaConsulta!.Value.AddHours(1))
              .Concat(_falhas.Values.Where(q => q > agora));

            foreach (var quando in liberacoes)
            {
                var falta = quando - agora + TimeSpan.FromSeconds(5);
                if (falta < pausa)
                    pausa = falta;
            }

            return pausa < TimeSpan.FromSeconds(10) ? TimeSpan.FromSeconds(10) : pausa;
        }

        private void DefinirStatus(string texto)
        {
            Status = texto;
            StatusAlterado?.Invoke(this, EventArgs.Empty);
        }
    }
}