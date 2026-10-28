using FirebirdSql.Data.FirebirdClient;
using System;
using System.Linq;
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
            if (string.IsNullOrWhiteSpace(cnpj))
                return null;

            string cnpjNumeros = new string(
                cnpj.Where(char.IsDigit).ToArray());

            if (cnpjNumeros.Length != 14)
                return null;

            using (var conn = new FbConnection(Conexao.ConnectionString))
            {
                conn.Open();

                string sql = @"
            SELECT CNPJ_CLIENTE, NOME_CLIENTE, UF_CLIENTE,
                   CERTIFICADO_CLIENTE, SENHA_CERTIFICADO
            FROM CLIENTES_CREDENCIAIS
            WHERE CNPJ_CLIENTE = @cnpj";

                using (var cmd = new FbCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@cnpj", cnpjNumeros);

                    using (var r = cmd.ExecuteReader())
                    {
                        if (!r.Read())
                            return null;

                        return new Cliente
                        {
                            Cnpj = r.IsDBNull(0)
                                ? null
                                : new string(r.GetString(0)
                                    .Where(char.IsDigit).ToArray()),

                            Nome = r.IsDBNull(1) ? null : r.GetString(1),
                            UfCodigo = r.IsDBNull(2) ? null : r.GetString(2),

                            CertificadoBytes = r.IsDBNull(3)
                                ? null
                                : (byte[])r["CERTIFICADO_CLIENTE"],

                            Senha_Certificado = r.IsDBNull(4)
                                ? null
                                : r.GetString(4),

                            UltNsu = "000000000000000"
                        };
                    }
                }
            }
        }

    }
}
