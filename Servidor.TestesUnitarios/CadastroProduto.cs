using A3.Gestao.Servidor.Models;
using A3.Gestao.Servidor.Repositories;
using A3.Gestao.Servidor.Services;
using Servidor.TestesUnitarios.Requests;
using System.Text;

namespace Servidor.TestesUnitarios
{
    public class CadastroProduto
    {
        public LeilaoService _svc;
        public CadastroProduto()
        {
            _svc = new(new ProdutoRepository());
        }
        [Fact]
        public void CadastroValido()
        {
            var req = MontaRequestProduto("Produto", "produto@produto", 10m);
            var result = _svc.Cadastro(req);
            Assert.True(result.FoiSucesso);
        }

        [Fact]
        public void NomeInvalido()
        {
            var req = MontaRequestProduto("", "produto@produto", 10m);
            var result = _svc.Cadastro(req);
            Assert.False(result.FoiSucesso);
        }
        [Fact]
        public void EmailInvalido()
        {
            var req = MontaRequestProduto("01", "", 10m);
            var result = _svc.Cadastro(req);
            Assert.False(result.FoiSucesso);
        }
        [Fact]
        public void ValorInvalido()
        {
            var req = MontaRequestProduto("01", "abc@abc", -10m);
            var result = _svc.Cadastro(req);
            Assert.False(result.FoiSucesso);
        }

        [Fact]
        public void ListarProdutosRetornaNenhum()
        {
            var result = _svc.RetornarProdutos();
            Assert.False(result.FoiSucesso);
        }
        [Fact]
        public void ListarProdutosRetorna()
        {
            var req = MontaRequestProduto("01", "abc@abc", 10m);
            _svc.Cadastro(req);
            var result = _svc.RetornarProdutos();
            Assert.True(result.FoiSucesso);
        }



        private static string MontaRequestProduto(string? nome, string? vendedorEmail, decimal? valor)
        {
            var produto = new ProdutoRequest() { Nome = nome, EmailVendedor = vendedorEmail, MelhorLance = valor };

            return produto.ToString();
        }
    }
}