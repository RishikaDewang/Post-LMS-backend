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
    public class DesignationController : Controller
    {
        private readonly AveryBitLms10Context _context;

        public DesignationController(AveryBitLms10Context context)
        {
            _context = context;
        }

        //Get All Designation 
        [Authorize(Roles = "Admin,User")]
        [HttpGet]

        public async Task<ActionResult<IEnumerable<Designation>>> GetAllDesignations()
        {
            try
            {
                var designation = await _context.Designations.ToListAsync();
                return Ok(designation);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        //GET Designation BY ID 
        [Authorize(Roles = "Admin,User")]

        [HttpGet("{id}")]
        public async Task<ActionResult<Designation>> GetDesignation(int id)
        {
            try
            {
                var designation = await _context.Designations.FindAsync(id);

                if (designation == null)
                {
                    return NotFound("Designation not found");
                }

                return Ok(designation);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        //Add Designation 
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddRole([FromBody] Designation designation)
        {
            try
            {
                if (designation == null)
                {
                    return BadRequest("Invalid designation data");
                }

                // Set created date and modified date
                designation.CreatedDate = DateTime.Now;
                designation.ModifiedDate = DateTime.Now;

                // Set default values
                designation.IsActive = true;

                _context.Designations.Add(designation);
                _context.SaveChanges();

                return Ok("designation added successfully");
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


        //update Designation API
        [Authorize(Roles = "Admin")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Put(int id, Designation designationData)
        {
            try
            {
                // Check if the request body or role ID is invalid, return BadRequest if so
                if (designationData == null || designationData.Id == 0 || id != designationData.Id)
                    return BadRequest();

                // Find the course in the database by ID, return NotFound if not found
                var designation = await _context.Designations.FindAsync(id);
                if (designation == null)
                    return NotFound();

                // Update role properties with values from the request body
                designation.DesignationName = designationData.DesignationName;
                designation.Description = designationData.Description;

                // Set default values
                designation.CreatedDate = designation.CreatedDate == DateTime.MinValue ? DateTime.Now : designation.CreatedDate;
                designation.ModifiedDate = DateTime.Now;
                designation.IsActive = true; // Assuming you want IsActive to be always true on update

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

        //Delete Designation API
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDesignation(int id)
        {
            try
            {
                var designation = await _context.Designations.FindAsync(id);

                if (designation == null)
                {
                    return NotFound(); // Return 404 if course not found
                }

                _context.Designations.Remove(designation);
                await _context.SaveChangesAsync();

                var responseMessage = new
                {
                    message = "Designation deleted successfully"
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
