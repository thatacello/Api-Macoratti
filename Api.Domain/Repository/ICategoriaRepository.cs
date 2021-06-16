using System.Collections.Generic;
using System.Threading.Tasks;
using Api_Macoratti.Models;
using Api_Macoratti.Pagination;

namespace Api_Macoratti.Repository
{
    public interface ICategoriaRepository : IRepository<Categoria>
    {
        Task<PagedList<Categoria>> GetCategorias(CategoriasParameters categoriasParameters);
        Task<IEnumerable<Categoria>> GetCategoriasProdutos();
    }
}