using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LMS.Models;
using System.Net.Mail;
using System.Net;
using System.Runtime.Intrinsics.X86;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace LMS.Controllers
{
    [Authorize] // Requires authorization for all actions in this controller
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AveryBitLms10Context _context;
        private readonly IConfiguration _configuration;
        private readonly string BaseURL;


        public UserController(AveryBitLms10Context context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            BaseURL = "http://10.0.0.185/lmsaverybit";

        }


        // LOGIN API  - Allows anonymous access for authentication
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User User)
        {

            // Validate email and password inputs
            if (string.IsNullOrEmpty(User.Email) || string.IsNullOrEmpty(User.Password))
            {
                return BadRequest("Email and password are required.");
            }

            // Additional email format validation
            if (!IsValidEmail(User.Email))
            {
                return BadRequest("Invalid email format.");
            }

            string email = User.Email;
            string password = User.Password;


            // Check if the provided email and password match a user in the database
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

           /* // Validate password complexity for existing users
            if (!IsPasswordValid(password))
            {
                return BadRequest("Invalid password format for existing users.");
            }*/

            // Check if the provided password matches the user's stored password
            if (user.Password != password)
            {
                return Unauthorized("Invalid email or password.");
            }

            // Retrieve the role associated with the authenticated user
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == user.FkRoleId);
            if (role == null)
            {
                // Return unauthorized status if the user's role is not found
                return Unauthorized();
            }

            // Create claims for the authenticated user
            var claims = new List<Claim>
            {
                new Claim("Id", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Name", user.Name),
                new Claim("RoleId", role.Id.ToString()),
                new Claim(ClaimTypes.Role, role.RoleName)   //Assuming role.RoleName contains the user's role(ADD Role Claim)
            };

            // Retrieve JWT configurations from appsettings.json
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            if (key is null)
            {
                // Return internal server error if JWT key configuration is invalid
                return StatusCode(StatusCodes.Status500InternalServerError, "Invalid JWT key configuration");
            }

            // Create a JWT token with the specified claims and configurations
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1), // Set the expiration time to 1 hour from now
                Issuer = issuer ?? string.Empty,
                Audience = audience ?? string.Empty,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);

            // Prepare the response containing user information and the generated JWT token
            var response = new
            {
                id = user.Id,
                email = user.Email,
                name = user.Name,
                roleId = role.Id,
                roleName = role.RoleName,
                token = jwtToken
            };
            // Return the response containing the user information and JWT token
            return Ok(response);
        }

        // Additional method to validate email format
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // Additional method to validate password complexity
        private bool IsPasswordValid(string password)
        {
            // Password must have at least 8 characters, including uppercase, lowercase, digit, and special character
            return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$");
        }


        //Validate TOKEN API
        [AllowAnonymous]
        [HttpPost("validateToken")]
        public IActionResult ValidateToken([FromBody] TokenRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
            {
                return BadRequest("Token is required.");
            }

            // Validate the provided token
            bool isValid = ValidateCurrentToken(request.Token);

            if (isValid)
            {
                return Ok(new { isValid = true });
            }
            else
            {
                return BadRequest("Invalid token.");
            }
        }

        private bool ValidateCurrentToken(string token)
        {
            // Your validation logic for the token...
            var mySecret = "ABCDeujujsik@!!!Ashsnskajuh";
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));

            var myIssuer = "https://lms.com/";
            var myAudience = "https://lms.com/";

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = myIssuer,
                    ValidAudience = myAudience,
                    IssuerSigningKey = mySecurityKey
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public class TokenRequest
        {
            public string Token { get; set; }
        }




        //GET USER API
        [Authorize(Roles = "Admin,User")]
        [HttpGet]

        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            try
            {
                // Filter users with fk_role_id not equal to 2
                //var users = await _context.Users.Where(u => u.FkRoleId != 2).ToListAsync();
                var users = await _context.Users.ToListAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }


        //GET USER BY ID 
        [Authorize(Roles = "Admin,User")]

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    return NotFound("User not found");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

   


        //Add New USER
        [Authorize(Roles = "Admin")]
        [HttpPost("add")]
        public async Task<IActionResult> AddUser(User userData)
        {
            try
            {
                // Check if the request body is invalid, return BadRequest if so
                if (userData == null)
                    return BadRequest("Invalid user data.");

                // Validate other user properties here
                if (string.IsNullOrWhiteSpace(userData.Name) || string.IsNullOrWhiteSpace(userData.Email))
                    return BadRequest("Name, email are required.");

                // Validate email format
                if (!IsValidEmail(userData.Email))
                    return BadRequest("Invalid email format.");

                // Check if the email is already present
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == userData.Email);
                if (existingUser != null)
                    return BadRequest("Email is already in use.");

                /*   // Check if the specified role and designation exist
                   var role = await _context.Roles.FindAsync(userData.FkRoleId);
                   var designation = await _context.Designations.FindAsync(userData.FkDesignationId);*/

                // Check if the specified role and designation exist
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == userData.RoleName);
                var designation = await _context.Designations.FirstOrDefaultAsync(d => d.DesignationName == userData.DesignationName);

                if (role == null || designation == null)
                    return BadRequest("Invalid role or designation.");


                if (role == null || designation == null)
                    return BadRequest("Invalid role or designation.");

                // Generate a random password
                string randomPassword = GenerateRandomPassword();

                // Set additional properties before adding the user to the database
                userData.CreatedDate = DateTime.Now;
                userData.IsActive = true;
                userData.ModifiedDate = DateTime.Now;  // Set default ModifiedDate
                userData.ModifiedBy = "admin";         // Set default ModifiedBy
                //userData.ProfilePic = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAN4AAACUCAMAAADLemePAAAAYFBMVEX///8AAADy8vKRkZH4+Pj8/Pzq6urJycmtra0gICC2trY6OjpZWVlfX1/X19eAgIC+vr7d3d1vb2+Hh4dRUVGcnJwUFBSioqINDQ3Pz88uLi5oaGh6enrj4+MmJiZCQkKJydWhAAADhUlEQVR4nO2cjXqrIAxAC4Jaa6k6tX+6vv9bTm8/t7pVBasN4eY8AecDkhCRzYYgCIIgCIIgCIIgiP8XKXiDkNDjWAHhpyo5eg3HRKW+gB7PkkhfhdmFfXPJQuW7MomyTqot+8W2StwQ5EH22+1OFnDosb2MTL3yuR1jpZcin0AZ7IfkWvYBaj8RXcbsmiATIY6hIhxcmN8hBq+fjP4EzGd+SNenDHbTdoztkO6/NNaxYyxOoUc6B/6hZ8fYB8L8JwNdO8YQLk8+mvD67NFNn8nkIZw+fjXRu2KbPmVix5iCHq8Z4mimd8RVu5gElhZkwaWeLDb7lDX0iI04m9kxdoYesQkyMtVDVVibRhZksUV4pnoeKr2Dqd4BlV5lqleh0tM+DHV8oNJzO7Q4nhgcT+ub+mRmd8JVlDleUjt+INqkZnrYeoGONyMcbyVtfJNGoA89WmMcb+M63oRvUnuuZ5fjSukdMtDqJ5W46rEfhFZhjauYfkTHD+/H52Z9JlN2Cdq5+8d59AP07ozbromfI12lA86Y2UMEAwkiDxBvuwd48uQSQZygTOZPEWlxe3S7FakbM/cDV6FXZVnlhcqdeSMIgiAIwkV4rc4NqubYT0E9pKwjL+5dq97GXlQ7IClFHQ72c/dhjfl3MNkcFCZui++8FKmh5JFWozOPEO5F6RefOnItnwWyn8EaOV23O6gEeWT4aZ2xU4TljCuU5seFPrlC0Z/wja/sdBT2f+aTSvPXmmfEyvIdKCa77uMkVi9QPnthdhQWRxjf+BrnXw7WbkDf6LLHEFdL/fwXgsojsZV+S9nZ6Wd2DWkc+y4pmV8OH8O6i+OGJfQUBbRPnxez+V8SaKNHUu2znS6fFl2BFIZ3b3XY27P9Ft54d6zZfqnx4VWH0pblucLSbNlDe91ZPGp2WBE9xW16oPO42RBdwlV2XssphHZbc/KsmL7pB3XmU4JPn5zV9NMlh24tBRrvBc1nGwDrVasFlpZTBWvHtZ5Dms8O9mCr85bVK2wjUL0FOxDPuULarb02gVdnMPFG3utcIGPn8OuNS1F6gHqLtTaHieHs1qw3OwDrTrV6ZGliC9wrUVOvby7BBS7zFSsn9ZYtXEspW7XgvHPKwPTeEDghQ+eqZ72OHEzvDXmhyQxQdu9Ie4CJb/2CugWsqOaF9waOYGcG/hag7AiCIAiCIAiCIAgCiC9NczDCtRzcHwAAAABJRU5ErkJggg==";
                userData.CreatedBy = "admin";
                userData.IsActive = true;
                userData.Password = randomPassword;

                userData.FkRole = role;
                userData.FkDesignation = designation;


                // Add the new user to the database
                _context.Users.Add(userData);
                await _context.SaveChangesAsync();

                // Set the generated password for the user
                userData.Password = randomPassword;

                // Send welcome email
                SendWelcomeEmail(userData.Email, "Welcome to LMS!", $"Please login with your Mail Id and Password : Your default password is: {randomPassword} Here is LMS link : http://10.0.0.185/LearningManagementSystem/#/");

                // Successful addition, return OK response
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
        private IActionResult HandleInvalidOperationException(InvalidOperationException ex)
        {
            // Specific handling for InvalidOperationException
            return StatusCode(StatusCodes.Status500InternalServerError, "Invalid operation: " + ex.Message);
        }

        private void SendWelcomeEmail(string toEmail, string subject, string body)
        {
            using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com"))
            {
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("rishika.dewang.averybit@gmail.com", "icuj fwlw vtkp ixzx");
                smtpClient.EnableSsl = true;
                smtpClient.Port = 587; // Use port 465 for SSL

                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress("rishika.dewang.averybit@gmail.com");
                    mailMessage.To.Add(toEmail);
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = false;

                    smtpClient.Send(mailMessage);
                }
            }
        }


        private string GenerateRandomPassword(int length = 8)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_-+=<>?";
            var random = new Random();

            var password = new char[length];
            for (int i = 0; i < length; i++)
            {
                password[i] = validChars[random.Next(0, validChars.Length)];
            }

            return new string(password);
        }


        //Update User API 
        [Authorize(Roles = "Admin")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser(int id, User userData)
        {
            try
            {
                // Check if the request body or user ID is invalid, return BadRequest if so
                if (userData == null || id == 0)
                    return BadRequest();

                // Find the user in the database by ID, return NotFound if not found
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                    return NotFound("User not found");

                // Check if the specified role and designation exist
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == userData.RoleName);
                var designation = await _context.Designations.FirstOrDefaultAsync(d => d.DesignationName == userData.DesignationName);

                if (role == null || designation == null)
                    return BadRequest("Invalid role or designation.");

                // Update user properties with values from the request body
                user.Name = userData.Name;
                user.Email = userData.Email;
                user.FkRole = role; // Update using the Role object
                user.FkDesignation = designation; // Update using the Designation object

              /*  // Update user properties with values from the request body
                user.Name = userData.Name;
                user.Email = userData.Email;
                user.FkRoleId = userData.FkRoleId;
                user.FkDesignationId = userData.FkDesignationId;
*/
                // Set default values
                user.CreatedDate = user.CreatedDate == DateTime.MinValue ? DateTime.Now : user.CreatedDate;
                user.CreatedBy = string.IsNullOrEmpty(user.CreatedBy) ? "admin" : user.CreatedBy;
                user.ModifiedBy = string.IsNullOrEmpty(user.ModifiedBy) ? "admin" : user.ModifiedBy; 
                user.IsActive = true; // Assuming you want IsActive to be always true on update



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
      
        //Update User Profile 
        [Authorize(Roles = "Admin ,User")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProfile(int id, [FromBody] User userData)
        {
            try
            {
                // Check if the request body or user ID is invalid, return BadRequest if so
                if (userData == null || id == 0)
                    return BadRequest();

                // Find the user in the database by ID, return NotFound if not found
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                    return NotFound("User not found");

                // Get the username of the currently authenticated user
                var currentUserName = HttpContext.User.Identity.Name;

                // Update specific fields without affecting others
                user.Name = userData.Name ?? user.Name; // Use the new value if provided, otherwise keep the existing value
                user.FkDesignationId = userData.FkDesignationId ?? user.FkDesignationId; // Use the new value if provided, otherwise keep the existing value
                user.Profile= userData.Profile?? user.Profile; // Use the new value if provided, otherwise keep the existing value

                // Set default values
                user.CreatedDate = user.CreatedDate == DateTime.MinValue ? DateTime.Now : user.CreatedDate;
                user.CreatedBy = string.IsNullOrEmpty(user.CreatedBy) ? "admin" : user.CreatedBy; 
                user.IsActive = true; // Assuming you want IsActive to be always true on update
                user.ModifiedDate = DateTime.Now;  // Set default ModifiedDate
                user.ModifiedBy = user.Name ?? user.Name; // Set ModifiedBy to the name of the authenticated user
                user.Email = user.Email ?? user.Email;
                user.Password = user.Password ?? user.Password;
                user.Profile = user.Profile ?? user.Profile;
            

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



        //Upload User Profile PIC 
        [HttpPost("UploadProfilePic")]
        public async Task<IActionResult> UploadImage(IFormFile ProfilePic, int Id)
        {
            try
            {
                if (ProfilePic == null || ProfilePic.Length == 0)
                {
                    return BadRequest("Invalid image file");
                }

                using (var memoryStream = new MemoryStream())
                {
                    await ProfilePic.CopyToAsync(memoryStream);
                    var imageBytes = memoryStream.ToArray();

                    var user = _context.Users.FirstOrDefault(e => e.Id == Id);

                    if (user == null)
                    {
                        return NotFound("User not found");
                    }

                    var imageId = Guid.NewGuid().ToString();
                    var imagePath = Path.Combine("wwwroot", "ProfileImages", $"{imageId}.jpg");
                    System.IO.File.WriteAllBytes(imagePath, imageBytes);

                    var baseUrl = "http://10.0.0.185/lmsaverybit";
                    user.Profile = $"{baseUrl}/ProfileImages/{imageId}.jpg";

                    await _context.SaveChangesAsync();

                    return Ok(new { Message = "Image uploaded successfully.", ImageUrl = user.Profile });
                }
            }
            catch (InvalidOperationException invalidOperationException)
            {
                // Log the exception or handle Invalid method call
                return HandleInvalidOperationException(invalidOperationException);
            }
            catch (DbUpdateException dbUpdateException)
            {
                // Log or return details of the inner exception
                var innerException = dbUpdateException.InnerException;
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating database: {innerException?.Message}");
            }
            catch (HttpRequestException httpRequestException)
            {
                // Log the exception or handle issues with HTTP requests
                return StatusCode(StatusCodes.Status500InternalServerError, "Error in HTTP request: " + httpRequestException.Message);
            }
            catch (Exception ex)
            {
                // Log or print the exception details
                Console.WriteLine(ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }


        //Get Profile PIC
        [Authorize(Roles = "Admin ,User")]
        [HttpGet("{Id}/getprofilePic")]
        public IActionResult GetUserImage(int Id)
        {
            try
            {
                // Get the user based on the provided ID
                var user = _context.Users.FirstOrDefault(e => e.Id == Id);
                if (user == null || user.Profile == null)
                {
                    return NotFound("User image not found");
                }
                // Return the image URL
                return Ok(new { ImageUrl = user.Profile });
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
      



        //Delete USER API 
        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                // Validate the input ID
                if (id < 1)
                    return BadRequest();

                // Find the user in the database by ID
                var user = await _context.Users.FindAsync(id);

                // Check if the user exists
                if (user == null)
                    return NotFound("User not found");

                // Remove the user from the database
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                // Successful deletion, return a custom success message
                return Ok("User deleted successfully!");
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
                // Log the exception for investigation (optional)
                Console.WriteLine($"Exception: {ex.Message}");

                // Return a custom success message for successful deletion
                return Ok("User deleted successfully!");
            }
        }




        //Update Password API
        [Authorize(Roles = "Admin,User")]
        [HttpPut("updatepassword/{id}")]
        public async Task<IActionResult> UpdateUserPassword(int id, [FromBody] PasswordUpdateRequest passwordUpdateRequest)
        {
            try
            {
                var existingUser = await _context.Users.FindAsync(id);
                if (existingUser == null)
                {
                    return NotFound();
                }

                // Validate the current password
                if (existingUser.Password != passwordUpdateRequest.CurrentPassword)
                {
                    return BadRequest("Invalid current password.");
                }

                // Validate the new password
                if (existingUser.Password == passwordUpdateRequest.NewPassword)
                {
                    return BadRequest("New password must be different from the current password.");
                }

                // Add validation for the new password
                if (!IsPasswordValid(passwordUpdateRequest.NewPassword))
                {
                    return BadRequest("Password must have at least 8 characters, including uppercase, lowercase, digit, and special character.");
                }


                // Update the user's password
                existingUser.Password = passwordUpdateRequest.NewPassword;

                await _context.SaveChangesAsync();

                return Ok("User password successfully updated.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExistsById(id))
                {
                    return NotFound();
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Database concurrency error occurred.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        private bool UserExistsById(int id)
        {
            return _context.Users.Any(u => u.Id == id);
        }

        public class PasswordUpdateRequest
        {
            public string? CurrentPassword { get; set; }
            public string? NewPassword { get; set; }
        }

        //Count of total Users , courses , Training plans  
        [Authorize(Roles = "Admin,User")]
        [HttpGet("count")]
        public async Task<IActionResult> GetCounts()
        {
            try
            {
                var userCount = await _context.Users.CountAsync();
                var courseCount = await _context.Courses.CountAsync();
                var trainingPlanCount = await _context.TrainingPlans.CountAsync();

                var counts = new
                {
                    UserCount = userCount,
                    CourseCount = courseCount,
                    TrainingPlanCount = trainingPlanCount
                };

                return Ok(counts);
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
       


        //Forgot Password API
        [AllowAnonymous]
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(LMS.Models.User model, [FromServices] IConfiguration configuration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                {
                    // User with the specified email address does not exist
                    return NotFound();
                }

                // Generate a random password
                string newPassword = GenerateRandomPassword();

                // Update the user's password
                user.Password = newPassword;
                await _context.SaveChangesAsync();

                // Read email settings from configuration
                var emailSettings = configuration.GetSection("EmailSettings");
                string smtpServer = emailSettings["SmtpServer"];
                int smtpPort = int.Parse(emailSettings["SmtpPort"]);
                string smtpUsername = emailSettings["SmtpUsername"];
                string smtpPassword = emailSettings["SmtpPassword"];
                bool enableSsl = bool.Parse(emailSettings["EnableSsl"]);

                // Send the new password to the user's email address
                bool emailSent = await SendPasswordResetEmail(
                    model.Email,
                    newPassword,
                    smtpServer,
                    smtpPort,
                    smtpUsername,
                    smtpPassword,
                    enableSsl
                );

                if (emailSent)
                {
                    // Return a success message
                    return Ok("Password reset email has been sent.");
                }
                else
                {
                    return BadRequest("Failed to send the password reset email. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during password reset
                return BadRequest("Failed to reset the password: " + ex.Message);
            }
        }

        private string GenerateRandomPassword()
          {
              string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
              Random random = new Random();
              char[] passwordChars = new char[8]; // Set the desired password length here
              for (int i = 0; i < passwordChars.Length; i++)
              {
                  passwordChars[i] = allowedChars[random.Next(allowedChars.Length)];
              }
              return new string(passwordChars);
          }
          private async Task<bool> SendPasswordResetEmail(string email, string password, string smtpServer, int smtpPort, string smtpUsername, string smtpPassword, bool enableSsl)
          {
              try
              {
                  // Set up the email message
                  MailMessage message = new MailMessage();
                  message.From = new MailAddress("rishika.dewang@averybit.in"); // Your email address
                  message.To.Add(new MailAddress(email));
                  message.Subject = "Password Reset";
                  message.Body = $"Your new password is: {password}";
                  // Configure the SMTP client
                  SmtpClient smtpClient = new SmtpClient(smtpServer); // Your SMTP server address
                  smtpClient.Port = smtpPort; // The port number of your SMTP server
                  smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword); // Your email credentials
                  smtpClient.EnableSsl = enableSsl;
                  // Send the email
                  await smtpClient.SendMailAsync(message);
                  return true;
              }
              catch (Exception ex)
              {
                  // Handle any errors that occur during email sending
                  // You can log the error or perform any desired actions
                  return false;
              }
          }

       //Get List Of Designation API
       [Authorize(Roles = "Admin,User")]
        [HttpGet("designation")]
        public IActionResult GetDesignations()
        {
            try
            {
                var designations = _context.Designations.ToList();
                return Ok(designations);
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
       


        //Get Designation By ID 
        [Authorize(Roles = "Admin,User")]
        [HttpGet("designation/{id}")]
        public IActionResult GetDesignationById(int id)
        {
            try
            {
                var designation = _context.Designations.FirstOrDefault(d => d.Id == id);

                if (designation == null)
                {
                    return NotFound(); // Return a 404 Not Found if the designation is not found
                }

                return Ok(designation);
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
        



        // Get Assigned Courses AND Assigned Training Plan Courses
        [Authorize(Roles = "Admin,User")]
        [HttpGet("courses/{id}")]
        public async Task<IActionResult> GetCourses(int id)
        {
            try
            {
                if (id < 1)
                {
                    return BadRequest();
                }

                var assignedCourses = await (from assign in _context.UserAssignCourses
                                             join course in _context.Courses on assign.FkCourseId equals course.ID
                                             where assign.FkUserId == id
                                             select new
                                             {
                                                 CourseId = course.ID,
                                                 Title = course.Title,
                                                 Description = course.Description,
                                                 // status = assign.status
                                             }).ToListAsync();

                var trainingPlanCourses = await (from assign in _context.UserAssignCourses
                                                 from ep in _context.TrainingPlanCourses
                                                 join e in _context.Courses on ep.FkCourseId equals e.ID
                                                 join t in _context.TrainingPlans on ep.FkPlanId equals t.Id
                                                 where t.Id == id
                                                 select new
                                                 {
                                                     CourseId = e.ID,
                                                     Title = e.Title,
                                                     Description = e.Description,
                                                     // status = assign.status
                                                 }).ToListAsync();

                var combinedCourses = assignedCourses.Concat(trainingPlanCourses)
                                            .GroupBy(c => c.CourseId)
                                            .Select(g => g.First())
                                            .ToList();

                return Ok(combinedCourses);
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






