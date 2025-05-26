using A3.Gestao.Servidor;
using A3.Gestao.Servidor.Repositories;
using A3.Gestao.Servidor.Services;

IProdutoRepository produtoRepo = new ProdutoRepository();
var leilaoSv = new LeilaoService(produtoRepo);
var servidor = new TcpServer(leilaoSv);
servidor.StartAsync();