using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace IMSAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShipmentController : ControllerBase{
        private readonly string _connectionString;

        public ShipmentController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet("obtenerenviobyid")]
         public async Task<IActionResult> ObtenerEnvioById(int shipmentId){
            if(shipmentId == null) {
                return BadRequest("Campos faltantes.");
            }
            
            ShipmentGetById shipmentList = new ShipmentGetById();
            string listaProductos = "", listaCantidades = "";
            TimeSpan time = new TimeSpan(0,0,0);

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT ShipmentId, Number, ShippingNote, DepartureDate, [Week], ClientName, Destination, CubicMeters, [Label], DepartureTime, Transportation, TrailerPlate, ContainerPlate, Pilot, Observation, ClientId, ProductsList, QuantityList FROM Shipment WHERE ShipmentId = repshipmentid";
                query = query.Replace("repshipmentid", shipmentId.ToString());
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                shipmentList = new ShipmentGetById
                                {
                                    ShipmentId = reader.GetInt32(0),
                                    Number = reader.GetString(1),
                                    ShippingNote = reader.GetString(2),
                                    DepartureDate = reader.GetDateTime(3),
                                    Week = reader.GetByte(4),
                                    ClientName = reader.GetString(5),
                                    Destination = reader.GetString(6),
                                    CubicMeters = reader.GetDecimal(7),
                                    Label = reader.GetString(8),
                                    Transportation = reader.GetString(10),
                                    TrailerPlate = reader.GetString(11),
                                    ContainerPlate = reader.GetString(12),
                                    Pilot = reader.GetString(13),
                                    Observation = reader.GetString(14),
                                    ClientId = reader.GetInt32(15)
                                };
                                time = reader.GetTimeSpan(9);
                                listaProductos = reader.GetString(16);
                                listaCantidades = reader.GetString(17);
                            }
                            
                            shipmentList.DepartureTime = time.ToString(@"hh\:mm");

                            string[] newListaCantidades = listaCantidades.Split(',');
                            string[] newListaProductos = listaProductos.Split(',');

                            List<ShipmentProducts> shipmentProducts = new List<ShipmentProducts>();

                            for(int i = 0; i < newListaCantidades.Length; i++){
                                ShipmentProducts ProductsList = new ShipmentProducts{
                                    ProductId = Convert.ToInt32(newListaProductos[i]),
                                    Quantity = Convert.ToInt32(newListaCantidades[i])
                                };

                                shipmentProducts.Add(ProductsList);
                            }

                            shipmentList.Products = shipmentProducts;
                        }
                        return Ok(shipmentList);
                    }
                    catch (SqlException ex)
                    {
                        // Handle exception
                        return StatusCode(500, $"Internal server error: {ex.Message}");
                    }
                }
            }
         }

        [HttpGet("obtenerenvioslist")]
         public async Task<IActionResult> ObtenerEnviosList(int companyId){
            if(companyId == null) {
                return BadRequest("Campos faltantes.");
            }
            
            List<GetShipment> shipmentList = new List<GetShipment>();
            
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT ShipmentId, Number, ClientName, DepartureDate FROM Shipment WHERE CompanyId = repcompanyid AND IsActive = 1";
                query = query.Replace("repcompanyid", companyId.ToString());
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                shipmentList.Add(new GetShipment
                                {
                                    ShipmentId = reader.GetInt32(0),
                                    Number = reader.GetString(1),
                                    ClientName = reader.GetString(2),
                                    DepartureDate = reader.GetDateTime(3)
                                });
                            }
                        }
                        return Ok(shipmentList);
                    }
                    catch (SqlException ex)
                    {
                        // Handle exception
                        return StatusCode(500, $"Internal server error: {ex.Message}");
                    }
                }
            }
         }

        [HttpPost("crearenvio")]
        public async Task<IActionResult> CrearEnvio(ShipmentCreate shipmentCreate){
            if(shipmentCreate == null){
                return BadRequest("Campos Faltantes.");
            }

            string productIds = "", quantityList = "";
            int count = 1;
            foreach(var producto in shipmentCreate.Products){
                if(count == shipmentCreate.Products.Count){
                    productIds += producto.ProductId;
                    quantityList += producto.Quantity;
                } else {
                    productIds += producto.ProductId + ",";
                    quantityList += producto.Quantity + ",";
                }
                count++;
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Shipment (CreationDate, CompanyId, Number, ShippingNote, DepartureDate, [Week], ClientName, Destination, ProductsList, QuantityList, CubicMeters, [Label], DepartureTime, Transportation, TrailerPlate, ContainerPlate, Pilot, Responsible, Observation, IsActive, ClientId) VALUES (GETDATE(), repcompanyid, 'repnumbership', 'repshippingnote', 'repdeparturedate', repweek, 'repclientname', 'repdestination', 'repproductslist', 'repquantitylist', repcubicmeters, 'replabel', 'repdeparturetime', 'reptransportation', 'reptrailerplate', 'repcontainerplate', 'reppilot', 'represponsible', 'repobservation', 1, repclientid);";
                
                query = query.Replace("repcompanyid", shipmentCreate.CompanyId.ToString());
                query = query.Replace("repnumbership", shipmentCreate.Number);
                query = query.Replace("repshippingnote", shipmentCreate.ShippingNote);
                query = query.Replace("repdeparturedate", shipmentCreate.DepartureDate.ToString());
                query = query.Replace("repweek", shipmentCreate.Week.ToString());
                query = query.Replace("repclientname", shipmentCreate.ClientName);
                query = query.Replace("repdestination", shipmentCreate.Destination);
                query = query.Replace("repproductslist", productIds);
                query = query.Replace("repquantitylist", quantityList);
                query = query.Replace("repcubicmeters", shipmentCreate.CubicMeters.ToString());
                query = query.Replace("replabel", shipmentCreate.Label);
                query = query.Replace("repdeparturetime", shipmentCreate.DepartureTime);
                query = query.Replace("reptransportation", shipmentCreate.Transportation);
                query = query.Replace("reptrailerplate", shipmentCreate.TrailerPlate);
                query = query.Replace("repcontainerplate", shipmentCreate.ContainerPlate);
                query = query.Replace("reppilot", shipmentCreate.Pilot);
                query = query.Replace("represponsible", shipmentCreate.Responsible);
                query = query.Replace("repobservation", shipmentCreate.Observation);
                query = query.Replace("repclientid", shipmentCreate.ClientId.ToString());
                
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

            foreach(var val in shipmentCreate.Products){
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "UPDATE [Product] SET Quantity = Quantity - @Quantityrep where ProductId = @productidrep";
                    query = query.Replace("@Quantityrep", val.Quantity.ToString());
                    query = query.Replace("@productidrep", val.ProductId.ToString());
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

            return Ok(new { status = "success", message = "Orden de compra creada correctamente" });
        }

        [HttpPost("editarenvio")]
        public async Task<IActionResult> EditarEnvio(ShipmentEdit shipmentCreate){
            if(shipmentCreate == null){
                return BadRequest("Campos Faltantes.");
            }

            string productIds = "", quantityList = "";
            int count = 1;
            foreach(var producto in shipmentCreate.Products){
                if(count == shipmentCreate.Products.Count){
                    productIds += producto.ProductId;
                    quantityList += producto.Quantity;
                } else {
                    productIds += producto.ProductId + ",";
                    quantityList += producto.Quantity + ",";
                }
                count++;
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Shipment SET Number = 'repnumber', ShippingNote = 'repshippingnote', DepartureDate = 'repdeparturedate', [Week] = repweek, ClientName = 'repclientname', Destination = 'repdestination', ProductsList = 'repproductslist', QuantityList = 'repquantitylist', CubicMeters = repcubicmeters, [Label] = 'replabel', DepartureTime = 'repdeparturetime', Transportation = 'reptransportation', TrailerPlate = 'reptrailerplate', ContainerPlate = 'repcontainerplate', Pilot = 'reppilot', Observation = 'repobservation', ClientId = repclientid WHERE  ShipmentId = repshipmentid;";
                
                query = query.Replace("repnumber", shipmentCreate.Number);
                query = query.Replace("repshippingnote", shipmentCreate.ShippingNote);
                query = query.Replace("repdeparturedate", shipmentCreate.DepartureDate.ToString());
                query = query.Replace("repweek", shipmentCreate.Week.ToString());
                query = query.Replace("repclientname", shipmentCreate.ClientName);
                query = query.Replace("repdestination", shipmentCreate.Destination);
                query = query.Replace("repproductslist", productIds);
                query = query.Replace("repquantitylist", quantityList);
                query = query.Replace("repcubicmeters", shipmentCreate.CubicMeters.ToString());
                query = query.Replace("replabel", shipmentCreate.Label);
                query = query.Replace("repdeparturetime", shipmentCreate.DepartureTime);
                query = query.Replace("reptransportation", shipmentCreate.Transportation);
                query = query.Replace("reptrailerplate", shipmentCreate.TrailerPlate);
                query = query.Replace("repcontainerplate", shipmentCreate.ContainerPlate);
                query = query.Replace("reppilot", shipmentCreate.Pilot);
                query = query.Replace("repobservation", shipmentCreate.Observation);
                query = query.Replace("repclientid", shipmentCreate.ClientId.ToString());
                query = query.Replace("repshipmentid", shipmentCreate.ShipmentId.ToString());
                
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

            return Ok(new { status = "success", message = "Orden de compra creada correctamente" });
        }
    }
}