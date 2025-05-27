using A3.Gestao.Servidor.Models;

namespace A3.Gestao.Servidor.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        public List<Produto> produtos;
        public ProdutoRepository()
        {
            produtos = [];
        }
        public void Adicionar(Produto produto)
        {
            var ultimo = produtos.LastOrDefault();
            produto.Id = ultimo is null ? 0 : ultimo.Id + 1;
            produtos.Add(produto);
        }
        public Produto? Buscar(int id) => produtos.FirstOrDefault(p => p.Id == id);
        public IList<Produto> Listar() => [.. produtos];
        public IList<Produto> ListarFinalizados() => produtos.Where(x => x.Finalizado).ToList();
        public void Remover(int id)
        {
            var produto = Buscar(id);
            if(produto is not null)
                produtos.Remove(produto);
        }
    }
}
