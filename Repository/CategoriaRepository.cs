using System.Collections.Generic;
using System.Linq;
using Api_Macoratti.Context;
using Api_Macoratti.Models;
using Microsoft.EntityFrameworkCore;

namespace Api_Macoratti.Repository
{
    public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(AppDbContext contexto) : base(contexto)
        {
        }

        public IEnumerable<Categoria> GetCategoriasProdutos()
        {
            return Get().Include(x => x.Produtos).ToList();
        }
    }
}