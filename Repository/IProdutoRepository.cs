using System.Collections.Generic;
using System.Threading.Tasks;
using Api_Macoratti.Models;
using Api_Macoratti.Pagination;

namespace Api_Macoratti.Repository
{
    public interface IProdutoRepository : IRepository<Produto>
    {
        Task<PagedList<Produto>> GetProdutos(ProdutosParameters produtosParameters);
        Task<IEnumerable<Produto>> GetProdutosPorPreco();
    }
}