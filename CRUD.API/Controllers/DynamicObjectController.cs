using CRUD.API.DTOs;
using CRUD.API.Models;
using CRUD.API.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CRUD.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DynamicObjectController : ControllerBase
    {
        private readonly IDynamicObjectService _dynamicObjectService;

        public DynamicObjectController(IDynamicObjectService dynamicObjectService)
        {
            _dynamicObjectService = dynamicObjectService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DynamicObjectDto dto)
        {
            try
            {
                var result = await _dynamicObjectService.CreateAsync(dto.ObjectType, dto.Fields);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _dynamicObjectService.GetByIdAsync(id);
                if (result == null) return NotFound(new { message = $"DynamicObject with ID {id} not found." });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpGet("type/{objectType}")]
        public async Task<IActionResult> GetByType(string objectType)
        {
            try
            {
                var result = await _dynamicObjectService.GetAllByTypeAsync(objectType);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Dictionary<string, object> fields)
        {
            try
            {
                await _dynamicObjectService.UpdateAsync(id, fields);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _dynamicObjectService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpPost("transaction")]
        public async Task<IActionResult> CreateTransaction([FromBody] TransactionDto dto)
        {
            try
            {
                string masterIdKey = dto.MasterFields.Keys.FirstOrDefault(k => k.ToLowerInvariant().EndsWith("id"));

                if (masterIdKey == null)
                {
                    return BadRequest(new { message = $"Master object must contain an ID field (e.g., ProductId, CustomerId, etc.)." });
                }

                var masterId = dto.MasterFields[masterIdKey];

                var masterObject = new DynamicObject
                {
                    ObjectType = dto.MasterObjectType.ToLowerInvariant(),
                    Data = JsonConvert.SerializeObject(dto.MasterFields)
                };

                var subObjects = new List<DynamicObject>();
                foreach (var subObjectDto in dto.SubObjects)
                {
                    if (!subObjectDto.Fields.ContainsKey("ParentId"))
                    {
                        return BadRequest(new { message = "Each child object must contain a ParentId field associated with the id field of the Master object." });
                    }

                    var parentId = subObjectDto.Fields["ParentId"];
                    if (!parentId.Equals(masterId))
                    {
                        return BadRequest(new { message = $"Sub-object ParentId ({parentId}) must match master object id ({masterId})." });
                    }

                    subObjects.Add(new DynamicObject
                    {
                        ObjectType = subObjectDto.ObjectType.ToLowerInvariant(),
                        Data = JsonConvert.SerializeObject(subObjectDto.Fields)
                    });
                }

                var success = await _dynamicObjectService.CreateTransactionAsync(masterObject, subObjects);
                if (success) return Ok("Transaction successful");

                return BadRequest("Transaction failed");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }


        [HttpPost("type/{objectType}/filters")]
        public async Task<IActionResult> GetByTypeWithFiltersPost(string objectType, [FromBody] Dictionary<string, string> filters)
        {
            try
            {
                var result = await _dynamicObjectService.GetAllByTypeWithFiltersAsync(objectType, filters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }


    }


}
