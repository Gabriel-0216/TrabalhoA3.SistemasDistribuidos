namespace A3.Gestao.Servidor.Models.Constante
{
    public class Delimitadores
    {
        public string Nome { get; set; } = string.Empty;
        public char Delimitador { get; set; }
    }
    public static class NomeDelimitador
    {
        public const string NOME = "NOMEPRODUTO";
        public const string EMAIL_VENDEDOR = "EMAILVENDEDOR";
        public const string VALOR_LANCE = "VALORLANCE";
        public const string EMAIL_COMPRADOR = "EMAILCOMPRADOR";
        public const string ID_PRODUTO = "IDPRODUTO";
    }
}
