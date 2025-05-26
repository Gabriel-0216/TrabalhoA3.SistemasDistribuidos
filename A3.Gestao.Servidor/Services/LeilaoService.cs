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
            var produtoId = Convert.ToInt32(RequestParser.Extrair(request, Delimitadores.IdProduto));
            var valorLance = Convert.ToDecimal(RequestParser.Extrair(request, Delimitadores.Decimal));
            var emailComprador = RequestParser.Extrair(request, Delimitadores.Texto);

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
            var delimitadorNome = Delimitadores.Nome;
            var produtoNome = RequestParser.Extrair(request, delimitadorNome);

            var delimitadorEmail = Delimitadores.EmailVendedor;
            var emailVendedor = RequestParser.Extrair(request, delimitadorEmail);

            var delimitadorValor = Delimitadores.ValorLance;
            var valor = RequestParser.Extrair(request, delimitadorValor);

            var produto = new Produto(produtoNome, emailVendedor, Convert.ToDecimal(valor));

            _repository.Adicionar(produto);
            return produto.Id;
        }

        public string ConsultaArrematados(string request)
        {
            var delimitadorEmail = Delimitadores.EmailComprador;
            var emailComprador = RequestParser.Extrair(request, delimitadorEmail);

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
            var delimitadorId = Delimitadores.IdProduto;
            var idProduto = RequestParser.Extrair(request, delimitadorId);

            var delimitadorEmail = Delimitadores.EmailVendedor;
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
