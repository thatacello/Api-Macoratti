using System.Collections.Generic;
using Api_Macoratti.Models;
using Api_Macoratti.Pagination;

namespace Api_Macoratti.Repository
{
    public interface IProdutoRepository : IRepository<Produto>
    {
        IEnumerable<Produto> GetProdutos(ProdutosParameters produtosParameters);
         IEnumerable<Produto> GetProdutosPorPreco();
    }
}