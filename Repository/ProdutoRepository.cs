using System.Linq;
using System.Collections.Generic;
using Api_Macoratti.Context;
using Api_Macoratti.Models;

namespace Api_Macoratti.Repository
{
    public class ProdutoRepository : Repository<Produto>, IProdutoRepository
    {
        public ProdutoRepository(AppDbContext contexto) : base(contexto)
        {
        }

        public IEnumerable<Produto> GetProdutosPorPreco()
        {
            return Get().OrderBy(c => c.Preco).ToList();
        }
    }
}