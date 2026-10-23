namespace Gerenciador_de_NF_e
{
    public enum SituacaoCliente
    {
        NuncaConsultado,
        HaNotas,
        AguardandoUmaHora,
        SemNotasLiberado,
        Erro
    }

    public class ClienteStatus
    {
        public string Cnpj { get; set; } = "";
        public string Nome { get; set; } = "";
        public string Uf { get; set; } = "";
        public DateTime? UltimaConsulta { get; set; }
        public string UltimoResultado { get; set; } = "";
        public int NotasGuardadas { get; set; }
        public SituacaoCliente Situacao { get; set; }

        public string CnpjFormatado => Cnpj.Length == 14
            ? $"{Cnpj[..2]}.{Cnpj.Substring(2, 3)}.{Cnpj.Substring(5, 3)}/{Cnpj.Substring(8, 4)}-{Cnpj.Substring(12, 2)}"
            : Cnpj;

        public string SituacaoTexto => Situacao switch
        {
            SituacaoCliente.NuncaConsultado => "Nunca consultado",
            SituacaoCliente.HaNotas => "Há notas para consultar",
            SituacaoCliente.AguardandoUmaHora => "Aguardando 1 hora",
            SituacaoCliente.SemNotasLiberado => "Sem notas (liberado)",
            _ => "Erro na última consulta"
        };

        public string UltimaConsultaTexto =>
            UltimaConsulta?.ToString("dd/MM/yyyy HH:mm") ?? "Nunca";

        public string ProximaConsultaTexto => Situacao switch
        {
            SituacaoCliente.AguardandoUmaHora =>
                "Às " + UltimaConsulta!.Value.AddHours(1).ToString("HH:mm"),
            SituacaoCliente.Erro => "—",
            _ => "Agora"
        };
    }
}