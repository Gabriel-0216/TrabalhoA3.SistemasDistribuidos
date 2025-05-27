using A3.Gestao.Servidor.Models;
using A3.Gestao.Servidor.Models.Enum;
using A3.Gestao.Servidor.Repositories;
using A3.Gestao.Servidor.Utils;

namespace A3.Gestao.Servidor.Services
{
    public class LeilaoService(IProdutoRepository repository)
    {
        private readonly IProdutoRepository _repository = repository;
        public ResultadoOperacao<bool> DarLance(string request)
        {
            var resultado = new ResultadoOperacao<bool>();
            try
            {
                var produtoId = ProcessarIdProduto(request);
                var valorLance = ProcessarValorLance(request);

                var produto = _repository.Buscar(produtoId);

                if (produto is null){
                    resultado.CriarErro("Produto não foi encontrado");
                    return resultado;
                }

                var emailComprador = Processar(request, Delimitadores.Email);

                produto.AtualizarLance(valorLance, emailComprador);

                resultado.CriarSucesso(true);
                return resultado;
            }
            catch(Exception ex)
            {
                resultado.CriarErro(ex.Message);
                return resultado;
            }
        }

        public ResultadoOperacao<IList<Produto>> RetornarProdutos()
        {
            var resultado = new ResultadoOperacao<IList<Produto>>();

            var produtos = _repository.Listar();

            if (!produtos.Any())
            {
                resultado.CriarErro("Nenhum produto encontrado");
            }
            else
            {
                resultado.CriarSucesso(content: produtos);
            }
            return resultado;
        }

        public ResultadoOperacao<Produto> Cadastro(string request)
        {
            var resultado = new ResultadoOperacao<Produto>();
            try
            {
                var produto = RetornaProdutoCadastro(request);
                _repository.Adicionar(produto);
                resultado.CriarSucesso(produto);
                return resultado;
            }
            catch(Exception ex)
            {
                resultado.CriarErro(ex.Message);
                return resultado;
            }
        }

        public ResultadoOperacao<IList<Produto>> ConsultaArrematados(string request)
        {
            var resultado = new ResultadoOperacao<IList<Produto>>();
            try
            {
                var emailComprador = Processar(request, Delimitadores.Email);

                var produtos = _repository.ListarFinalizados();

                produtos = produtos.Where(x => x.EmailClienteMelhorLance.Equals(emailComprador)).ToList();

                if (!produtos.Any())
                {
                    resultado.CriarErro("Você não arrematou nenhum produto");
                }
                else
                {
                    resultado.CriarSucesso(produtos);
                }
                return resultado;
            }
            catch(Exception ex)
            {
                resultado.CriarErro(ex.Message);
                return resultado;
            }
        }
        public ResultadoOperacao<Produto> FinalizarLeilao(string request)
        {
            var resultado = new ResultadoOperacao<Produto>();

            try
            {
                var idProduto = ProcessarIdProduto(request);

                var emailVendedor = Processar(request, Delimitadores.Email);

                var produto = _repository.Buscar(idProduto);
                if (produto is null)
                {
                    resultado.CriarErro("Produto não existe");
                    return resultado;
                }

                if(!produto.EmailVendedor.Equals(emailVendedor, StringComparison.CurrentCultureIgnoreCase))
                {
                    resultado.CriarErro("Você não tem autorização para encerrar esse leilão");
                }
                else
                {
                    produto.FinalizarLeilao();
                    resultado.CriarSucesso(produto);
                }
                return resultado;
            }
            catch(Exception ex)
            {
                resultado.CriarErro(ex.Message);
                return resultado;
            }
        }


        private static Produto RetornaProdutoCadastro(string request)
        {
            var produtoNome = Processar(request, Delimitadores.Nome);
            var emailVendedor = Processar(request, Delimitadores.Email);
            var valor = Processar(request, Delimitadores.ValorLance);
            
            return new Produto(produtoNome, emailVendedor, Convert.ToDecimal(valor));
        }
        private static int ProcessarIdProduto(string request)
        {
            if (!int.TryParse(Processar(request, Delimitadores.IdProduto), out int idProduto))
                throw new Exception("Não foi possível converter a requisição em um id produto válido");

            return idProduto;
        }
        private static decimal ProcessarValorLance(string request)
        {
            if (!decimal.TryParse(Processar(request, Delimitadores.ValorLance), out decimal valorLance))
                throw new Exception("Não foi possível converter o valor do lance em um decimal válido");

            return valorLance;
        }
        private static string Processar(string request, Delimitadores delimitador)
        {
            return RequestParser.Extrair(request, delimitador);
        }

    }
}
