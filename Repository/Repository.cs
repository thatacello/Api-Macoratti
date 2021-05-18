using System;
using System.Linq;
using System.Linq.Expressions;
using Api_Macoratti.Context;
using Microsoft.EntityFrameworkCore;

namespace Api_Macoratti.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected AppDbContext _context; 
        public Repository(AppDbContext contexto)
        {
            _context = contexto;
        }
        public IQueryable<T> Get() // lista de entidades
        {
            return _context.Set<T>().AsNoTracking();
        }
        public T GetById(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().SingleOrDefault(predicate);
        }
        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }
        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            _context.Set<T>().Update(entity);
        }
    }
}