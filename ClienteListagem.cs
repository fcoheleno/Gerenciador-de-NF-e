using FirebirdSql.Data.FirebirdClient;

namespace Gerenciador_de_NF_e
{
    public class ClienteListagem
    {
        private readonly string _conexao;

        public ClienteListagem(string conexao)
        {
            _conexao = conexao;
        }

        private string ultNsu = "000000000000000";

        public List<Cliente> Listar()
        {
            var lista = new List<Cliente>();

            using var conn = Conexao.AbrirConexao();

            const string sql = @"SELECT CNPJ_CLIENTE, NOME_CLIENTE
                                 FROM CLIENTES_CREDENCIAIS
                                 ORDER BY NOME_CLIENTE";

            using var cmd = new FbCommand(sql, conn);
            using var r = cmd.ExecuteReader();

            while (r.Read())
            {
                lista.Add(new Cliente
                {
                    Cnpj = r.GetString(0).Trim(),
                    Nome = r.GetString(1).Trim()
                });
            }

            return lista;
        }

        public Cliente? BuscarPorCnpj(string cnpj)
        {
            using var conn = Conexao.AbrirConexao();

            const string sql = @"SELECT CNPJ_CLIENTE, NOME_CLIENTE, UF_CLIENTE,
                                        CERTIFICADO_CLIENTE, SENHA_CERTIFICADO
                                 FROM CLIENTES_CREDENCIAIS
                                 WHERE CNPJ_CLIENTE = @cnpj";

            using var cmd = new FbCommand(sql, conn);
            cmd.Parameters.AddWithValue("@cnpj", cnpj);
            using var r = cmd.ExecuteReader();

            if (!r.Read()) return null;

            return new Cliente
            {
                Cnpj = r.GetString(0).Trim(),
                Nome = r.GetString(1).Trim(),
                UfCodigo = r.IsDBNull(2) ? "" : r.GetString(2).Trim(),
                CertificadoBytes = r.IsDBNull(3) ? null : (byte[])r.GetValue(3),
                Senha_Certificado = r.IsDBNull(4) ? "" : r.GetString(4),
                UltNsu = "000000000000000"   // o NSU real é lido de SOLICITACOES no UcSolicitação
            };
        }

        public void Excluir(string cnpj)
        {
            using (FbConnection conn = Conexao.AbrirConexao())
            {
                string sql = "DELETE FROM CLIENTES_CREDENCIAIS WHERE CNPJ_CLIENTE = @cnpj";
                using (FbCommand cmd = new FbCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@cnpj", cnpj);

                    int linhas = cmd.ExecuteNonQuery();

                    if (linhas == 0)
                    {
                        throw new Exception("Nenhum cliente foi deletado. Confira o CNPJ");
                    }
                }
            }
        }
    }
}