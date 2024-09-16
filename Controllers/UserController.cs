using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace IMSAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase{
        private readonly string _connectionString;

        public UserController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet("obteneruserbyid")]
        public async Task<IActionResult> ObtenerUserById(int userId){

            if(userId == 0){
                return BadRequest("Campos faltantes.");
            }

            UserGet user = null;
            
            using (SqlConnection connection = new SqlConnection(_connectionString)){
                string query = "select Username, Password, Name, Role, ProductsIdList from [User] where UserId = ruserid";
                query = query.Replace("ruserid",userId.ToString());
                using(SqlCommand command = new SqlCommand(query,connection)) {
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync()){
                            while (await reader.ReadAsync())
                            {
                                user = new UserGet{
                                    UserId = userId,
                                    Username = reader.GetString(0),
                                    Password = reader.GetString(1),
                                    Name = reader.GetString(2),
                                    Role = reader.GetByte(3),
                                    ProductsIdList = reader.GetString(4)
                                };
                            }
                        }

                        if(user == null){
                            return NotFound("Usuario no encontrado");
                        }

                        return Ok(user);
                    }
                    catch(SqlException ex)
                    {
                         return StatusCode(500, $"Internal server error: {ex.Message}");
                    }
                }
            }
        }

        [HttpGet("loginuser")]
        public async Task<IActionResult> LoginUser(string username, string password){
            if(username == null || password == null){
                return BadRequest("Campos faltantes");
            }

            UserGet user = null;
            
            using (SqlConnection connection = new SqlConnection(_connectionString)){
                string query = "select UserId, Name, Role, ProductsIdList from [User] where Username = 'rusername' AND Password = HASHBYTES('SHA2_256', 'rpassword');";
                query = query.Replace("rusername",username);
                query = query.Replace("rpassword",password);
                using(SqlCommand command = new SqlCommand(query,connection)) {
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync()){
                            while (await reader.ReadAsync())
                            {
                                user = new UserGet{
                                    UserId = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Role = reader.GetByte(2),
                                    ProductsIdList = reader.GetString(3)
                                };
                            }
                        }

                        if(user == null){
                            return NotFound("Credenciales incorrectas.");
                        }

                        return Ok(user);
                    }
                    catch(SqlException ex)
                    {
                         return StatusCode(500, $"Internal server error: {ex.Message}");
                    }
                }
            }
        }

        [HttpGet("obtenerusers")]
        public async Task<IActionResult> ObtenerUsers(int companyId){
            
            if(companyId == 0){
                return BadRequest("Campos faltantes.");
            }

            List<UserClass> users = new List<UserClass>();

            using (SqlConnection connection = new SqlConnection(_connectionString)){
                string query = "select UserId, Username, Password, Name, Role, ProductsIdList, ExpirationDate from [User] where CompanyId = rcompanyid";
                query = query.Replace("rcompanyid",companyId.ToString());
                using(SqlCommand command = new SqlCommand(query,connection)) {
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync()){
                            while (await reader.ReadAsync())
                            {
                                users.Add(new UserClass
                                {
                                    UserId = reader.GetInt32(0),
                                    CompanyId = companyId,
                                    Username = reader.GetString(1),
                                    Password = reader.GetString(2),
                                    Name = reader.GetString(3),
                                    Role = reader.GetByte(4),
                                    ProductsIdList = reader.GetString(5),
                                    ExpirationDate = reader.GetDateTime(6)
                                });
                            }
                        }
                        return Ok(users);
                    }
                    catch(SqlException ex)
                    {
                         return StatusCode(500, $"Internal server error: {ex.Message}");
                    }
                }
            }
        }

        [HttpPost("crearuser")]
        public async Task<IActionResult> CrearUser(UserCreation user){

            if(user == null){
                return BadRequest("Campos faltantes.");
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO [User] (CompanyId, Username, Password, Name, Role, ProductsIdList, ExpirationDate) VALUES (rcompany, 'ruser', HASHBYTES('SHA2_256', 'rpass'), 'rnameuser', rrole, 'rproducts', 'rexp')";
                query = query.Replace("rcompany", user.CompanyId.ToString());
                query = query.Replace("ruser", user.Username);
                query = query.Replace("rpass", user.Password);
                query = query.Replace("rnameuser", user.Name);
                query = query.Replace("rrole", user.Role.ToString());
                query = query.Replace("rproducts", user.ProductsIdList);
                query = query.Replace("rexp", user.ExpirationDate.ToString());
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();
                        return Ok("Usuario guardada correctamente");
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