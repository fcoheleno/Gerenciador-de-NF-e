using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Text;

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

            using (FbConnection conn = Conexao.AbrirConexao())
            {

                string sql = @"SELECT CNPJ_CLIENTE, NOME_CLIENTE
                               FROM CLIENTES_CREDENCIAIS
                               ORDER BY NOME_CLIENTE";

                using (var cmd = new FbCommand(sql, conn))
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        lista.Add(new Cliente
                        {
                            Cnpj = r.GetString(0),
                            Nome = r.GetString(1)
                        });
                    }
                }
            }

            return lista;
        }

        public Cliente BuscarPorCnpj(string cnpj)
        {
            using (var conn = new FbConnection(Conexao.ConnectionString))
            {
                conn.Open();

                string sql = @"SELECT CNPJ_CLIENTE, NOME_CLIENTE, UF_CLIENTE,
                              CERTIFICADO_CLIENTE, SENHA_CERTIFICADO
                       FROM CLIENTES_CREDENCIAIS
                       WHERE CNPJ_CLIENTE = @cnpj";
                

                using (var cmd = new FbCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@cnpj", cnpj);

                    using (var r = cmd.ExecuteReader())
                    {
                        
                        if (!r.Read()) return null;

                        return new Cliente
                        {
                            Cnpj = r.GetString(0),
                            Nome = r.GetString(1),
                            UfCodigo = r.GetString(2),
                            CertificadoBytes = r.IsDBNull(3) ? null : (byte[])r["CERTIFICADO_CLIENTE"],
                            Senha_Certificado = r.GetString(4),
                            UltNsu = "000000000000000" // provisório


                        };
                        
                    }
                }
            }
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
