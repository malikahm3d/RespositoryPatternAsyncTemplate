using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Template.DAL.AppDbContext;

namespace Template.DAL.BaseRepository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<T> _dbSet;
        private readonly IQueryable<T> _dbGet;

        public BaseRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
            _dbGet = _dbSet.AsNoTracking();
        }

        // READ operations
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbGet.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetByPredicateAsync(Expression<Func<T, bool>> predicate)
        {
            var entity = await _dbGet.Where(predicate).ToListAsync();
            if (entity.Count > 0)
            {
                return entity;
            }
            return null;

        }

        public async Task<int> GetCountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbGet.CountAsync();
        }

        public async Task<IEnumerable<T>> GetPagedAsync(int? pageSize, int? pageNumber, Expression<Func<T, bool>>? predicate = null, Expression<Func<T, object>>? orderBy = null, bool? isDescending = false)
        {
            var query = _dbSet.AsNoTracking();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (orderBy != null)
            {
                if (isDescending.Value)
                {
                    query = query.OrderByDescending(orderBy);
                }
                else
                {
                    query = query.OrderBy(orderBy);
                }
            }

            if (pageNumber.HasValue && pageSize.HasValue)
            {
                var skip = (pageNumber.Value - 1) * pageSize.Value;
                return await query.Skip(skip).Take(pageSize.Value).ToListAsync();
            }

            return await query.ToListAsync();
        }

        //INSERT operations

        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            return entities;
        }

        public async Task<bool> AddAsync(T entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
                return true;
            }
            catch (Exception e)
            {

                return false;
            }
        }

        public bool Update(T entity)
        {
            try
            {
                _dbSet.Update(entity);
                return true;
            }
            catch (Exception e)
            {

                return false;
            }
        }
    }
}
