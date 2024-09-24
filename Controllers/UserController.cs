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
                string query = "select Username, Name, Role, ProductsIdList from [User] where UserId = ruserid AND IsActive = 1";
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
                                    Name = reader.GetString(1),
                                    Role = reader.GetByte(2),
                                    ProductsIdList = reader.GetString(3)
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
        public async Task<IActionResult> LoginUser(string user, string password){
            if(user == null || password == null){
                return BadRequest("Campos faltantes");
            }

            UserGet userInfo = null;
            
            using (SqlConnection connection = new SqlConnection(_connectionString)){
                string query = "select UserId, CompanyId, Username, Name, Role, ProductsIdList from [User] where Username = 'rusername' AND Password = HASHBYTES('SHA2_256', 'rpassword') AND IsActive = 1;";
                query = query.Replace("rusername",user);
                query = query.Replace("rpassword",password);
                using(SqlCommand command = new SqlCommand(query,connection)) {
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync()){
                            while (await reader.ReadAsync())
                            {
                                userInfo = new UserGet{
                                    UserId = reader.GetInt32(0),
                                    CompanyId = reader.GetInt32(1),
                                    Username = reader.GetString(2),
                                    Name = reader.GetString(3),
                                    Role = reader.GetByte(4),
                                    ProductsIdList = reader.GetString(5)
                                };
                            }
                        }

                        if(userInfo == null){
                            return NotFound("Credenciales incorrectas.");
                        }

                        return Ok(userInfo);
                    }
                    catch(SqlException ex)
                    {
                         return StatusCode(500, $"Internal server error: {ex.Message}");
                    }
                }
            }
        }

        [HttpPost("editarusuario")]
        public async Task<IActionResult> EditarProducto(UserClass user){

            if(user.UserId == 0){
                return BadRequest("Campos faltantes");
            }

            if(user.Password == ""){
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "UPDATE [User] SET Username = 'rusername', Name = 'rnameuser', Role = rrole, ProductsIdList = 'rproducts', ExpirationDate = 'rexpiration' WHERE UserId = ruserid";
                    query = query.Replace("rusername", user.Username);
                    query = query.Replace("rnameuser", user.Name);
                    query = query.Replace("rrole", user.Role.ToString());
                    query = query.Replace("rproducts", user.ProductsIdList);
                    query = query.Replace("rexpiration", user.ExpirationDate.ToString());
                    query = query.Replace("ruserid", user.UserId.ToString());
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
            } else{
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "UPDATE [User] SET Username = 'rusername', Password = HASHBYTES('SHA2_256', 'rpassword'), Name = 'rnameuser', Role = rrole, ProductsIdList = 'rproducts', ExpirationDate = 'rexpiration' WHERE UserId = ruserid";
                    query = query.Replace("rusername", user.Username);
                    query = query.Replace("rpassword", user.Password);
                    query = query.Replace("rnameuser", user.Name);
                    query = query.Replace("rrole", user.Role.ToString());
                    query = query.Replace("rproducts", user.ProductsIdList);
                    query = query.Replace("rexpiration", user.ExpirationDate.ToString());
                    query = query.Replace("ruserid", user.UserId.ToString());
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
            }

            List<UserClass> users = new List<UserClass>();

            using (SqlConnection connection = new SqlConnection(_connectionString)){
                string query = "select UserId, Username, Name, Role, ProductsIdList, ExpirationDate from [User] where CompanyId = rcompanyid AND IsActive = 1";
                query = query.Replace("rcompanyid",user.CompanyId.ToString());
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
                                    CompanyId = user.CompanyId,
                                    Username = reader.GetString(1),
                                    Name = reader.GetString(2),
                                    Role = reader.GetByte(3),
                                    ProductsIdList = reader.GetString(4),
                                    ExpirationDate = reader.GetDateTime(5)
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

        [HttpDelete("eliminaruser")]
        public async Task<IActionResult> EliminarProducto(int userId, int companyId){
            if(userId == 0 || companyId == 0){
                return BadRequest("Campos faltantes");
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE [User] SET IsActive = 0 WHERE UserId = ruserid";
                query = query.Replace("ruserid", userId.ToString());
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

            List<UserClass> users = new List<UserClass>();

            using (SqlConnection connection = new SqlConnection(_connectionString)){
                string query = "select UserId, Username, Name, Role, ProductsIdList, ExpirationDate from [User] where CompanyId = rcompanyid AND IsActive = 1";
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
                                    Name = reader.GetString(2),
                                    Role = reader.GetByte(3),
                                    ProductsIdList = reader.GetString(4),
                                    ExpirationDate = reader.GetDateTime(5)
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

        [HttpGet("obtenerusers")]
        public async Task<IActionResult> ObtenerUsers(int companyId){
            
            if(companyId == 0){
                return BadRequest("Campos faltantes.");
            }

            List<UserClass> users = new List<UserClass>();

            using (SqlConnection connection = new SqlConnection(_connectionString)){
                string query = "select UserId, Username, Name, Role, ProductsIdList, ExpirationDate from [User] where CompanyId = rcompanyid AND IsActive = 1";
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
                                    Name = reader.GetString(2),
                                    Role = reader.GetByte(3),
                                    ProductsIdList = reader.GetString(4),
                                    ExpirationDate = reader.GetDateTime(5)
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
                    }
                    catch (SqlException ex)
                    {
                        // Handle exception
                        return StatusCode(500, $"Internal server error: {ex.Message}");
                    }
                }
            }

            List<UserClass> users = new List<UserClass>();

            using (SqlConnection connection = new SqlConnection(_connectionString)){
                string query = "select UserId, Username, Name, Role, ProductsIdList, ExpirationDate from [User] where CompanyId = rcompanyid AND IsActive = 1";
                query = query.Replace("rcompanyid",user.CompanyId.ToString());
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
                                    CompanyId = user.CompanyId,
                                    Username = reader.GetString(1),
                                    Name = reader.GetString(2),
                                    Role = reader.GetByte(3),
                                    ProductsIdList = reader.GetString(4),
                                    ExpirationDate = reader.GetDateTime(5)
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
    }
}