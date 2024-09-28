using CRUD.API.Data;
using CRUD.API.DTOs;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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

        public async Task<object> CreateAsync(string objectType, Dictionary<string, object> fields)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                ValidateFields(objectType, fields);

                var mainObject = CreateObjectFromFields(objectType, fields);
                _context.Add(mainObject);
                await _context.SaveChangesAsync();

                var mainObjectId = GetObjectId(mainObject);

                if (fields.ContainsKey("SubObjects") && fields["SubObjects"] is IEnumerable<object> subObjects)
                {
                    foreach (var subObjectDto in subObjects)
                    {
                        if (subObjectDto is JObject subObject)
                        {
                            var subObjectType = subObject["objectType"]?.ToString();
                            var subObjectFields = subObject["fields"]?.ToObject<Dictionary<string, object>>();

                            if (subObjectType != null && subObjectFields != null)
                            {
                                ValidateFields(subObjectType, subObjectFields);

                                LinkParentToChild(mainObject, subObjectFields);

                                var subObjectInstance = CreateObjectFromFields(subObjectType, subObjectFields);
                                _context.Add(subObjectInstance);
                            }
                            else
                            {
                                throw new Exception($"Invalid sub-object format for type {subObjectType}.");
                            }
                        }
                        else
                        {
                            throw new Exception("SubObjects must be a valid JSON object.");
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return mainObject;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new { message = "An unexpected error occurred.", details = ex.Message };
            }
        }


        private void LinkParentToChild(object parentObject, Dictionary<string, object> childFields)
        {
            var parentObjectType = parentObject.GetType();
            var parentIdProperty = parentObjectType.GetProperties().FirstOrDefault(p => p.Name.EndsWith("Id"));
            if (parentIdProperty != null)
            {
                var parentIdValue = parentIdProperty.GetValue(parentObject);
                var parentIdName = parentIdProperty.Name;

                foreach (var key in childFields.Keys.ToList())
                {
                    if (key.Equals(parentIdName, StringComparison.OrdinalIgnoreCase) || key.Contains(parentObjectType.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        childFields[key] = parentIdValue;
                        return;
                    }
                }

                childFields[parentIdName] = parentIdValue;
            }
        }

        private int GetObjectId(object obj)
        {
            var property = obj.GetType().GetProperty($"{obj.GetType().Name}Id");
            if (property == null)
                throw new InvalidOperationException("ID property not found.");

            return (int)property.GetValue(obj);
        }


        private object CreateObjectFromFields(string objectType, Dictionary<string, object> fields)
        {
            var modelName =  objectType;

            var availableModels = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.Namespace == "CRUD.API.Models" && t.IsClass)
                .Select(t => t.Name)
                .ToList();

            if (!availableModels.Contains(modelName))
            {
                throw new Exception($"Invalid object type '{objectType}'. Available object types are: {string.Join(", ", availableModels)}");
            }

            var modelType = Type.GetType($"CRUD.API.Models.{modelName}");
            var modelInstance = Activator.CreateInstance(modelType);

            foreach (var field in fields)
            {
                var property = modelType.GetProperty(field.Key);
                if (property != null)
                {
                    property.SetValue(modelInstance, Convert.ChangeType(field.Value, property.PropertyType));
                }
            }

            return modelInstance;
        }


        public async Task<object> GetByIdAsync(string objectType, int id)
        {
            try
            {
                var modelName = char.ToUpper(objectType[0]) + objectType.Substring(1);
                var modelType = Type.GetType($"CRUD.API.Models.{modelName}");
                if (modelType == null)
                    throw new ArgumentException($"Model for objectType '{objectType}' not found.");

                var dbSet = _context.GetType().GetProperty(modelName + "s")?.GetValue(_context) as IQueryable<object>;
                if (dbSet == null)
                    throw new ArgumentException($"DbSet for '{modelName}' not found in DbContext.");

                var keyProperty = $"{modelName}Id";
                var result = await dbSet.FirstOrDefaultAsync(x => EF.Property<int>(x, keyProperty) == id);

                if (result == null)
                    throw new KeyNotFoundException($"Entity of type {objectType} with ID {id} not found.");

                return result;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                return new { message = "Invalid request.", details = ex.Message };
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
                return new { message = "Not Found.", details = ex.Message };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new { message = "An unexpected error occurred.", details = ex.Message };
            }
        }


        public async Task<object> UpdateAsync(string objectType, int id, Dictionary<string, object> fields)
        {
            try
            {
                var entity = await GetByIdAsync(objectType, id);
                if (entity == null)
                    throw new KeyNotFoundException($"Object of type {objectType} with ID {id} not found.");

                var modelType = entity.GetType();
                foreach (var field in fields)
                {
                    var property = modelType.GetProperty(field.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (property != null)
                    {
                        var convertedValue = Convert.ChangeType(field.Value, property.PropertyType);
                        property.SetValue(entity, convertedValue);
                    }
                }

                _context.Update(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
                return new { message = "Update failed.", details = ex.Message };
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                return new { message = "Invalid data provided.", details = ex.Message };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new { message = "An unexpected error occurred while updating.", details = ex.Message };
            }
        }


        public async Task DeleteAsync(string objectType, int id)
        {
            try
            {
                var modelName = char.ToUpper(objectType[0]) + objectType.Substring(1);
                var modelType = Type.GetType($"CRUD.API.Models.{modelName}");
                if (modelType == null)
                    throw new ArgumentException($"Model for objectType '{objectType}' not found.");

                var dbSet = _context.GetType().GetProperty(modelName + "s")?.GetValue(_context) as IQueryable<object>;
                if (dbSet == null)
                    throw new ArgumentException($"DbSet for '{modelName}' not found in DbContext.");

                var keyProperty = $"{modelName}Id";
                var entity = await dbSet.FirstOrDefaultAsync(x => EF.Property<int>(x, keyProperty) == id);
                if (entity == null)
                    throw new KeyNotFoundException($"Entity with {keyProperty}={id} not found.");

                _context.Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("Invalid request.", ex);
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("Delete failed. Entity not found.", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("An unexpected error occurred during deletion.", ex);
            }
        }

        private void ValidateFields(string objectType, Dictionary<string, object> fields)
        {
            var requiredFields = new Dictionary<string, List<string>>
            {
                { "product", new List<string> { "ProductName", "ProductDescription", "Price" } },
                { "customer", new List<string> { "FirstName", "LastName", "Email", "Phone", "Address" } },
                { "order", new List<string> { "CustomerId", "OrderDate", "TotalAmount", "Status" } },
                { "orderproduct", new List<string> {  "ProductId", "Quantity", "Price" } }
            };

            if (requiredFields.ContainsKey(objectType.ToLower()))
            {
                var missingFields = requiredFields[objectType.ToLower()].Where(field => !fields.ContainsKey(field)).ToList();
                if (missingFields.Any())
                {
                    throw new Exception($"Missing required fields for {objectType}: {string.Join(", ", missingFields)}");
                }
            }
        }
        public async Task<IEnumerable<object>> GetAllAsync(string objectType, Dictionary<string, string> filters)
        {
            var modelName = char.ToUpper(objectType[0]) + objectType.Substring(1);
            var modelType = Type.GetType($"CRUD.API.Models.{modelName}");
            if (modelType == null)
                throw new ArgumentException($"Model for objectType '{objectType}' not found.");

            var dbSetProperty = _context.GetType().GetProperties()
                .FirstOrDefault(p => p.PropertyType.IsGenericType &&
                                     p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>) &&
                                     p.PropertyType.GenericTypeArguments.Contains(modelType));

            if (dbSetProperty == null)
                throw new ArgumentException($"DbSet for '{modelName}' not found in DbContext.");

            var dbSet = dbSetProperty.GetValue(_context) as IQueryable<object>;
            if (dbSet == null)
                throw new ArgumentException($"Unable to retrieve DbSet for '{modelName}'.");

            if (filters != null && filters.Any())
            {
                foreach (var filter in filters)
                {
                    var propertyInfo = modelType.GetProperty(filter.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (propertyInfo == null)
                    {
                        continue;
                    }

                    var parameter = Expression.Parameter(modelType, "x");
                    var member = Expression.Property(parameter, propertyInfo);
                    var constant = Expression.Constant(Convert.ChangeType(filter.Value, propertyInfo.PropertyType));
                    var predicate = Expression.Equal(member, constant);
                    var lambda = Expression.Lambda(predicate, parameter);

                    var methodName = "Where";
                    var method = typeof(Queryable).GetMethods()
                        .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                        .MakeGenericMethod(modelType);

                    dbSet = (IQueryable<object>)method.Invoke(null, new object[] { dbSet, lambda });
                }
            }

            return await dbSet.ToListAsync();
        }



    }
}
