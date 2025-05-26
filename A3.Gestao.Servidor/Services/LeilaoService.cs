using A3.Gestao.Servidor.Models;
using A3.Gestao.Servidor.Models.Enum;
using A3.Gestao.Servidor.Repositories;
using A3.Gestao.Servidor.Utils;

namespace A3.Gestao.Servidor.Services
{
    public class LeilaoService(IProdutoRepository repository)
    {
        private readonly IProdutoRepository _repository = repository;
        public ResultadoOperacao DarLance(string request)
        {
            try
            {
                var produtoId = ProcessarIdProduto(request);
                var valorLance = ProcessarValorLance(request);

                var produto = _repository.Buscar(produtoId);
                if (produto is null)
                    return ResultadoOperacao.CriarErro("Produto não encontrado");

                var emailComprador = Processar(request, Delimitadores.EmailComprador);

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
            var produto = RetornaProdutoCadastro(request);
            _repository.Adicionar(produto);
            return produto.Id;
        }

        public string ConsultaArrematados(string request)
        {
            var emailComprador = Processar(request, Delimitadores.EmailComprador);

            var produtos = _repository.ListarFinalizados();

            produtos = produtos.Where(x => x.EmailClienteMelhorLance.Equals(emailComprador)).ToList();

            var mensagem = "";
            if (!produtos.Any())
                mensagem = "Você não arrematou nenhum leilão";

            foreach(var produto in produtos)
                mensagem += $"{produto.Status()}";

            return mensagem;
        }
        public string FinalizarLeilao(string request)
        {
            var idProduto = ProcessarIdProduto(request);

            var emailVendedor = Processar(request, Delimitadores.EmailVendedor);

            var produto = _repository.Buscar(idProduto);
            if (produto is null)
                return @"produto não existe";

            if (produto.EmailVendedor == emailVendedor)
                return produto.FinalizarLeilao();

            return "Você não tem autorização para encerrar esse leilão";
        }


        private static Produto RetornaProdutoCadastro(string request)
        {
            var produtoNome = Processar(request, Delimitadores.Nome);
            var emailVendedor = Processar(request, Delimitadores.EmailVendedor);
            var valor = Processar(request, Delimitadores.ValorLance);
            
            return new Produto(produtoNome, emailVendedor, Convert.ToDecimal(valor));
        }
        private static int ProcessarIdProduto(string request)
        {
            if (!int.TryParse(RequestParser.Extrair(request, Delimitadores.IdProduto), out int idProduto))
                throw new Exception("Não foi possível converter a requisição em um id produto válido");

            return idProduto;
        }
        private static decimal ProcessarValorLance(string request)
        {
            if (!decimal.TryParse(RequestParser.Extrair(request, Delimitadores.Decimal), out decimal valorLance))
                throw new Exception("Não foi possível converter o valor do lance em um decimal válido");

            return valorLance;
        }
        private static string Processar(string request, Delimitadores delimitador)
        {
            return RequestParser.Extrair(request, delimitador);
        }

    }
}
