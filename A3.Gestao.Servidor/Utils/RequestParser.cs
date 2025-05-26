using A3.Gestao.Servidor.Models.Constante;

namespace A3.Gestao.Servidor.Utils
{
    public static class RequestParser
    {
        public static string Extrair(string request, Delimitadores delimitador)
        {
            var inicioIndex = request.IndexOf(delimitador.Delimitador);
            var ultimoIndex = request.LastIndexOf(delimitador.Delimitador);
            return request.Substring(inicioIndex + 1, ultimoIndex - inicioIndex - 1);
        }
    }
}
