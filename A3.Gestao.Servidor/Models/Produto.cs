namespace A3.Gestao.Servidor.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string EmailVendedor { get; set; } = string.Empty;
        public decimal MelhorLance { get; set; }
        public string EmailClienteMelhorLance { get; set; } = string.Empty;
        public bool Finalizado { get; set; } = false;
        public bool TeveLances { get; set; } = false;

        public string FinalizarLeilao()
        {
            Finalizado = true;
            if (!TeveLances) return @$"LEILÃO FINALIZADO, O produto de ID: {Id}, Nome :{Nome}, NÃO TEVE NENHUM LANCE REGISTRADO.";

            return @$"LEILÃO FINALIZADO, DADOS: Id: {Id}, Nome do produto: {Nome}, Email do Comprador: {EmailClienteMelhorLance} Melhor lance: {MelhorLance}";

        }

        public void AtualizarLance(decimal valor, string emailComprador)
        {
            if (valor == 0)
                throw new Exception("O valor do lance deve ser superior a zero");
            if (valor <= MelhorLance)
                throw new Exception("O valor do lance deve ser superior ao lance atual");

            MelhorLance = valor;
            EmailClienteMelhorLance = emailComprador;
        }
        public string Status() => @$"Id: {Id}, Nome do produto: {Nome}, Melhor lance: {MelhorLance}, Em aberto?: {(Finalizado ? "Não" : "Sim")}";
    }
}
