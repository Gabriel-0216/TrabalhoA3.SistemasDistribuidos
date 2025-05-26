using A3.Gestao.Servidor.Models;

namespace A3.Gestao.Servidor.Repositories
{
    public interface IProdutoRepository
    {
        Produto? Buscar(int id);
        void Remover(int id);
        void Adicionar(Produto produto);
        IList<Produto> Listar();
        IList<Produto> ListarFinalizados();
    }
}
