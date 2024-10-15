using LMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace LMS.Controllers.Courses
{
    // Requires authorization for all actions in this controller
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly AveryBitLms10Context _context;

        public CourseController(AveryBitLms10Context context)
        {
            _context = context;
        }
        // GET api/course
        // Requires "Admin" or "User" role for access
        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<IEnumerable<Course>> Get()
        {
            var courses = await _context.Courses
                .Select(c => new Course
                {
                    ID = c.ID,
                    Title = c.Title,
                    Description = c.Description,
                    VideoLink = c.VideoLink,
                    CourseDuration = c.CourseDuration,
                    Keywords = c.Keywords,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    CreatedDate = c.CreatedDate,
                    ModifiedDate = c.ModifiedDate,
                    IsActive = c.IsActive,
                    IsDeleted = c.IsDeleted,
                    DeletedDate = c.DeletedDate,
                    PdfUpload = c.PdfUpload,
                    FeaturedImage = c.FeaturedImage
                })
                .ToListAsync();

            return courses;
        }



        // GET api/course/{id}
        // Requires "Admin" or "User" role for access
        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {

                // Validate the input ID
                if (id < 1)
                    return BadRequest();

                // Retrieve the course with the specified ID from the database
                var course = await _context.Courses.FirstOrDefaultAsync(m => m.ID == id);
                // Check if the course exists
                if (course == null)
                    return NotFound();

                // Return the course in the response
                return Ok(course);
            }
            catch (InvalidOperationException invalidOperationException)
            {
                // Log the exception or handle Invalid method call
                return HandleInvalidOperationException(invalidOperationException);
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
        private IActionResult HandleInvalidOperationException(InvalidOperationException ex)
        {
            // Specific handling for InvalidOperationException
            return StatusCode(StatusCodes.Status500InternalServerError, "Invalid operation: " + ex.Message);
        }



        //ADD Course API
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddCourse([FromBody] Course course)
        {
            try
            {
                if (course == null)
                {
                    return BadRequest("Invalid course data");
                }

                // Set created date and modified date
                course.CreatedDate = DateTime.Now;
                course.ModifiedDate = DateTime.Now;

                // Set default values
                course.CourseDuration = course.CourseDuration == default ? "4:00:00" : course.CourseDuration;
                course.Keywords = string.IsNullOrEmpty(course.Keywords) ? GetStartingLetters(course.Title, 2) : course.Keywords;
                course.StartDate = course.CreatedDate;
                course.IsActive = true;
                course.IsDeleted = false;
                course.PdfUpload = "not available";
                course.DeletedDate = DateTime.Now.AddYears(100);

                // Set end date as 100 years from now
                course.EndDate = DateTime.Now.AddYears(100);

                _context.Courses.Add(course);
                _context.SaveChanges();

                return Ok("Course added successfully");
            }
            catch (InvalidOperationException invalidOperationException)
            {
                // Log the exception or handle Invalid method call
                return HandleInvalidOperationException(invalidOperationException);
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
       

        // Helper method to get starting letters of a string
        private string GetStartingLetters(string input, int count)
        {
            if (string.IsNullOrEmpty(input) || count <= 0)
            {
                return string.Empty;
            }

            return input.Substring(0, Math.Min(count, input.Length));
        }

        //Update Course API
        [Authorize(Roles = "Admin")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Put(int id, Course courseData)
        {
            try
            {
                // Check if the request body or course ID is invalid, return BadRequest if so
                if (courseData == null || courseData.ID == 0 || id != courseData.ID)
                    return BadRequest();

                // Find the course in the database by ID, return NotFound if not found
                var course = await _context.Courses.FindAsync(id);
                if (course == null)
                    return NotFound();

                // Update course properties with values from the request body
                course.Title = courseData.Title;
                course.Description = courseData.Description;
                course.VideoLink = courseData.VideoLink;
                course.CourseDuration = courseData.CourseDuration;
                course.Keywords = courseData.Keywords;
                course.PdfUpload = courseData.PdfUpload;
                course.FeaturedImage = courseData.FeaturedImage;

                // Set default values
                course.CourseDuration = course.CourseDuration == default ? "4:00:00" : course.CourseDuration;
                course.Keywords = string.IsNullOrEmpty(course.Keywords) ? GetStartingLetters(course.Title, 2) : course.Keywords;
                course.PdfUpload = "not available";



                // Save changes to the database
                await _context.SaveChangesAsync();

                // Successful update, return OK response
                return Ok();
            }
            catch (InvalidOperationException invalidOperationException)
            {
                // Log the exception or handle Invalid method call
                return HandleInvalidOperationException(invalidOperationException);
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
        


        //Delete Course API
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            try
            {
                var course = await _context.Courses.FindAsync(id);

                if (course == null)
                {
                    return NotFound(); // Return 404 if course not found
                }

                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();

                var responseMessage = new
                {
                    message = "Course deleted successfully"
                };

                return Ok(responseMessage); // Return 200 OK with success message
            }
            catch (InvalidOperationException invalidOperationException)
            {
                // Log the exception or handle Invalid method call
                return HandleInvalidOperationException(invalidOperationException);
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

