namespace A3.Gestao.Servidor.Models
{
    public class ResultadoOperacao<T>
    {
        public bool FoiSucesso { get; private set; } 
        public string? Mensagem { get; private set; }
        public T Content { get; private set; }
        public void CriarSucesso(T content, string mensagem = "Operação concluída")
        {
            Mensagem = mensagem;
            FoiSucesso = true;
            Content = content;
        }

        public void CriarErro(string mensagemErro)
        {
            Mensagem = mensagemErro;
            FoiSucesso = false;
        }
    }
}
