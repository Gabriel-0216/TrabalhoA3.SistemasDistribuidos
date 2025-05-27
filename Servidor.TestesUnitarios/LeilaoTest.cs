using A3.Gestao.Servidor.Repositories;
using A3.Gestao.Servidor.Services;
using Servidor.TestesUnitarios.Requests;

namespace Servidor.TestesUnitarios
{
    public class LeilaoTest
    {
        public LeilaoService _svc;

        public LeilaoTest()
        {
            _svc = new(new ProdutoRepository());
        }


        [Fact]
        public void LanceValido()
        {
            var idCadastro = CadastraProdutoValido();

            var lanceReq = MontaRequestLance(idCadastro, 12, "teste@comprador");
            var resp = _svc.DarLance(lanceReq);
            Assert.True(resp.FoiSucesso);

        }
        [Fact]
        public void LanceInvalidoValor()
        {
            var idCadastro = CadastraProdutoValido();

            var lanceReq = MontaRequestLance(idCadastro, 10, "teste@comprador");
            var resp = _svc.DarLance(lanceReq);
            Assert.False(resp.FoiSucesso);
        }
        [Fact]
        public void LanceInvalidoEmail()
        {
            var idCadastro = CadastraProdutoValido();

            var lanceReq = MontaRequestLance(idCadastro, 15, "");
            var resp = _svc.DarLance(lanceReq);
            Assert.False(resp.FoiSucesso);
        }
        [Fact]
        public void LanceInvalidoId()
        {
            var idCadastro = CadastraProdutoValido();

            idCadastro++;
            var lanceReq = MontaRequestLance(idCadastro, 15, "");
            var resp = _svc.DarLance(lanceReq);
            Assert.False(resp.FoiSucesso);
        }

        [Fact]
        public void FinalizaLeilaoSemLance()
        {
            var idCadastro = CadastraProdutoValido();

            var finaliza = MontaRequestFinalizar(idCadastro, "produto@produto");
            var resp = _svc.FinalizarLeilao(finaliza);
            Assert.True(resp.FoiSucesso);
        }
        [Fact]
        public void FinalizaLeilaoLance()
        {
            var idCadastro = CadastraProdutoValido();

            var lanceReq = MontaRequestLance(idCadastro, 15, "1bcd@abc");
            _svc.DarLance(lanceReq);
            var finaliza = MontaRequestFinalizar(idCadastro, "produto@produto");
            var resp = _svc.FinalizarLeilao(finaliza);
            Assert.True(resp.FoiSucesso);
        }







        private int CadastraProdutoValido()
        {
            return _svc.Cadastro(MontaRequestProduto("Produto", "produto@produto", 10m)).Content.Id;
        }

        private static string MontaRequestProduto(string? nome, string? vendedorEmail, decimal? valor)
        {
            return new ProdutoRequest() { Nome = nome, EmailVendedor = vendedorEmail, MelhorLance = valor }.ToString();
        }
        private static string MontaRequestLance(int idProduto, int valor, string emailComprador)
        {
            return new LanceRequest() { IdProduto = idProduto, Lance = valor, EmailComprador = emailComprador }.ToString();
        }
        private static string MontaRequestFinalizar(int idProduto, string emailVendedor)
        {
            return new FinalizarLeilaoRequest() { Id = idProduto, Email = emailVendedor }.ToString();
        }
    }
}
