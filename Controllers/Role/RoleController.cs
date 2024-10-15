using LMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Globalization;

namespace LMS.Controllers
{
    // Requires authorization for all actions in this controller
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : Controller
    {
        private readonly AveryBitLms10Context _context;

        public RoleController(AveryBitLms10Context context)
        {
            _context = context;
        }

        //Get All Roles 
        [Authorize(Roles = "Admin,User")]
        [HttpGet]

        public async Task<ActionResult<IEnumerable<Role>>> GetAllRoles()
        {
            try
            {    
                var roles= await _context.Roles.ToListAsync();
                return Ok(roles); 
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        //GET role  BY ID 
        [Authorize(Roles = "Admin,User")]

        [HttpGet("{id}")]
        public async Task<ActionResult<Role>> GetRole(int id)
        {
            try
            {
                var role = await _context.Roles.FindAsync(id);

                if (role == null)
                {
                    return NotFound("Role not found");
                }

                return Ok(role);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        //Add Role 
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddRole([FromBody] Role role)
        {
            try
            {
                if (role == null)
                {
                    return BadRequest("Invalid role data");
                }

                // Set created date and modified date
                role.CreatedDate = DateTime.Now;
                role.ModifiedDate = DateTime.Now;

                // Set default values
                role.IsActive = true;
     
                _context.Roles.Add(role);
                _context.SaveChanges();

                return Ok("Role added successfully");
            }
            
            catch (HttpRequestException httpRequestException)
            {
                // Log the exception or handle issues with HTTP requests
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in HTTP request: " + httpRequestException.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }
        //update Role API
        [Authorize(Roles = "Admin")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Put(int id, Role roleData)
        {
            try
            {
                // Check if the request body or role ID is invalid, return BadRequest if so
                if (roleData == null || roleData.Id == 0 || id != roleData.Id)
                    return BadRequest();

                // Find the course in the database by ID, return NotFound if not found
                var role = await _context.Roles.FindAsync(id);
                if (role == null)
                    return NotFound();

                // Update role properties with values from the request body
                role.RoleName = roleData.RoleName;
                role.Description = roleData.Description;
               
                // Set default values
                role.CreatedDate = role.CreatedDate == DateTime.MinValue ? DateTime.Now : role.CreatedDate;
                role.ModifiedDate =  DateTime.Now;
                role.IsActive = true; // Assuming you want IsActive to be always true on update

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Successful update, return OK response
                return Ok();
            }
            catch (HttpRequestException httpRequestException)
            {
                // Log the exception or handle issues with HTTP requests
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in HTTP request: " + httpRequestException.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        //Delete Role API
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            try
            {
                var role = await _context.Roles.FindAsync(id);

                if (role == null)
                {
                    return NotFound(); // Return 404 if course not found
                }

                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();

                var responseMessage = new
                {
                    message = "Role deleted successfully"
                };

                return Ok(responseMessage); // Return 200 OK with success message
            }
            catch (HttpRequestException httpRequestException)
            {
                // Log the exception or handle issues with HTTP requests
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in HTTP request: " + httpRequestException.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }





    }

}
