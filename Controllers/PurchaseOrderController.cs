using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace IMSAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseOrderController : ControllerBase{
        private readonly string _connectionString;

        public PurchaseOrderController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet("obtenerclientpurchaseorder")]
        public async Task<IActionResult> ObtenerClientPurchaseOrder(int companyId, int userId){
            if(companyId == null) {
                return BadRequest("Campos faltantes.");
            }
            
            List<PurchaseOrderGet> purchaseOrders = new List<PurchaseOrderGet>();
            
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT PurchaseOrderId, ClientName, Number, LastDate, [state] FROM PurchaseOrder WHERE CompanyId = repcompanyid AND ClientId = repuserid AND IsActive = 1";
                query = query.Replace("repcompanyid", companyId.ToString());
                query = query.Replace("repuserid", userId.ToString());
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                purchaseOrders.Add(new PurchaseOrderGet
                                {
                                    PurchaseOrderId = reader.GetInt32(0),
                                    ClientName = reader.GetString(1),
                                    Number = reader.GetString(2),
                                    LastDate = reader.GetDateTime(3),
                                    state = reader.GetByte(4)
                                });
                            }
                        }
                        return Ok(purchaseOrders);
                    }
                    catch (SqlException ex)
                    {
                        // Handle exception
                        return StatusCode(500, $"Internal server error: {ex.Message}");
                    }
                }
            }
        }

        [HttpGet("purchaseorderbyid")]
        public async Task<IActionResult> PurchaseOrderById(int purchaseOrderId){
            if(purchaseOrderId == null){
                return BadRequest("Campos faltantes.");
            }

            PurchaseOrderEdit purchaseOrderEdit = new PurchaseOrderEdit();
            string listaProductos = "", listaCantidades = "";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "select PurchaseOrderId, Number, Direction, NIT, LastDate, [Description], SubTotal1, SubTotal2, Weekend, ProductsList, CompanyId, QuantityList, ClientId from PurchaseOrder where PurchaseOrderId = reppurchaseorder;";
                query = query.Replace("reppurchaseorder", purchaseOrderId.ToString());
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                purchaseOrderEdit = new PurchaseOrderEdit{
                                    PurchaseOrderId = reader.GetInt32(0),
                                    Number = reader.GetString(1),
                                    Direction = reader.GetString(2),
                                    NIT = reader.GetString(3),
                                    LastDate = reader.GetDateTime(4),
                                    Description = reader.GetString(5),
                                    SubTotal1 = reader.GetDecimal(6),
                                    SubTotal2 = reader.GetDecimal(7),
                                    Weekend = reader.GetByte(8),
                                    CompanyId = reader.GetInt32(10),
                                    ClientId = reader.GetInt32(12),
                                };
                                listaProductos = reader.GetString(9);
                                listaCantidades = reader.GetString(11);
                            }
                        }

                        string[] newListaCantidades = listaCantidades.Split(',');
                        string[] newListaProductos = listaProductos.Split(',');
                        
                        List<ProductsListEdit> productsLists = new List<ProductsListEdit>();

                        for(int i = 0; i < newListaCantidades.Length; i++){
                            ProductsListEdit ProductsList = new ProductsListEdit{
                                ProductId = Convert.ToInt32(newListaProductos[i]),
                                ProductQuantity = Convert.ToInt32(newListaCantidades[i])
                            };

                            productsLists.Add(ProductsList);
                        }

                        purchaseOrderEdit.ListaProductos = productsLists;

                        return Ok(purchaseOrderEdit);
                    }
                    catch (SqlException ex)
                    {
                        // Handle exception
                        return StatusCode(500, $"Internal server error: {ex.Message}");
                    }
                }
            }
        }

        [HttpPost("aceptarordencompra")]
        public async Task<IActionResult> AceptarOrdenCompra(int purchaseOrderId){
            if(purchaseOrderId == null) {
                return BadRequest("Campos faltantes");
            }
            
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE PurchaseOrder SET State = repestado, MessageState = 'Approved' WHERE PurchaseOrderId = reppurchaseorderid;";
                query = query.Replace("repestado", "2");
                query = query.Replace("reppurchaseorderid", purchaseOrderId.ToString());
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

            return Ok(new { status = "success", message = "Orden de compra aceptada" });
        }

        [HttpDelete("eliminarordencompra")]
        public async Task<IActionResult> EliminarOrdenCompra(int purchaseOrderId){
            if(purchaseOrderId == null) {
                return BadRequest("Campos faltantes");
            }
            
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE PurchaseOrder SET IsActive = 0 WHERE PurchaseOrderId = reppurchaseorderid;";
                query = query.Replace("reppurchaseorderid", purchaseOrderId.ToString());
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

            return Ok(new { status = "success", message = "Orden de compra eliminada" });
        }

        [HttpPost("rechazarordencompra")]
        public async Task<IActionResult> RechazarOrdenCompra(int purchaseOrderId, string reason){
            if(purchaseOrderId == null) {
                return BadRequest("Campos faltantes");
            }
            
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE PurchaseOrder SET State = repestado, MessageState = 'Rejected', Reason = 'repreason' WHERE PurchaseOrderId = reppurchaseorderid;";
                query = query.Replace("repestado", "3");
                query = query.Replace("reppurchaseorderid", purchaseOrderId.ToString());
                query = query.Replace("repreason", reason);
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

            return Ok(new { status = "success", message = "Orden de compra rechazada" });
        }

        [HttpGet("getlastpurchaseorder")]
        public async Task<IActionResult> GetLastPurchaseOrder(int companyId){
            if(companyId == null) {
                return BadRequest("Campos faltantes.");
            }
            
            string poNumber = "";
            
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT TOP 1 Number FROM PurchaseOrder WHERE CompanyId = repcompanyid ORDER BY PurchaseOrderId DESC;";
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
                                poNumber = reader.GetString(0);
                            }
                        }

                        if(poNumber == "" || poNumber == null){
                            return Ok("01");
                        }

                        int newpoNumber = int.Parse(poNumber);
                        newpoNumber++;

                        return Ok(newpoNumber.ToString());
                    }
                    catch (SqlException ex)
                    {
                        // Handle exception
                        return StatusCode(500, $"Internal server error: {ex.Message}");
                    }
                }
            }
        }

        [HttpGet("obtenerallpurchaseorder")]
        public async Task<IActionResult> ObtenerAllPurchaseOrder(int companyId){
            if(companyId == null) {
                return BadRequest("Campos faltantes.");
            }
            
            List<PurchaseOrderGet> purchaseOrders = new List<PurchaseOrderGet>();
            
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT PurchaseOrderId, ClientName, Number, LastDate, [state] FROM PurchaseOrder WHERE CompanyId = repcompanyid AND IsActive = 1";
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
                                purchaseOrders.Add(new PurchaseOrderGet
                                {
                                    PurchaseOrderId = reader.GetInt32(0),
                                    ClientName = reader.GetString(1),
                                    Number = reader.GetString(2),
                                    LastDate = reader.GetDateTime(3),
                                    state = reader.GetByte(4)
                                });
                            }
                        }
                        return Ok(purchaseOrders);
                    }
                    catch (SqlException ex)
                    {
                        // Handle exception
                        return StatusCode(500, $"Internal server error: {ex.Message}");
                    }
                }
            }
        }

        [HttpPost("crearpurchaseorder")]
        public async Task<IActionResult> CrearPurchaseOrder(PurchaseOrder purchaseOrder){

            if(purchaseOrder == null){
                return BadRequest("Campos Faltantes.");
            }

            string descripciones = "", productIds = "", quantityList = "";
            int count = 1;
            foreach(var producto in purchaseOrder.ListaProductos){
                if(count == purchaseOrder.ListaProductos.Count){
                    descripciones += producto.ProductName;
                    productIds += producto.ProductId;
                    quantityList += producto.ProductQuantity;
                } else {
                    descripciones += producto.ProductName + ",";
                    productIds += producto.ProductId + ",";
                    quantityList += producto.ProductQuantity + ",";
                }
                count++;
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO PurchaseOrder (Amount, ClientName, Quantity, Number, CreationDate, Direction, NIT, LastDate, Supplier, Seller, [Description], UnitPrice, SubTotal1, SubTotal2, Weekend, [State], MessageState, Reason, ProductsList, CompanyId, QuantityList, ClientId, IsActive) VALUES (repamount, 'repclientname', replaquantity, 'repnumber', 'repcreationdate', 'repdirection', 'repnit', GETDATE(), 'repsupplier', 'repseller', 'repdescription', repunitprice, repsubtotal1, repsubtotal2, repweekend, repstate, 'repmessagestate', 'repreason', 'repproductslist', repcompanyid, 'repquantitylist', repclientid, 1);";
                
                query = query.Replace("repamount", purchaseOrder.Amount.ToString());
                query = query.Replace("repclientname", purchaseOrder.ClientName);
                query = query.Replace("replaquantity", purchaseOrder.Quantity.ToString());
                query = query.Replace("repnumber", purchaseOrder.Number);
                query = query.Replace("repcreationdate", purchaseOrder.CreationDate.ToString());
                query = query.Replace("repdirection", purchaseOrder.Direction);
                query = query.Replace("repnit", purchaseOrder.NIT);
                query = query.Replace("repsupplier", purchaseOrder.Supplier);
                query = query.Replace("repseller", purchaseOrder.Seller);
                query = query.Replace("repdescription", purchaseOrder.Description);
                query = query.Replace("repunitprice", "0");
                query = query.Replace("repsubtotal1", purchaseOrder.SubTotal1.ToString());
                query = query.Replace("repsubtotal2", purchaseOrder.SubTotal2.ToString());
                query = query.Replace("repweekend", purchaseOrder.Weekend.ToString());
                query = query.Replace("repstate", "1");
                query = query.Replace("repmessagestate", "Pendiente");
                query = query.Replace("repreason", "No issues");
                query = query.Replace("repproductslist", productIds);
                query = query.Replace("repcompanyid", purchaseOrder.CompanyId.ToString());
                query = query.Replace("repquantitylist", quantityList);
                query = query.Replace("repclientid", purchaseOrder.ClientId.ToString());
                
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

        [HttpPost("editarpurchaseorder")]
        public async Task<IActionResult> EditarPurchaseOrder(PurchaseOrderUpdate purchaseOrder){

            if(purchaseOrder == null){
                return BadRequest("Campos Faltantes.");
            }

            string descripciones = "", productIds = "", quantityList = "";
            decimal UnitPrice = 0;
            int count = 1;
            foreach(var producto in purchaseOrder.ListaProductos){
                if(count == purchaseOrder.ListaProductos.Count){
                    descripciones += producto.ProductName;
                    productIds += producto.ProductId;
                    quantityList += producto.ProductQuantity;
                } else {
                    descripciones += producto.ProductName + ",";
                    productIds += producto.ProductId + ",";
                    quantityList += producto.ProductQuantity + ",";
                }
                UnitPrice += producto.UnitPrice;
                count++;
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE PurchaseOrder SET Amount = repamount, ClientName = 'repclientname', Quantity = repquantity, Direction = 'repdirection', NIT = 'repnit', LastDate = GETDATE(), Supplier = 'repsupplier', Seller = 'repseller', [Description] = 'repdescription', UnitPrice = repunitprice, SubTotal1 = repsubtotal1, SubTotal2 = repsubtotal2, Weekend = repweekend, ProductsList = 'repproductslist', QuantityList = 'replaquantitylist', ClientId = repclientid WHERE PurchaseOrderId = reppurchaseid;";
                
                query = query.Replace("reppurchaseid", purchaseOrder.PurchaseOrderId.ToString());
                query = query.Replace("repamount", purchaseOrder.Amount.ToString());
                query = query.Replace("repclientname", purchaseOrder.ClientName);
                query = query.Replace("repquantity", purchaseOrder.Quantity.ToString());
                query = query.Replace("repdirection", purchaseOrder.Direction);
                query = query.Replace("repnit", purchaseOrder.NIT);
                query = query.Replace("repsupplier", purchaseOrder.Supplier);
                query = query.Replace("repseller", purchaseOrder.Seller);
                query = query.Replace("repdescription", purchaseOrder.Description);
                query = query.Replace("repunitprice", UnitPrice.ToString());
                query = query.Replace("repsubtotal1", purchaseOrder.SubTotal1.ToString());
                query = query.Replace("repsubtotal2", purchaseOrder.SubTotal2.ToString());
                query = query.Replace("repweekend", purchaseOrder.Weekend.ToString());
                query = query.Replace("repproductslist", productIds);
                query = query.Replace("repcompanyid", purchaseOrder.CompanyId.ToString());
                query = query.Replace("replaquantitylist", quantityList);
                query = query.Replace("repclientid", purchaseOrder.ClientId.ToString());
                
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

            return Ok(new { status = "success", message = "Orden de compra editada correctamente" });
        }
    }
}