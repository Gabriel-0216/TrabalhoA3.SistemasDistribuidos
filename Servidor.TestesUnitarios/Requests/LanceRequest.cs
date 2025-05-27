namespace Servidor.TestesUnitarios.Requests
{
    public class LanceRequest
    {
        public int IdProduto { get; set; }
        public int Lance { get; set; }
        public string EmailComprador { get; set; }

        public override string ToString()
        {
            return @$"Id_Produto: ${IdProduto}$, Lance: %{Lance}%, Email_Comprador: &{EmailComprador}&";
        }
    }
}
