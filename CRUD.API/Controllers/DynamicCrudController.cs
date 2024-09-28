using CRUD.API.DTOs;
using CRUD.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRUD.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DynamicCrudController : ControllerBase
    {
        private readonly IDynamicCrudService _dynamicCrudService;

        public DynamicCrudController(IDynamicCrudService dynamicCrudService)
        {
            _dynamicCrudService = dynamicCrudService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateDynamicObject([FromBody] DynamicObjectDto dto)
        {
            var result = await _dynamicCrudService.CreateAsync(dto.ObjectType, dto.Fields);

            // Başarılı işlemi kontrol et
            if (result is not null && !(result is string))
            {
                return Ok(result);
            }

            return StatusCode(500, result);
        }

        [HttpGet("{objectType}/{id}")]
        public async Task<IActionResult> GetById(string objectType, int id)
        {
            try
            {
                var result = await _dynamicCrudService.GetByIdAsync(objectType, id);
                if (result == null) return NotFound(new { message = $"Object of type {objectType} with ID {id} not found." });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string objectType, int id, [FromBody] Dictionary<string, object> fields)
        {
            try
            {
                await _dynamicCrudService.UpdateAsync(objectType, id, fields);
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
        public async Task<IActionResult> Delete(string objectType, int id)
        {
            try
            {
                await _dynamicCrudService.DeleteAsync(objectType, id);
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
        [HttpGet("{objectType}")]
        public async Task<IActionResult> GetAll([FromRoute] string objectType, [FromQuery] Dictionary<string, string> filters)
        {
            try
            {
                var result = await _dynamicCrudService.GetAllAsync(objectType, filters);
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
    }
}
