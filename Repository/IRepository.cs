using System.Linq.Expressions;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace Api_Macoratti.Repository
{
    public interface IRepository<T> // interface genérica
    {
         IQueryable<T> Get();
         Task<T> GetById(Expression<Func<T, bool>> predicate);
         void Add(T entity);
         void Update(T entity);
         void Delete(T entity);
    }
}