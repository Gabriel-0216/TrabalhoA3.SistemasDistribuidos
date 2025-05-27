namespace Servidor.TestesUnitarios.Requests
{
    public class ProdutoRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string EmailVendedor { get; set; } = string.Empty;
        public decimal? MelhorLance { get; set; }

        public override string ToString()
        {
            return $@"NOME_PRODUTO: ${Nome}$
                  EMAIL_VENDEDOR: &{EmailVendedor}&
                  LANCE_MINIMO: %{MelhorLance}%";
        }
    }
}
