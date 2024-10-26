using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Service.Service.Base
{
    public interface IBaseService<T> where T : class
    {


        Task<int> AddAsync(T entity);
        Task<bool> UpdateAsync(T entity);


        bool Delete(int id);

        Task<bool> DeleteAsync(int id);

        Task<bool> DeleteAsync(T entity);

        Task<T?> GetByIdAsync(int id);

        Task<IEnumerable<T>> GetAllAsync();


    }
}

