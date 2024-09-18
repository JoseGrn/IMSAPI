public class GetProduct
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int MinQuantity { get; set; }
    public int Quantity { get; set; }
    public string Specie { get; set; }
    public decimal Price { get; set; }
}

public class Product
{
    public int CompanyId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int MinQuantity { get; set; }
    public int Quantity { get; set; }
    public string Specie { get; set; }
    public decimal Price { get; set; }
}