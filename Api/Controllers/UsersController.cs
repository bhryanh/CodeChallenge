
using System.Data;
using CodeChallenge.Domain.Model;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
namespace CodeChallenge.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public UsersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [HttpGet("{username}")]
        public async Task<IActionResult> GetUser(string username)
        {
            if(string.IsNullOrWhiteSpace(username))
                return BadRequest("Request error, username is empty or null");
                
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                //Create the connection
                using IDbConnection connection = new SqliteConnection(connectionString);
                connection.Open();
                
                //Create The query
                var query = "SELECT * FROM Users WHERE username = @Username";

                //Add the parameters
                var parameters = new DynamicParameters();
                parameters.Add("@Username", username);

                //Execute the query with parameters
                var result = await connection.QueryAsync<User>(query, parameters);

                //Get User
                var user = result.FirstOrDefault();

                if(user is null)
                    return NotFound($"User with user name {username} not found");

                return Ok(new {Message = $"Hello, {user.userName}"});
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, new { Message = "An error occurred while processing your request."});
            }

        }
    }
}