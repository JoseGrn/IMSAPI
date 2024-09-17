using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace IMSAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase{
        private readonly string _connectionString;

        public ProductController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet("obtenerlistaproductos")]
        public async Task<IActionResult> ObtenerProductos(string productos, int companyId){

            if(productos == null || productos == "" || companyId == 0){
                return BadRequest("Campos faltantes.");
            }

            List<GetProduct> listaProductos = new List<GetProduct>();
            
            using (SqlConnection connection = new SqlConnection(_connectionString)){
                string query = "select Name, Description, MinQuantity, Quantity, Specie, Price from Product where ProductId IN (rproductos) AND CompanyId = rcompanyid";
                query = query.Replace("rproductos", productos);
                query = query.Replace("rcompanyid", companyId.ToString());
                using(SqlCommand command = new SqlCommand(query,connection)) {
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync()){
                            while (await reader.ReadAsync())
                            {
                                listaProductos.Add(new GetProduct
                                {
                                    Name = reader.GetString(0),
                                    Description = reader.GetString(1),
                                    MinQuantity = reader.GetInt32(2),
                                    Quantity = reader.GetInt32(3),
                                    Specie = reader.GetString(4),
                                    Price = reader.GetDecimal(5)
                                });
                            }
                        }

                        if(listaProductos.Count == 0){
                            return BadRequest("No se encontraron productos");
                        }

                        return Ok(listaProductos);
                    }
                    catch(SqlException ex)
                    {
                         return StatusCode(500, $"Internal server error: {ex.Message}");
                    }
                }
            }
        }

        [HttpPost("crearproducto")]
        public async Task<IActionResult> CrearProducto(Product product){

            if(product == null){
                return BadRequest("Campos faltantes.");
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Product (CompanyId, Name, Description, MinQuantity, Quantity, Specie, Price) VALUES (rcompany, 'rname', 'rdescription', rminq, rquantity, 'rspecie', rprice)";
                query = query.Replace("rcompany", product.CompanyId.ToString());
                query = query.Replace("rname", product.Name);
                query = query.Replace("rdescription", product.Description);
                query = query.Replace("rminq", product.MinQuantity.ToString());
                query = query.Replace("rquantity", product.Quantity.ToString());
                query = query.Replace("rspecie", product.Specie);
                query = query.Replace("rprice", product.Price.ToString());
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();
                        return Ok("Producto guardado correctamente");
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