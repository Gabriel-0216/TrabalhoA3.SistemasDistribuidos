using A3.Gestao.Servidor.Models.Enum;

namespace A3.Gestao.Servidor.Utils
{
    public static class RequestParser
    {
        public static string Extrair(string request, Delimitadores delimitador)
        {
            if (string.IsNullOrEmpty(request))
                throw new ArgumentException("Request não pode ser vazio");

            var delimitadorString = delimitador.ToString();
            var inicioIndex = request.IndexOf(delimitadorString);
            var ultimoIndex = request.LastIndexOf(delimitadorString);

            if (inicioIndex == -1 || ultimoIndex == -1 || inicioIndex == ultimoIndex)
                throw new FormatException($"Delimitador '{delimitador}' não encontrado ou inválido");

            return request.Substring(inicioIndex + 1, ultimoIndex - inicioIndex - 1);
        }
    }
}
