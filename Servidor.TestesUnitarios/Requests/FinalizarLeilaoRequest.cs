namespace Servidor.TestesUnitarios.Requests
{
    public class FinalizarLeilaoRequest
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public override string ToString() => @$"Id: ${Id}$, EMAIL_COMPRADOR: &{Email}&";
    }
}