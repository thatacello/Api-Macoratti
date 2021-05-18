using System.Linq;
using System.Collections.Generic;
using Api_Macoratti.Context;
using Api_Macoratti.Models;
using Api_Macoratti.Pagination;

namespace Api_Macoratti.Repository
{
    public class ProdutoRepository : Repository<Produto>, IProdutoRepository
    {
        public ProdutoRepository(AppDbContext contexto) : base(contexto)
        {
        }
        public IEnumerable<Produto> GetProdutos(ProdutosParameters produtosParameters)
        {
            return Get().OrderBy(on => on.Nome)
                        .Skip((produtosParameters.PageNumber - 1) * produtosParameters.PageSize)
                        .Take(produtosParameters.PageSize)
                        .ToList();
        }

        public IEnumerable<Produto> GetProdutosPorPreco()
        {
            return Get().OrderBy(c => c.Preco).ToList();
        }
    }
}