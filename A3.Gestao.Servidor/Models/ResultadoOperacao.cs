namespace A3.Gestao.Servidor.Models
{
    public class ResultadoOperacao
    {
        public bool FoiSucesso { get; } 
        public string Mensagem { get; }
        public object? Dados { get; }

        private ResultadoOperacao(bool foiSucesso, string mensagem, object? dados = null)
        {
            FoiSucesso = foiSucesso;
            Mensagem = mensagem;
            Dados = dados;
        }

        public static ResultadoOperacao CriarSucesso(string mensagem = "Operação concluída", object? dados = null) => new (true, mensagem, dados);

        public static ResultadoOperacao CriarErro(string mensagemErro) => new (false, mensagemErro);
    }
}
