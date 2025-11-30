using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AirBB.Models;

namespace AirBB.Models.DataLayer.Repositories
{
    public class Repository<T> : IGenericRepository<T> where T : class
    {
        private readonly AirBBContext _context;

        public Repository(AirBBContext context)
        {
            _context = context;
        }

        public IQueryable<T> Query()
        {
            return _context.Set<T>().AsQueryable();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(AirBB.Models.DataLayer.QueryOptions<T>? options = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (options != null)
            {
                if (options.Includes != null)
                {
                    foreach (var include in options.Includes)
                    {
                        query = query.Include(include);
                    }
                }

                if (options.Filter != null)
                {
                    query = query.Where(options.Filter);
                }

                if (options.OrderBy != null)
                {
                    query = options.OrderBy(query);
                }

                if (options.Skip.HasValue)
                {
                    query = query.Skip(options.Skip.Value);
                }

                if (options.Take.HasValue)
                {
                    query = query.Take(options.Take.Value);
                }
            }

            return await query.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(params object[] keyValues)
        {
            var entity = await _context.Set<T>().FindAsync(keyValues);
            return entity == null ? null : entity;
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().AnyAsync(predicate);
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
