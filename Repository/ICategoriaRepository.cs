using System.Collections.Generic;
using Api_Macoratti.Models;

namespace Api_Macoratti.Repository
{
    public interface ICategoriaRepository : IRepository<Categoria>
    {
         IEnumerable<Categoria> GetCategoriasProdutos();
    }
}