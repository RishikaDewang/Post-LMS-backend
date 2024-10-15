using LMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Globalization;
using System.Linq;

namespace LMS.Controllers.Courses
{
    // Requires authorization for all actions in this controller
     [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingPlanController : ControllerBase
    {
        private readonly AveryBitLms10Context _context;

        public TrainingPlanController(AveryBitLms10Context context)
        {
            _context = context;
        }
        //GET ALL Training Plans 
        [Authorize(Roles = "Admin,User")]
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<TrainingPlan>>> GetTrainingPlans()
        {
            try
            {
                // Retrieve all training plans from the database
                var trainingPlans = await _context.TrainingPlans.ToListAsync();

                // Check if any training plans were found
                if (trainingPlans == null || trainingPlans.Count == 0)
                {
                    // Return 404 Not Found status if no training plans were found
                    return NotFound("No training plans found.");
                }

                // Return the list of training plans
                return trainingPlans;
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal Server Error: {ex.Message}");
            }
        }

        //GET ALL Training Plans By ID 
        [Authorize(Roles = "Admin,User")]
        [HttpGet("training-plan-courses/{id}")]
        public IActionResult GetTrainingPlanCourses(int id)
        {
            try
            {
                // Find the training plan by ID including its associated courses
                var trainingPlan = _context.TrainingPlans
                    .Include(tp => tp.TrainingPlanCourses)
                        .ThenInclude(tpc => tpc.FkCourse)
                    .FirstOrDefault(tp => tp.Id == id);

                if (trainingPlan == null)
                {
                    return NotFound("Training Plan not found");
                }

                // Shape the response directly in the API action
                var response = new
                {
                    Id = trainingPlan.Id,
                    trainingPlanName = trainingPlan.TrainingPlanName,
                    description = trainingPlan.Description,
                    featuredImage = trainingPlan.FeaturedImage,
                    trainingPlanCourses = trainingPlan.TrainingPlanCourses
                        .Select(tpc => new
                        {
                            FkCourseId = tpc.FkCourseId,
                            courseTitle = tpc.FkCourse?.Title,
                            courseDescription = tpc.FkCourse?.Description,
                            cideoLink = tpc.FkCourse?.VideoLink,
                            featuredImage = tpc.FkCourse?.FeaturedImage
                            // Add more properties as needed
                        })
                        .ToList()
                };

                var jsonOptions = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                    MaxDepth = 32
                };

                var jsonString = JsonSerializer.Serialize(response, jsonOptions);

                return Content(jsonString, "application/json");
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

        //ADD Training PLAN API
        [Authorize(Roles = "Admin")]  
        [HttpPost]
        public IActionResult AddTrainingPlanWithCourses([FromBody] TrainingPlan newTrainingPlan)
        {
            try
            {
                // Validate the incoming data
                if (newTrainingPlan == null)
                {
                    return BadRequest("Invalid request body");
                }
                // Set default values
                newTrainingPlan.IsActive = true;
                newTrainingPlan.PlannedDurationDays = 60; // or PlannedDurationDays = 100 if it should be 100 by default

                // Add the new training plan to the context
                _context.TrainingPlans.Add(newTrainingPlan);

                // Add the new training plan to the context
                _context.TrainingPlans.Add(newTrainingPlan);

                // Create a new list to store the modified TrainingPlanCourses
                var modifiedTrainingPlanCourses = new List<TrainingPlanCourse>();

                // Add courses to the training plan
                if (newTrainingPlan.TrainingPlanCourses != null && newTrainingPlan.TrainingPlanCourses.Any())
                {
                    foreach (var trainingPlanCourse in newTrainingPlan.TrainingPlanCourses)
                    {
                        var existingCourse = _context.Courses.Find(trainingPlanCourse.FkCourseId);

                        if (existingCourse != null)
                        {
                            // Create a new instance of TrainingPlanCourse
                            var newTrainingPlanCourse = new TrainingPlanCourse
                            {
                                FkCourseId = existingCourse.ID,
                                FkPlanId = newTrainingPlan.Id,
                            };

                            // Add the new TrainingPlanCourse to the list
                            modifiedTrainingPlanCourses.Add(newTrainingPlanCourse);
                        }
                        else
                        {
                            // Handle invalid course ID
                            return BadRequest($"Course with ID {trainingPlanCourse.FkCourseId} not found");
                        }
                    }

                    // Convert ICollection<TrainingPlanCourse> to List<TrainingPlanCourse>
                    var trainingPlanCoursesList = newTrainingPlan.TrainingPlanCourses.ToList();

                    // Add the modified TrainingPlanCourses to the TrainingPlan after the iteration
                    trainingPlanCoursesList.AddRange(modifiedTrainingPlanCourses);

                    // Assign the modified list back to the TrainingPlan
                    newTrainingPlan.TrainingPlanCourses = trainingPlanCoursesList;
                }

                // Save changes to the database
                _context.SaveChanges();

                // Return the newly created training plan if needed
                return Ok(newTrainingPlan);
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
     

        //ADD Courses to Training plan 
        public class TrainingPlanCourseRequest
        {
            public int TrainingPlanId { get; set; }
            public List<int> CourseIds { get; set; }
        }

       [Authorize(Roles = "Admin")]
       [HttpPost("add-training-plan-courses")]
        public IActionResult AddTrainingPlanWithCourses([FromBody] TrainingPlanCourseRequest requestBody)
        {
            try
            {
                // Validate the incoming data
                int trainingPlanId = requestBody?.TrainingPlanId ?? 0;
                List<int> courseIds = requestBody?.CourseIds;

                if (trainingPlanId <= 0 || courseIds == null || !courseIds.Any())
                {
                    return BadRequest("Invalid request parameters");
                }

                // Find the existing training plan
                var existingTrainingPlan = _context.TrainingPlans.Find(trainingPlanId);

                if (existingTrainingPlan == null)
                {
                    return BadRequest($"Training plan with ID {trainingPlanId} not found");
                }

                // Create a new list to store the modified TrainingPlanCourses
                var modifiedTrainingPlanCourses = new List<TrainingPlanCourse>();

                foreach (var courseId in courseIds)
                {
                    var existingCourse = _context.Courses.Find(courseId);

                    if (existingCourse != null)
                    {
                        // Create a new instance of TrainingPlanCourse
                        var newTrainingPlanCourse = new TrainingPlanCourse
                        {
                            FkCourseId = existingCourse.ID,
                            FkPlanId = existingTrainingPlan.Id,
                            // Set other properties as needed
                        };

                        // Add the new TrainingPlanCourse to the list
                        modifiedTrainingPlanCourses.Add(newTrainingPlanCourse);
                    }
                    else
                    {
                        // Handle invalid course ID
                        return BadRequest($"Course with ID {courseId} not found");
                    }
                }

                // Convert ICollection<TrainingPlanCourse> to List<TrainingPlanCourse>
                var trainingPlanCoursesList = existingTrainingPlan.TrainingPlanCourses.ToList();

                // Add the modified TrainingPlanCourses to the TrainingPlan after the iteration
                trainingPlanCoursesList.AddRange(modifiedTrainingPlanCourses);

                // Assign the modified list back to the TrainingPlan
                existingTrainingPlan.TrainingPlanCourses = trainingPlanCoursesList;

                // Save changes to the database
                _context.SaveChanges();

                // Return a success message
                return Ok("Training plan courses added successfully");
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
     

        //Update Training Plan API
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTrainingPlan(int id, [FromBody] TrainingPlan updatedTrainingPlan)
        {
            try
            {
                if (id != updatedTrainingPlan.Id)
                {
                    return BadRequest("Invalid ID in request data.");
                }

                var existingTrainingPlan = await _context.TrainingPlans
                    .Include(tp => tp.TrainingPlanCourses)
                    .FirstOrDefaultAsync(tp => tp.Id == id);

                if (existingTrainingPlan == null)
                {
                    return NotFound("TrainingPlan not found.");
                }

                // Update TrainingPlan properties
                existingTrainingPlan.TrainingPlanName = updatedTrainingPlan.TrainingPlanName;
                existingTrainingPlan.Description = updatedTrainingPlan.Description;
                existingTrainingPlan.FeaturedImage = updatedTrainingPlan.FeaturedImage;
                
                //Set Default Values 
                existingTrainingPlan.IsActive = true;
                existingTrainingPlan.PlannedDurationDays = 60;

                // Detach existing TrainingPlanCourses
                foreach (var trainingPlanCourse in existingTrainingPlan.TrainingPlanCourses)
                {
                    _context.Entry(trainingPlanCourse).State = EntityState.Detached;
                }

                // Update TrainingPlanCourses
                // Clear existing courses and add the updated courses
                existingTrainingPlan.TrainingPlanCourses.Clear();
                foreach (var updatedCourse in updatedTrainingPlan.TrainingPlanCourses)
                {
                    var existingCourse = _context.TrainingPlanCourses.Find(updatedCourse.Id);
                    if (existingCourse != null)
                    {
                        existingCourse.FkPlanId = updatedCourse.FkPlanId;
                        existingCourse.FkCourseId = updatedCourse.FkCourseId;
                        // Update other properties as needed
                        existingTrainingPlan.TrainingPlanCourses.Add(existingCourse);
                    }
                    else
                    {
                        // If the course doesn't exist in the context, add it as a new course
                        _context.TrainingPlanCourses.Attach(updatedCourse); // Attach new course
                        existingTrainingPlan.TrainingPlanCourses.Add(updatedCourse);
                    }
                }

                await _context.SaveChangesAsync();
                return Ok("TrainingPlan and TrainingPlanCourses updated successfully.");
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
       

     //Delete Training Plan API
     [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTrainingPlan(int id)
    {
        try
        {
             var trainingPlan = await _context.TrainingPlans
            .Include(tp => tp.DesignationTrainingPlans)
            .Include(tp => tp.TrainingPlanCourses)
            .FirstOrDefaultAsync(tp => tp.Id == id);

        if (trainingPlan == null)
        {
            return NotFound();
        }

        // Remove associated DesignationTrainingPlans
        _context.DesignationTrainingPlans.RemoveRange(trainingPlan.DesignationTrainingPlans);

        // Remove associated TrainingPlanCourses
        _context.TrainingPlanCourses.RemoveRange(trainingPlan.TrainingPlanCourses);

        // Remove the TrainingPlan
        _context.TrainingPlans.Remove(trainingPlan);

        await _context.SaveChangesAsync();

         var response = new
         {
             message = "TrainingPlan deleted successfully."
         };

            // Configure JSON serialization options
            var jsonOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            MaxDepth = 32 // Set the maximum depth if needed, default is 32
        };

        // Serialize the deleted TrainingPlan to JSON
        var jsonString = JsonSerializer.Serialize(trainingPlan, jsonOptions);

        // Return the serialized TrainingPlan as content
        return Content(jsonString, "application/json");
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


        //GET Assigned Training Plans API
        [Authorize(Roles = "Admin,User")]
        [HttpGet("GetUserTrainingPlan/{userId}")]
        public IActionResult GetUserTrainingPlan(int userId)
        {
            try
            {
                var allTrainingPlans = _context.TrainingPlans
                    .Include(tp => tp.TrainingPlanCourses)
                    .ToList()  // Materialize the query to a list
                    .Select(tp => new
                    {
                        trainingPlanId = tp.Id,
                        trainingPlanName = tp.TrainingPlanName,
                        featuredImage = tp.FeaturedImage,
                        description = tp.Description,
                        statusTP = _context.UserAssignCourses
                            .Any(uac => uac.FkTrainingplanId == tp.Id && uac.FkUserId == userId),
                        trainingPlanCourses = tp.TrainingPlanCourses
                            .Select(tpc => new
                            {
                                courseId = tpc.FkCourseId,
                                status = _context.AssignedTrainingPlanCourseStatus
                                    .Where(atpcs => atpcs.id == _context.UserAssignCourses
                                        .Where(u => u.FkCourseId == tpc.FkCourseId && u.FkUserId == userId && u.FkTrainingplanId == tp.Id)
                                        .Select(uac => uac.assigncoursestatus_id)
                                        .FirstOrDefault())
                                    .Select(atpcs => atpcs.status_name)
                                    .FirstOrDefault(),
                                priority = _context.AssignedTrainingPlanCoursePriority
                                    .Where(atpcs => atpcs.id == _context.UserAssignCourses
                                        .Where(u => u.FkCourseId == tpc.FkCourseId && u.FkUserId == userId && u.FkTrainingplanId == tp.Id)
                                        .Select(uac => uac.assigncoursepriority_id)
                                        .FirstOrDefault())
                                    .Select(atpcs => atpcs.priority_name)
                                    .FirstOrDefault(),
                                courseTitle = _context.Courses
                                    .Where(c => c.ID == tpc.FkCourseId)
                                    .Select(c => c.Title)
                                    .FirstOrDefault()
                            })
                            .DistinctBy(tpi => tpi.courseId)
                            .ToList()
                    })
                    .ToList();

                return Ok(allTrainingPlans);
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





        //Assign and remove TP to user 
        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateStatusTP/{userId}")]
        public IActionResult UpdateUserTrainingPlans(int userId, [FromBody] List<TrainingPlanAssignment> trainingPlanAssignments)
        {
            try
            {
                var user = _context.Users
                    .Include(u => u.UserAssignCourses)
                    .FirstOrDefault(u => u.Id == userId);

                if (user == null)
                {
                    return NotFound($"User with ID {userId} not found.");
                }

                var messages = new List<string>();

                foreach (var assignment in trainingPlanAssignments)
                {
                    var trainingPlanId = assignment.TrainingPlanId;
                    var status = assignment.StatusTP;

                    // Check if the training plan is already assigned to the user
                    var existingAssignment = user.UserAssignCourses.FirstOrDefault(uac => uac.FkTrainingplanId == trainingPlanId);

                    if (status)
                    {
                        // Assign the training plan and associated courses to the user
                        if (existingAssignment == null)
                        {
                            // Get the training plan
                            var trainingPlan = _context.TrainingPlans
                                .Include(tp => tp.TrainingPlanCourses)
                                .FirstOrDefault(tp => tp.Id == trainingPlanId);

                            if (trainingPlan != null)
                            {
                                // Check if any courses are already assigned
                                var assignedCourses = trainingPlan.TrainingPlanCourses
                                    .Where(tpc => user.UserAssignCourses.Any(uac => uac.FkCourseId == tpc.FkCourseId))
                                    .ToList();

                                if (assignedCourses.Count == 0)
                                {
                                    // Assign all courses in the training plan to the user
                                    var newAssignments = trainingPlan.TrainingPlanCourses
                                        .Select(tpc => new UserAssignCourse
                                        {
                                            FkUserId = userId,
                                            FkTrainingplanId = trainingPlanId,
                                            FkCourseId = tpc.FkCourseId,
                                            DateAssigned = DateTime.UtcNow,
                                            DateCompleted = new DateTime(2050, 1, 1),
                                            assigncoursestatus_id = 1,
                                            assigncoursepriority_id = 1

                                        })
                                        .ToList();

                                    _context.UserAssignCourses.AddRange(newAssignments);
                                    messages.Add($"Training Plan {trainingPlanId} assigned successfully.");
                                }
                                else
                                {
                                    messages.Add($"Some courses in Training Plan {trainingPlanId} are already assigned to the user.");
                                }
                            }
                        }
                        else
                        {
                            messages.Add($"Training Plan {trainingPlanId} is already assigned to the user.");
                        }
                    }
                    else
                    {
                        // Remove the training plan and associated courses from the user
                        var existingAssignments = user.UserAssignCourses
                            .Where(uac => uac.FkTrainingplanId == trainingPlanId)
                            .ToList();

                        if (existingAssignments.Any())
                        {
                            _context.UserAssignCourses.RemoveRange(existingAssignments);
                            messages.Add($"Training Plan {trainingPlanId} and associated courses removed successfully.");
                        }
                        else
                        {
                            messages.Add($"Training Plan {trainingPlanId} is not assigned to the user.");
                        }
                    }
                }

                // Save changes to the database
                _context.SaveChanges();

                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        public class TrainingPlanAssignment
        {
            public int TrainingPlanId { get; set; }
            public bool StatusTP { get; set; }
        }

        // GET Progress API for one training plan on the basic of completed course in that training plan 
        [Authorize(Roles = "Admin,User")]
        [HttpGet("GetProgressTP/{userId}")]
        public IActionResult GetProgressTP(int userId)
        {
            try
            {
                var user = _context.Users
                    .Include(u => u.UserAssignCourses)
                    .ThenInclude(uac => uac.FkTrainingPlan)
                    .ThenInclude(tp => tp.TrainingPlanCourses)
                    .FirstOrDefault(u => u.Id == userId);

                if (user == null)
                {
                    return NotFound($"User with ID {userId} not found.");
                }

                var trainingPlanInfo = user.UserAssignCourses
                    .Select(uac => new
                    {
                        trainingPlanId = uac.FkTrainingPlan?.Id,
                        trainingPlanName = uac.FkTrainingPlan?.TrainingPlanName,
                        progressPercentage = CalculateProgressPercentage(uac.FkTrainingPlan?.TrainingPlanCourses)
                    })
                    .DistinctBy(tpi => tpi.trainingPlanId)  // Use DistinctBy to get unique training plans
                    .ToList();

                return Ok(trainingPlanInfo);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, "Internal server error");
            }
        }
        private double CalculateProgressPercentage(IEnumerable<TrainingPlanCourse> courses)
        {
            if (courses == null || !courses.Any())
            {
                return 0.0; // Return 0 if no courses are available
            }

            int totalCourses = courses.Count();
            DateTime cutoffDate = DateTime.UtcNow.AddDays(-1); 

            int completedCourses = courses.Count(tpc =>
                _context.UserAssignCourses.Any(uac =>
                    uac.FkCourseId == tpc.FkCourseId &&
                    uac.assigncoursestatus_id == 3 &&
                    uac.DateAssigned > cutoffDate));

            double progressPercentage = (double)completedCourses / totalCourses * 100;

            // Format progressPercentage to two decimal places
            string formattedProgress = progressPercentage.ToString("F2");

            // Parse the formatted string back to a double
            if (double.TryParse(formattedProgress, out double result))
            {
                return result;
            }

            return 0.0; // Return 0 if parsing fails
        }

      //  Assign TP to USER by selecting user id and TP ID so that
     //   All the courses associated with TP assign to user ADD API
        [Authorize(Roles = "Admin")]
        [HttpPost("AssignTrainingPlanToUser")]
        public IActionResult AssignTrainingPlanToUser([FromBody] UserAssignCourse request)
        {
            try
            {
                var user = _context.Users.Include(u => u.UserAssignCourses).FirstOrDefault(u => u.Id == request.FkUserId);
                var trainingPlan = _context.TrainingPlans.Include(tp => tp.TrainingPlanCourses).FirstOrDefault(tp => tp.Id == request.FkTrainingplanId);

                if (user == null || trainingPlan == null)
                {
                    return NotFound("User or Training Plan not found.");
                }

                // Check if the user already has the training plan assigned
                if (user.UserAssignCourses.Any(uac => uac.FkTrainingplanId == request.FkTrainingplanId))
                {
                    return BadRequest("User already has this Training Plan assigned.");
                }

                // Create a new UserAssignCourse entry for the user and assign the Training Plan
                var userAssignCourse = new UserAssignCourse
                {
                    FkUserId = user.Id,
                    FkTrainingplanId = trainingPlan.Id
                };

                _context.UserAssignCourses.Add(userAssignCourse);
                _context.SaveChanges();

                // Associate all courses from the Training Plan to the user

                foreach (var trainingPlanCourse in trainingPlan.TrainingPlanCourses)
                {
                    // Skip the iteration if FkCourseId is NULL
                    if (trainingPlanCourse.FkCourseId == null)
                    {
                        continue;
                    }

                    // Check if the course is not already assigned to the user
                    var isCourseAssigned = user.UserAssignCourses
                        .Any(uac => uac.FkCourseId == trainingPlanCourse.FkCourseId);

                    if (!isCourseAssigned)
                    {
                        var userCourse = new UserAssignCourse
                        {
                            FkUserId = user.Id,
                            FkCourseId = trainingPlanCourse.FkCourseId,
                            FkTrainingplanId = trainingPlan.Id,
                            DateAssigned = DateTime.UtcNow,
                            Status = false

                        };

                        userCourse.DateCompleted = (bool)userCourse.Status ? DateTime.UtcNow : new DateTime(2050, 1, 1); // Use any appropriate default date


                        _context.UserAssignCourses.Add(userCourse);


                    }
                }

                _context.SaveChanges();

                return Ok("Training Plan assigned to the user successfully.");
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
       
        //Update Assigned Training plan courses
        [Authorize(Roles = "Admin")]
        [HttpPut("update-assigned-training-plan/{userId}")]
        public async Task<IActionResult> UpdateAssignedTrainingPlan(int userId, [FromBody] List<UserAssignCourse> updateTrainingPlans)
        {
            try
            {
                if (userId < 1)
                {
                    return BadRequest("Invalid user ID");
                }

                foreach (var updatePlan in updateTrainingPlans)
                {
                    var userAssignments = await (
                        from assign in _context.UserAssignCourses
                        where assign.FkUserId == userId && assign.FkTrainingplanId == updatePlan.FkTrainingplanId
                        select assign
                    ).ToListAsync();

                    foreach (var userAssignment in userAssignments)
                    {
                        // Update properties in the UserAssignCourses entity based on the UpdateUserAssignCourseModel
                        userAssignment.Status = updatePlan.Status;
                        userAssignment.FkTrainingplanId = updatePlan.FkTrainingplanId;
                        userAssignment.FkCourseId = updatePlan.FkCourseId;

                        // Update other properties as needed
                        // Automatically set DateAssigned to the current date and time
                        userAssignment.DateAssigned = DateTime.UtcNow;
                        userAssignment.DateCompleted = updatePlan.DateCompleted;
                    }
                }

                // Save changes to the database
                await _context.SaveChangesAsync();

                return Ok("Training plans updated successfully");
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

        //Update Assigned Training plan Status of courses
        [Authorize(Roles = "Admin,User")]
        [HttpPut("assigned-training-plan-status/{userId}")]
        public async Task<IActionResult> AssignedTrainingPlan(int userId, [FromBody] List<UserAssignCourse> updateTrainingPlans)
        {
            try
            {
                if (userId < 1)
                {
                    return BadRequest("Invalid user ID");
                }

                foreach (var updatePlan in updateTrainingPlans)
                {
                    var userAssignment = await (
                        from assign in _context.UserAssignCourses
                        where assign.FkUserId == userId
                              && assign.FkTrainingplanId == updatePlan.FkTrainingplanId
                              && assign.FkCourseId == updatePlan.FkCourseId // Add this condition
                        select assign
                    ).SingleOrDefaultAsync();

                    if (userAssignment != null)
                    {
                        // Update only the Status property if provided in the request
                        if (updatePlan.assigncoursestatus_id.HasValue)
                        {
                            userAssignment.assigncoursestatus_id = updatePlan.assigncoursestatus_id.Value;
                        }

                        // Update only the Priority property if provided in the request
                        if (updatePlan.assigncoursepriority_id.HasValue)
                        {
                            userAssignment.assigncoursepriority_id = updatePlan.assigncoursepriority_id.Value;
                        }
                    }
                }

                // Save changes to the database
                await _context.SaveChangesAsync();

                return Ok("Training plans updated successfully");
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







  

  
