namespace A3.Gestao.Servidor.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string EmailVendedor { get; private set; } = string.Empty;
        public decimal MelhorLance { get; private set; }
        public string EmailClienteMelhorLance { get; private set; } = string.Empty;
        public bool Finalizado { get; private set; } = false;
        public bool TeveLances { get; private set; } = false;

        public Produto(string nome, string emailVendedor, decimal melhorLance)
        {
            Nome = nome;
            EmailVendedor = emailVendedor;
            MelhorLance = melhorLance;
            Validar();
        }

        private void Validar()
        {
            if (string.IsNullOrEmpty(Nome))
                throw new Exception("O nome do produto deve ser informado");
            if (string.IsNullOrEmpty(EmailVendedor))
                throw new Exception("O nome do vendedor deve ser informado");
            if (MelhorLance <= 0)
                throw new Exception("O valor do produto deve ser maior que zero");
        }
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

            if (string.IsNullOrEmpty(emailComprador))
                throw new Exception("É obrigatório informar o email do comprador");

            MelhorLance = valor;
            EmailClienteMelhorLance = emailComprador;
        }
    }
}
