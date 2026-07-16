using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Data.Repository
{
    public abstract class BaseRepository<T> where T : class
    {
        protected readonly CurrencyConverterContext _context;
        protected readonly DbSet<T> _dbSet;

        protected BaseRepository(CurrencyConverterContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public virtual void Add(T entity) => _dbSet.Add(entity);

        public virtual void Update(T entity) => _dbSet.Update(entity);

        public virtual void Delete(T entity) => _dbSet.Remove(entity);

        public virtual IQueryable<T> Query() => _dbSet.AsQueryable();

        public virtual void SaveChanges() => _context.SaveChanges();
    }
}
