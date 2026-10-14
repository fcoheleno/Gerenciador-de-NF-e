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

        public List<Cliente> Listar()
        {
            var lista = new List<Cliente>();
            using (var conn = new FbConnection(_conexao))
            {
                conn.Open();
                using (var cmd = new FbCommand("SELECT ID, NOME_CLIENTE FROM CREDENCIAIS ORDER BY NOME_CLIENTE", conn))
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                        lista.Add(new Cliente { Id = r.GetString(0), Nome = r.GetString(1) });
                }
            }


            return lista;
        }


    }
}
