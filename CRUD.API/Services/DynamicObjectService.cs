using CRUD.API.Data;
using CRUD.API.Services;
using CRUD.API.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRUD.API.Services
{
    public class DynamicObjectService : IDynamicObjectService
    {
        private readonly AppDbContext _context;

        public DynamicObjectService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DynamicObject> CreateAsync(string objectType, Dictionary<string, object> fields)
        {
            objectType = objectType.ToLowerInvariant();
            ValidateFields(objectType, fields);

            var dynamicObject = new DynamicObject
            {
                ObjectType = objectType,
                Data = JsonConvert.SerializeObject(fields)
            };
            _context.DynamicObjects.Add(dynamicObject);
            await _context.SaveChangesAsync();
            return dynamicObject;
        }

        public async Task<DynamicObject> GetByIdAsync(int id)
        {
            return await _context.DynamicObjects.FindAsync(id);
        }

        public async Task<List<DynamicObject>> GetAllByTypeAsync(string objectType)
        {
            objectType = objectType.ToLowerInvariant();
            return await _context.DynamicObjects.Where(x => x.ObjectType == objectType).ToListAsync();
        }

        public async Task UpdateAsync(int id, Dictionary<string, object> fields)
        {
            var dynamicObject = await _context.DynamicObjects.FindAsync(id);
            if (dynamicObject != null)
            {
                ValidateFields(dynamicObject.ObjectType.ToLowerInvariant(), fields);
                dynamicObject.Data = JsonConvert.SerializeObject(fields);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var masterObject = await _context.DynamicObjects.FindAsync(id);
            if (masterObject != null)
            {
                var masterData = JsonConvert.DeserializeObject<Dictionary<string, object>>(masterObject.Data);

                string idKey = masterData.Keys.FirstOrDefault(k => k.ToLowerInvariant().EndsWith("id"));

                if (idKey != null && masterData.TryGetValue(idKey, out var objectId))
                {
                    var relatedObjects = await _context.DynamicObjects
                                                       .Where(x => x.Data.Contains($"\"ParentId\":{objectId}"))
                                                       .ToListAsync();

                    _context.DynamicObjects.RemoveRange(relatedObjects);
                }

                _context.DynamicObjects.Remove(masterObject);
                await _context.SaveChangesAsync();
            }
        }




        public async Task<bool> CreateTransactionAsync(DynamicObject masterObject, List<DynamicObject> subObjects)
        {
            masterObject.ObjectType = masterObject.ObjectType.ToLowerInvariant();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.DynamicObjects.Add(masterObject);
                await _context.SaveChangesAsync();

                foreach (var subObject in subObjects)
                {
                    subObject.ObjectType = subObject.ObjectType.ToLowerInvariant();
                    _context.DynamicObjects.Add(subObject);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
        public async Task<List<DynamicObject>> GetAllByTypeWithFiltersAsync(string objectType, Dictionary<string, string> filters)
        {
            objectType = objectType.ToLowerInvariant();

            var query = _context.DynamicObjects.Where(x => x.ObjectType == objectType);

            foreach (var filter in filters)
            {
                query = query.Where(x => x.Data.Contains($"\"{filter.Key}\":\"{filter.Value}\"") || x.Data.Contains($"\"{filter.Key}\":{filter.Value}"));
            }

            return await query.ToListAsync();
        }

        private void ValidateFields(string objectType, Dictionary<string, object> fields)
        {
            if (objectType == "product")
            {
                if (!fields.ContainsKey("ProductId") || !fields.ContainsKey("Name") || !fields.ContainsKey("Price"))
                {
                    throw new ArgumentException("Product must have ProductId, Name, and Price fields.");
                }
            }
            else if (objectType == "customer")
            {
                if (!fields.ContainsKey("CustomerId") || !fields.ContainsKey("FirstName") || !fields.ContainsKey("LastName") || !fields.ContainsKey("Email"))
                {
                    throw new ArgumentException("Customer must have CustomerId, FirstName, LastName, and Email fields.");
                }
            }
            //else
            //{
            //throw new ArgumentException($"Unknown objectType: {objectType}");
            //}
            // Validations based on object types can continue to be listed in this section.
        }
    }

}
