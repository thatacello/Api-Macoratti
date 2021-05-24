using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api_Macoratti.Context;
using Api_Macoratti.Models;
using Api_Macoratti.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Api_Macoratti.Repository
{
    public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(AppDbContext contexto) : base(contexto)
        {
        }
        public async Task<PagedList<Categoria>> GetCategorias(CategoriasParameters categoriasParameters)
        {
            return await PagedList<Categoria>.ToPagedList(Get().OrderBy(on => on.Nome), 
                categoriasParameters.PageNumber, categoriasParameters.PageSize);
        }
        public async Task<IEnumerable<Categoria>> GetCategoriasProdutos()
        {
            return await Get().Include(x => x.Produtos).ToListAsync();
        }
    }
}