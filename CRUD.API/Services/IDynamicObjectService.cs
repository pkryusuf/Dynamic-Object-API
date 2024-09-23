using CRUD.API.Models;

namespace CRUD.API.Services
{
    public interface IDynamicObjectService
{
    Task<DynamicObject> CreateAsync(string objectType, Dictionary<string, object> fields);
    Task<DynamicObject> GetByIdAsync(int id);
    Task<List<DynamicObject>> GetAllByTypeAsync(string objectType);
    Task UpdateAsync(int id, Dictionary<string, object> fields);
    Task DeleteAsync(int id);
    Task<bool> CreateTransactionAsync(DynamicObject masterObject, List<DynamicObject> subObjects);
    Task<List<DynamicObject>> GetAllByTypeWithFiltersAsync(string objectType, Dictionary<string, string> filters);

    }
}
