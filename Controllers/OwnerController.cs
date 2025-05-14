using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace IMSAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OwnerController : ControllerBase
    {
        private readonly string _connectionString;

        public OwnerController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet("loginowner")]
        public async Task<IActionResult> LoginOwner(string user, string password)
        {

            if (user == null || password == null)
            {
                return BadRequest("Campos faltantes");
            }

            Owner owner = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT OwnerId, Username, FirstName, LastName FROM Owner where Username = 'ruser' AND Password =  HASHBYTES('SHA2_256', 'rpassword');";
                query = query.Replace("ruser", user);
                query = query.Replace("rpassword", password);
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                owner = new Owner
                                {
                                    OwnerId = reader.GetInt32(0),
                                    username = reader.GetString(1),
                                    FirstName = reader.GetString(2),
                                    LastName = reader.GetString(3),
                                };
                            }
                        }

                        if(owner == null){
                            return BadRequest("Inicio de sesion fallido.");
                        }

                        return Ok(owner);
                    }
                    catch (SqlException ex)
                    {
                        // Handle exception
                        return StatusCode(500, $"Internal server error: {ex.Message}");
                    }
                }
            }
        }

        [HttpPost("editowner")]
        public async Task<IActionResult> EditOwner(OwnerEdition owner)
        {
            if(owner == null){
                return BadRequest("Campos faltantes");
            }
            
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Owner SET FirstName = 'rfname', LastName = 'rlname', Username = 'ruser' WHERE OwnerId = rid;";
                query = query.Replace("rfname", owner.FirstName);
                query = query.Replace("rlname", owner.LastName);
                query = query.Replace("ruser", owner.Username);
                query = query.Replace("rid", owner.OwnerId.ToString());
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();
                    }
                    catch (SqlException ex)
                    {
                        // Handle exception
                        return StatusCode(500, $"Internal server error: {ex.Message}");
                    }
                }
            }

            Owner newowner = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT OwnerId, Username, FirstName, LastName FROM Owner where OwnerId = rownerid;";
                query = query.Replace("rownerid", owner.OwnerId.ToString());
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                newowner = new Owner
                                {
                                    OwnerId = reader.GetInt32(0),
                                    username = reader.GetString(1),
                                    FirstName = reader.GetString(2),
                                    LastName = reader.GetString(3),
                                };
                            }
                        }
                        return Ok(owner);
                    }
                    catch (SqlException ex)
                    {
                        // Handle exception
                        return StatusCode(500, $"Internal server error: {ex.Message}");
                    }
                }
            }
        }

        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword(string password, int ownerId){
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Owner SET Password = HASHBYTES('SHA2_256', 'rpassword') WHERE OwnerId = rowner;";
                query = query.Replace("rpassword", password);
                query = query.Replace("rowner", ownerId.ToString());
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();
                        return Ok("Contraseña actualizada correctamente.");
                    }
                    catch (SqlException ex)
                    {
                        // Handle exception
                        return StatusCode(500, $"Internal server error: {ex.Message}");
                    }
                }
            }
        }
    }
}