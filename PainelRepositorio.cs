using FirebirdSql.Data.FirebirdClient;

namespace Gerenciador_de_NF_e
{
    public class PainelRepositorio
    {
        public List<ClienteStatus> Listar()
        {
            var lista = new List<ClienteStatus>();

            const string sql = @"
                SELECT c.CNPJ_CLIENTE, c.NOME_CLIENTE, c.UF_CLIENTE,
                       s.DATA_HORA, s.RESULTADO
                FROM CLIENTES_CREDENCIAIS c
                LEFT JOIN SOLICITACOES s
                       ON s.ID = (SELECT MAX(s2.ID) FROM SOLICITACOES s2
                                   WHERE s2.CNPJ = c.CNPJ_CLIENTE)
                ORDER BY c.NOME_CLIENTE";

            using var conn = Conexao.AbrirConexao();
            using var cmd = new FbCommand(sql, conn);
            using var r = cmd.ExecuteReader();

            while (r.Read())
            {
                var c = new ClienteStatus
                {
                    Cnpj = Texto(r, 0),
                    Nome = Texto(r, 1),
                    Uf = Texto(r, 2),
                    UltimaConsulta = r.IsDBNull(3) ? null : r.GetDateTime(3),
                    UltimoResultado = Texto(r, 4)
                };
                c.Situacao = CalcularSituacao(c);
                lista.Add(c);
            }

            return lista;
        }

        private static SituacaoCliente CalcularSituacao(ClienteStatus c)
        {
            if (c.UltimaConsulta == null)
                return SituacaoCliente.NuncaConsultado;

            string cStat = c.UltimoResultado.Length >= 3 ? c.UltimoResultado[..3] : c.UltimoResultado;

            if (cStat is not ("137" or "138" or "656"))
                return SituacaoCliente.Erro;

            bool aguardar = cStat is "137" or "656" || c.UltimoResultado.Contains("[aguardar 1h]");

            if (!aguardar)                      // 138 com mais documentos na fila
                return SituacaoCliente.HaNotas;

            return DateTime.Now < c.UltimaConsulta.Value.AddHours(1)
                ? SituacaoCliente.AguardandoUmaHora
                : SituacaoCliente.SemNotasLiberado;
        }

        private static string Texto(FbDataReader r, int i)
        {
            return r.IsDBNull(i) ? "" : (Convert.ToString(r.GetValue(i)) ?? "").Trim();
        }
    }
}