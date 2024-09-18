using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace IMSAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly string _connectionString;

        public CompanyController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet("obtenerempresas")]
        public async Task<IActionResult> ObtenerEmpresa(int ownerId)
        {
            if(ownerId == null) {
                return BadRequest("Campos faltantes.");
            }
            
            List<Company> companies = new List<Company>();
            
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT CompanyId, Name, Description, CreationDate FROM Company WHERE OwnerId = rownerid AND IsActive = 1;";
                query = query.Replace("rownerid", ownerId.ToString());
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                companies.Add(new Company
                                {
                                    CompanyId = reader.GetInt32(0),
                                    OwnerId = ownerId,
                                    Name = reader.GetString(1),
                                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                                    Date = reader.GetDateTime(3)
                                });
                            }
                        }
                        return Ok(companies);
                    }
                    catch (SqlException ex)
                    {
                        // Handle exception
                        return StatusCode(500, $"Internal server error: {ex.Message}");
                    }
                }
            }
        }

        [HttpPost("crearempresa")]
        public async Task<IActionResult> CrearEmpresa(CreateCompany company)
        {
            if(company == null) {  
                return BadRequest("Campos faltantes");
            }

            List<Company> companies = new List<Company>();
            
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Company (OwnerId, Name, Description) VALUES (rownerid, 'rname', 'rdescription');";
                query = query.Replace("rownerid", company.OwnerId.ToString());
                query = query.Replace("rname", company.Name);
                query = query.Replace("rdescription", company.Description);
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();
                        //return Ok(new { message = "Empresa guardada correctamente" });
                    }
                    catch (SqlException ex)
                    {
                        // Handle exception
                        return StatusCode(500, $"Internal server error: {ex.Message}");
                    }
                }
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT CompanyId, Name, Description, CreationDate FROM Company WHERE OwnerId = rownerid AND IsActive = 1;";
                query = query.Replace("rownerid", company.OwnerId.ToString());
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                companies.Add(new Company
                                {
                                    CompanyId = reader.GetInt32(0),
                                    OwnerId = company.OwnerId,
                                    Name = reader.GetString(1),
                                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                                    Date = reader.GetDateTime(3)
                                });
                            }
                        }
                        return Ok(companies);
                    }
                    catch (SqlException ex)
                    {
                        // Handle exception
                        return StatusCode(500, $"Internal server error: {ex.Message}");
                    }
                }
            }
        }

        [HttpPost("editarempresa")]
        public async Task<IActionResult> EditarEmpresa(int companyId, string name, string description, int ownerId){
            if(companyId == null || name == null || description == null) {
                return BadRequest("Campos faltantes");
            }

            List<Company> companies = new List<Company>();
            
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Company SET Name = 'rname', Description = 'rdescription.' WHERE CompanyId = rcompanyid;";
                query = query.Replace("rcompanyid", companyId.ToString());
                query = query.Replace("rname", name);
                query = query.Replace("rdescription", description);
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

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT CompanyId, Name, Description, CreationDate FROM Company WHERE OwnerId = rownerid AND IsActive = 1;";
                query = query.Replace("rownerid", ownerId.ToString());
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                companies.Add(new Company
                                {
                                    CompanyId = reader.GetInt32(0),
                                    OwnerId = ownerId,
                                    Name = reader.GetString(1),
                                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                                    Date = reader.GetDateTime(3)
                                });
                            }
                        }
                        return Ok(companies);
                    }
                    catch (SqlException ex)
                    {
                        // Handle exception
                        return StatusCode(500, $"Internal server error: {ex.Message}");
                    }
                }
            }
        }

        [HttpPost("borrarempresa")]
        public async Task<IActionResult> BorrarEmpresa(int companyId){
            if(companyId == null) {
                return BadRequest("Campos faltantes");
            }
            
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Company SET IsActive = 0 WHERE CompanyId = rcompanyid;";
                query = query.Replace("rcompanyid", companyId.ToString());
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();
                        return Ok("Empresa eliminada correctamente");
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


// using (SqlConnection connection = new SqlConnection(_connectionString))
// {
//     const string query = "INSERT INTO Company (OwnerId, Name, Description) VALUES (rownerid, 'rname', 'rdescription');";

//     using (SqlCommand command = new SqlCommand(query, connection))
//     {
//         try
//         {
//             await connection.OpenAsync();

//             using (SqlDataReader reader = await command.ExecuteReaderAsync())
//             {
//                 while (await reader.ReadAsync())
//                 {
//                     companies.Add(new Company
//                     {
//                         OwnerId = reader.GetInt32(1),
//                         Name = reader.GetString(2),
//                         Description = reader.IsDBNull(3) ? null : reader.GetString(3)
//                     });
//                 }
//             }

//         }
//         catch (SqlException ex)
//         {
//             // Handle exception
//             return StatusCode(500, $"Internal server error: {ex.Message}");
//         }
//     }
// }