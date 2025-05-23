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

        [HttpGet("obtenerproductosnombres")]
        public async Task<IActionResult> ObtenerProductosNombres(int companyId){
            if(companyId == 0){
                return BadRequest("Campos faltantes.");
            }

            List<ProductNames> listaProductos = new List<ProductNames>();

            using (SqlConnection connection = new SqlConnection(_connectionString)){
                string query = "select ProductId, Name from Product where CompanyId = rcompanyid AND IsActive = 1";
                query = query.Replace("rcompanyid", companyId.ToString());
                using(SqlCommand command = new SqlCommand(query,connection)) {
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync()){
                            while (await reader.ReadAsync())
                            {
                                listaProductos.Add(new ProductNames
                                {
                                    ProductId = reader.GetInt32(0),
                                    Name = reader.GetString(1)
                                });
                            }
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

        [HttpGet("obtenerproductosgeneral")]
        public async Task<IActionResult> ObtenerProductosGeneral(int companyId){
            if(companyId == 0){
                return BadRequest("Campos faltantes.");
            }

            List<GetProduct> listaProductos = new List<GetProduct>();

            using (SqlConnection connection = new SqlConnection(_connectionString)){
                string query = "select ProductId, Name, Description, MinQuantity, Quantity, Specie, Price from Product where CompanyId = rcompanyid AND IsActive = 1";
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
                                    ProductId = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Description = reader.GetString(2),
                                    MinQuantity = reader.GetInt32(3),
                                    Quantity = reader.GetInt32(4),
                                    Specie = reader.GetString(5),
                                    Price = reader.GetDecimal(6)
                                });
                            }
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

        [HttpGet("obtenerlistaproductos")]
        public async Task<IActionResult> ObtenerProductos(string productos, int companyId){

            if(productos == null || productos == "" || companyId == 0){
                return BadRequest("Campos faltantes.");
            }

            List<GetProduct> listaProductos = new List<GetProduct>();
            
            using (SqlConnection connection = new SqlConnection(_connectionString)){
                string query = "select ProductId, Name, Description, MinQuantity, Quantity, Specie, Price from Product where ProductId IN (rproductos) AND CompanyId = rcompanyid AND IsActive = 1";
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
                                    ProductId = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Description = reader.GetString(2),
                                    MinQuantity = reader.GetInt32(3),
                                    Quantity = reader.GetInt32(4),
                                    Specie = reader.GetString(5),
                                    Price = reader.GetDecimal(6)
                                });
                            }
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
                    }
                    catch (SqlException ex)
                    {
                        // Handle exception
                        return StatusCode(500, $"Internal server error: {ex.Message}");
                    }
                }
            }

            List<GetProduct> listaProductos = new List<GetProduct>();

            using (SqlConnection connection = new SqlConnection(_connectionString)){
                string query = "select ProductId, Name, Description, MinQuantity, Quantity, Specie, Price from Product where CompanyId = rcompanyid AND IsActive = 1";
                query = query.Replace("rcompanyid", product.CompanyId.ToString());
                using(SqlCommand command = new SqlCommand(query,connection)) {
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync()){
                            while (await reader.ReadAsync())
                            {
                                listaProductos.Add(new GetProduct
                                {
                                    ProductId = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Description = reader.GetString(2),
                                    MinQuantity = reader.GetInt32(3),
                                    Quantity = reader.GetInt32(4),
                                    Specie = reader.GetString(5),
                                    Price = reader.GetDecimal(6)
                                });
                            }
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

        [HttpPost("modificarcantidad")]
        public async Task<IActionResult> ModificarCantidad(int cantidad, int productId, int companyId, string listaProductos){

            if(cantidad == null || productId == 0 || companyId == 0){
                return BadRequest("Campos faltantes");
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Product SET Quantity = rquantity WHERE ProductId = rproductid";
                query = query.Replace("rquantity", cantidad.ToString());
                query = query.Replace("rproductid", productId.ToString());
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

            List<GetProduct> listaProductosNuevos = new List<GetProduct>();

            using (SqlConnection connection = new SqlConnection(_connectionString)){
                string query = "select ProductId, Name, Description, MinQuantity, Quantity, Specie, Price from Product where ProductId IN (rproductos) AND CompanyId = rcompanyid AND IsActive = 1";
                query = query.Replace("rproductos", listaProductos);
                query = query.Replace("rcompanyid", companyId.ToString());
                using(SqlCommand command = new SqlCommand(query,connection)) {
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync()){
                            while (await reader.ReadAsync())
                            {
                                listaProductosNuevos.Add(new GetProduct
                                {
                                    ProductId = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Description = reader.GetString(2),
                                    MinQuantity = reader.GetInt32(3),
                                    Quantity = reader.GetInt32(4),
                                    Specie = reader.GetString(5),
                                    Price = reader.GetDecimal(6)
                                });
                            }
                        }

                        if(listaProductosNuevos.Count == 0){
                            return BadRequest("No se encontraron productos");
                        }

                        return Ok(listaProductosNuevos);
                    }
                    catch(SqlException ex)
                    {
                         return StatusCode(500, $"Internal server error: {ex.Message}");
                    }
                }
            }
        }

        [HttpPost("editarproducto")]
        public async Task<IActionResult> EditarProducto(EditProduct product){

            if(product == null){
                return BadRequest("Campos faltantes");
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Product SET Name = 'rname', Description = 'rdescription', MinQuantity = rminq, Quantity = rquantity, Specie = 'rspecie', Price = rprice WHERE ProductId = rproductid";
                query = query.Replace("rname", product.Name);
                query = query.Replace("rdescription", product.Description);
                query = query.Replace("rminq", product.MinQuantity.ToString());
                query = query.Replace("rquantity", product.Quantity.ToString());
                query = query.Replace("rspecie", product.Specie);
                query = query.Replace("rprice", product.Price.ToString());
                query = query.Replace("rproductid", product.ProductId.ToString());
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

            List<GetProduct> listaProductos = new List<GetProduct>();

            using (SqlConnection connection = new SqlConnection(_connectionString)){
                string query = "select ProductId, Name, Description, MinQuantity, Quantity, Specie, Price from Product where CompanyId = rcompanyid AND IsActive = 1";
                query = query.Replace("rcompanyid", product.CompanyId.ToString());
                using(SqlCommand command = new SqlCommand(query,connection)) {
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync()){
                            while (await reader.ReadAsync())
                            {
                                listaProductos.Add(new GetProduct
                                {
                                    ProductId = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Description = reader.GetString(2),
                                    MinQuantity = reader.GetInt32(3),
                                    Quantity = reader.GetInt32(4),
                                    Specie = reader.GetString(5),
                                    Price = reader.GetDecimal(6)
                                });
                            }
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

        [HttpDelete("eliminarproducto")]
        public async Task<IActionResult> EliminarProducto(int productId, int companyId){
            if(productId == 0 || companyId == 0){
                return BadRequest("Campos faltantes");
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Product SET IsActive = 0 WHERE ProductId = rproductid";
                query = query.Replace("rproductid", productId.ToString());
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

            List<GetProduct> listaProductos = new List<GetProduct>();

            using (SqlConnection connection = new SqlConnection(_connectionString)){
                string query = "select ProductId, Name, Description, MinQuantity, Quantity, Specie, Price from Product where CompanyId = rcompanyid AND IsActive = 1";
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
                                    ProductId = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Description = reader.GetString(2),
                                    MinQuantity = reader.GetInt32(3),
                                    Quantity = reader.GetInt32(4),
                                    Specie = reader.GetString(5),
                                    Price = reader.GetDecimal(6)
                                });
                            }
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
    }
}