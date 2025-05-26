using A3.Gestao.Servidor.Models;
using A3.Gestao.Servidor.Models.Constante;
using A3.Gestao.Servidor.Repositories;
using A3.Gestao.Servidor.Utils;

namespace A3.Gestao.Servidor.Services
{
    public class LeilaoService(IProdutoRepository repository, IList<Delimitadores> delimitadores)
    {
        private readonly IProdutoRepository _repository = repository;
        private IList<Delimitadores> _delimitadores = delimitadores;
        public ResultadoOperacao DarLance(string request)
        {
            var produtoId = Convert.ToInt32(RequestParser.Extrair(request, _delimitadores.FirstOrDefault(p => p.Nome == NomeDelimitador.ID_PRODUTO)));
            var valorLance = Convert.ToDecimal(RequestParser.Extrair(request, _delimitadores.FirstOrDefault(p => p.Nome == NomeDelimitador.DECIMAL)));
            var emailComprador = RequestParser.Extrair(request, _delimitadores.FirstOrDefault(p => p.Nome == NomeDelimitador.TEXTO));

            var produto = _repository.Buscar(produtoId);
            if (produto == null)
                return ResultadoOperacao.CriarErro("Produto não encontrado");

            try
            {
                produto.AtualizarLance(valorLance, emailComprador);
                return ResultadoOperacao.CriarSucesso();
            }
            catch(Exception ex)
            {
                return ResultadoOperacao.CriarErro(ex.Message);
            }
        }

        public string RetornarProdutos()
        {
            var produtos = _repository.Listar();

            if (!produtos.Any())
                return "Nenhum produto em leilão";

            var mensagem = "";
            foreach (var item in produtos.Where(p => p.Finalizado == false).ToList())
                mensagem += $"{item.Status()}" + Environment.NewLine;

            return mensagem;

        }

        public int Cadastro(string request)
        {
            var delimitadorNome = _delimitadores.FirstOrDefault(p => p.Nome == NomeDelimitador.NOME);
            var produtoNome = RequestParser.Extrair(request, delimitadorNome);

            var delimitadorEmail = _delimitadores.FirstOrDefault(p => p.Nome == NomeDelimitador.EMAIL_VENDEDOR);
            var emailVendedor = RequestParser.Extrair(request, delimitadorEmail);

            var delimitadorValor = _delimitadores.FirstOrDefault(p => p.Nome == NomeDelimitador.VALOR_LANCE);
            var valor = RequestParser.Extrair(request, delimitadorValor);


            var produto = new Produto() { Nome = produtoNome, EmailVendedor = emailVendedor, MelhorLance = Convert.ToDecimal(valor) };

            _repository.Adicionar(produto);
            return produto.Id;
        }

        public string ConsultaArrematados(string request)
        {
            var delimitadorEmail = _delimitadores.FirstOrDefault(p => p.Nome == NomeDelimitador.EMAIL_COMPRADOR);
            var emailComprador = RequestParser.Extrair(request, delimitadorEmail);

            var produtos = _repository.ListarFinalizados();

            produtos = produtos.Where(x => x.EmailClienteMelhorLance.Equals(emailComprador)).ToList();

            var mensagem = "";
            if (!produtos.Any())
                mensagem = "Você não arrematou nenhum leilão";

            foreach(var produto in produtos)
                mensagem += $"{produto.Status}";

            return mensagem;
        }
        public string FinalizarLeilao(string request)
        {
            var delimitadorId = _delimitadores.FirstOrDefault(p => p.Nome == NomeDelimitador.ID_PRODUTO);
            var idProduto = RequestParser.Extrair(request, delimitadorId);

            var delimitadorEmail = _delimitadores.FirstOrDefault(p => p.Nome == NomeDelimitador.EMAIL_VENDEDOR);
            var emailVendedor = RequestParser.Extrair(request, delimitadorEmail);

            var produto = _repository.Buscar(Convert.ToInt32(idProduto));
            if (produto is null)
                return @"produto não existe";

            if (produto.EmailVendedor == emailVendedor)
                return produto.FinalizarLeilao();

            return "Você não tem autorização para encerrar esse leilão";
        }


    }
}
