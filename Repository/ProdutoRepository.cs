using System.Linq;
using System.Collections.Generic;
using Api_Macoratti.Context;
using Api_Macoratti.Models;
using Api_Macoratti.Pagination;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Api_Macoratti.Repository
{
    public class ProdutoRepository : Repository<Produto>, IProdutoRepository
    {
        public ProdutoRepository(AppDbContext contexto) : base(contexto)
        {
        }
        public async Task<PagedList<Produto>> GetProdutos(ProdutosParameters produtosParameters)
        {
            // return Get().OrderBy(on => on.Nome)
            //             .Skip((produtosParameters.PageNumber - 1) * produtosParameters.PageSize)
            //             .Take(produtosParameters.PageSize)
            //             .ToList();
            return await PagedList<Produto>.ToPagedList(Get().OrderBy(on => on.ProdutoId),
                produtosParameters.PageNumber, produtosParameters.PageSize);
        }

        public async Task<IEnumerable<Produto>> GetProdutosPorPreco()
        {
            return await Get().OrderBy(c => c.Preco).ToListAsync();
        }
    }
}