using System.Collections.Generic;
using Api_Macoratti.Models;

namespace Api_Macoratti.Repository
{
    public interface IProdutoRepository : IRepository<Produto>
    {
         IEnumerable<Produto> GetProdutosPorPreco();
    }
}