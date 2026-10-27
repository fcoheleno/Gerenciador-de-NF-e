using FirebirdSql.Data.FirebirdClient;

namespace Gerenciador_de_NF_e
{
    public class UltimaSolicitacao
    {
        public DateTime? DataHora { get; set; }
        public string Resultado { get; set; } = "";
        public string Nsu { get; set; } = "000000000000000";

        // Quando a Sefaz exige esperar 1 hora, devolve a hora em que a nova consulta é liberada
        public DateTime? LiberadaEm
        {
            get
            {
                if (DataHora == null) return null;
                string cStat = Resultado.Length >= 3 ? Resultado[..3] : Resultado;
                bool aguardar = cStat is "137" or "656" || Resultado.Contains("[aguardar 1h]");
                return aguardar ? DataHora.Value.AddHours(1) : null;
            }
        }
    }

    public class SolicitacaoRepositorio
    {
        public UltimaSolicitacao? BuscarUltima(string cnpj)
        {
            const string sql = @"SELECT FIRST 1 DATA_HORA, RESULTADO, NSU
                                 FROM SOLICITACOES
                                 WHERE CNPJ = @cnpj
                                 ORDER BY ID DESC";

            using var conn = Conexao.AbrirConexao();
            using var cmd = new FbCommand(sql, conn);
            cmd.Parameters.AddWithValue("@cnpj", cnpj);
            using var r = cmd.ExecuteReader();

            if (!r.Read()) return null;

            return new UltimaSolicitacao
            {
                DataHora = r.IsDBNull(0) ? null : r.GetDateTime(0),
                Resultado = r.IsDBNull(1) ? "" : (Convert.ToString(r.GetValue(1)) ?? "").Trim(),
                Nsu = r.IsDBNull(2)
                    ? "000000000000000"
                    : (Convert.ToString(r.GetValue(2)) ?? "").Trim().PadLeft(15, '0')
            };
        }

        public void Registrar(string cnpj, string resultado, string nsu)
        {
            const string sql = @"INSERT INTO SOLICITACOES (CNPJ, DATA_HORA, RESULTADO, NSU)
                                 VALUES (@cnpj, @data, @resultado, @nsu)";

            using var conn = Conexao.AbrirConexao();
            using var cmd = new FbCommand(sql, conn);
            cmd.Parameters.AddWithValue("@cnpj", cnpj);
            cmd.Parameters.AddWithValue("@data", DateTime.Now);
            cmd.Parameters.AddWithValue("@resultado", resultado);
            cmd.Parameters.AddWithValue("@nsu", nsu);
            cmd.ExecuteNonQuery();
        }
    }
}