using CRUD.API.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRUD.API.Services
{
    public interface IDynamicCrudService
    {
        Task<object> CreateAsync(string objectType, Dictionary<string, object> fields);

        Task<object> GetByIdAsync(string objectType, int id);

        Task<object> UpdateAsync(string objectType, int id, Dictionary<string, object> fields);

        Task DeleteAsync(string objectType, int id);
        Task<IEnumerable<object>> GetAllAsync(string objectType, Dictionary<string, string> filters);

    }

}
