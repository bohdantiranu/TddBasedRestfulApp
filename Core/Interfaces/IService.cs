using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IService<T>
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T> GetByIdAsync(int id);

        Task AddAsync(T dto);

        Task DeleteByIdAsync(int id);

        Task<IEnumerable<T>> FindAsync(Predicate<T> predicate);

        Task<T> FindOneAsync(Predicate<T> predicate);
    }
}