using System;
using System.Collections.Generic;
using System.Text;
using FirebirdSql.Data.FirebirdClient;

namespace Gerenciador_de_NF_e
{
    public static class Conexao
    {
        public static string ConnectionString =
    "DataSource=localhost;Port=3050;" +
    "Database=C:\\Users\\2008n\\source\\repos\\Natanael1005-ai\\Gerenciador-de-NF-e-N-\\Dados\\GERENCIAMENTO.FDB;" +
    "User=SYSDBA;Password=masterkey;Charset=UTF8;Dialect=3;" +
    "Connection Timeout=5;";
        public static string ConnectionString = "DataSource=localhost; Port=3050; Database =C:\\Projetos\\Gerenciador-de-NF-e\\Dados\\GERENCIAMENTO.fdb; User=SYSDBA; Password=masterkey; Charset=UTF8; Dialetc=3;";


        public static FbConnection AbrirConexao()
        {
            FbConnection conn = new FbConnection(ConnectionString);
            conn.Open();
            return conn;
        }
    }
}
