using Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        protected DbContext Context { get; }

        protected BaseRepository(DbContext context)
        {
            Context = context;
        }

        public async Task<IEnumerable<T>> GetAllAsync() =>
            await Task<IEnumerable<T>>.Factory.StartNew(() => Context.Set<T>());

        public async Task<T> GetByIdAsync(int id) => await Context.Set<T>().FindAsync(id);

        public async Task AddAsync(T entity) => await Context.Set<T>().AddAsync(entity);

        public async Task DeleteAsync(T entity) => await Task.Factory.StartNew(() => Context.Set<T>().Remove(entity));

        public async Task SaveAsync() => await Context.SaveChangesAsync();
    }
}