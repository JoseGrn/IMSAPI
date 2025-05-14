public class PurchaseOrder
{
    public decimal Amount { get; set; }
    public string? ClientName { get; set; }
    public int Quantity { get; set; }
    public string? Number { get; set; }
    public DateTime? CreationDate { get; set; }
    public string? Direction { get; set; }
    public string? NIT { get; set; }
    public DateTime? LastDate { get; set; }
    public string? Supplier { get; set; }
    public string? Seller { get; set; }
    public string? Description { get; set; }
    public decimal? SubTotal1 { get; set; }
    public decimal? SubTotal2 { get; set; }
    public byte? Weekend { get; set; }
    public byte? State { get; set; }
    public string? MessageState { get; set; }
    public string? Reason { get; set; }
    public int? CompanyId { get; set; }
    public List<ProductsList> ListaProductos { get; set; }
    public int ClientId { get; set; }
}

public class ProductsList
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int ProductQuantity { get; set; }
}

public class ProductsListEdit
{
    public int ProductId { get; set; }
    public int ProductQuantity { get; set; }
}

public class PurchaseOrderGet{
    public int PurchaseOrderId { get; set; }
    public string ClientName { get; set; }
    public string Number { get; set; }
    public DateTime LastDate { get; set; }
    public byte state { get; set; }
}

public class PurchaseOrderUpdate
{
    public int PurchaseOrderId { get; set; }
    public decimal Amount { get; set; }
    public string ClientName { get; set; }
    public int Quantity { get; set; }
    public string Direction { get; set; }
    public string NIT { get; set; }
    public DateTime LastDate { get; set; }
    public string Supplier { get; set; }
    public string Seller { get; set; }
    public string? Description { get; set; }
    public decimal SubTotal1 { get; set; } 
    public decimal SubTotal2 { get; set; } 
    public byte Weekend { get; set; }
    public int ClientId { get; set; } = 4;
    public int? CompanyId { get; set; }
    public List<ProductsList> ListaProductos { get; set; }
}

public class PurchaseOrderEdit
{
    public int PurchaseOrderId { get; set; }
    public string? Number { get; set; }
    public string? Direction { get; set; }
    public string? NIT { get; set; }
    public DateTime? LastDate { get; set; }
    public string? Description { get; set; }
    public decimal? SubTotal1 { get; set; }
    public decimal? SubTotal2 { get; set; }
    public byte? Weekend { get; set; }
    public int? CompanyId { get; set; }
    public List<ProductsListEdit> ListaProductos { get; set; }
    public int ClientId { get; set; }
}