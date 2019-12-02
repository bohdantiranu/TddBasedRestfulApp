using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryServices
{
    public abstract class ServiceBase<T, U, URepo> : IService<T> where URepo : IRepository<U> where T : class
    {
        protected URepo Repository { get; }

        protected ServiceBase(URepo repository)
        {
            Repository = repository;
        }

        public async Task AddAsync(T dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            await Repository.AddAsync(CastToDbType(dto));
            await Repository.SaveAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            if (id < 1)
                throw new InvalidOperationException("Specified id doesn't exist");

            var entityToRemove = await Repository.GetByIdAsync(id);
            if (entityToRemove == null)
                throw new InvalidOperationException("Specified id doesn't exist");

            await Repository.DeleteAsync(await Repository.GetByIdAsync(id));
            await Repository.SaveAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Predicate<T> predicate)
        {
            var returnList = new List<T>();
            foreach (var entity in await Repository.GetAllAsync())
            {
                var dto = CastToDto(entity);
                if (predicate(dto))
                    returnList.Add(dto);
            }

            return returnList;
        }

        public async Task<T> FindOneAsync(Predicate<T> predicate)
        {
            foreach (var entity in await Repository.GetAllAsync())
            {
                var dto = CastToDto(entity);
                if (predicate(dto))
                    return dto;
            }

            return null;
        }

        public async Task<IEnumerable<T>> GetAllAsync() =>
            (await Repository.GetAllAsync()).Select(entity => CastToDto(entity)).ToList();

        public async Task<T> GetByIdAsync(int id)
        {
            if (id < 1)
                throw new InvalidOperationException("Specified id less then 1");

            return CastToDto(await Repository.GetByIdAsync(id));
        }

        protected abstract T CastToDto(U dbTypeEntity);
        protected abstract U CastToDbType(T dto);
    }
}