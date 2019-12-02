using Core.Interfaces;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        protected GroupsContext Context { get; }

        protected BaseRepository(GroupsContext context)
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