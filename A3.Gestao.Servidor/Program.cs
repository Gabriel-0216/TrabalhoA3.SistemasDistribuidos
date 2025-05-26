using A3.Gestao.Servidor;
using A3.Gestao.Servidor.Models.Constante;
using A3.Gestao.Servidor.Repositories;
using A3.Gestao.Servidor.Services;

var delimitadores = new List<Delimitadores>
{
    new() { Nome = NomeDelimitador.NOME, Delimitador = '$' },
    new() { Nome = NomeDelimitador.ID_PRODUTO, Delimitador = '$' },
    new() { Nome = NomeDelimitador.EMAIL_VENDEDOR, Delimitador = '#' },
    new() { Nome = NomeDelimitador.VALOR_LANCE, Delimitador = '&' },
    new() { Nome = NomeDelimitador.EMAIL_COMPRADOR, Delimitador = '%' },
    new() { Nome = NomeDelimitador.DECIMAL, Delimitador = '%' },
    new() { Nome = NomeDelimitador.TEXTO, Delimitador = '&' }

};


IProdutoRepository produtoRepo = new ProdutoRepository();
var leilaoSv = new LeilaoService(produtoRepo, delimitadores);
var servidor = new TcpServer(leilaoSv);
servidor.StartAsync();