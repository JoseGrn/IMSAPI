public class ShipmentCreate
{
    public int CompanyId { get; set; }
    public string Number { get; set; }
    public string ShippingNote { get; set; }
    public DateTime? DepartureDate { get; set; }
    public byte? Week { get; set; } //falta
    public string ClientName { get; set; }
    public string Destination { get; set; }
    public decimal? CubicMeters { get; set; } //falta
    public string Label { get; set; } //falta
    public string? DepartureTime { get; set; }
    public string Transportation { get; set; } //falta
    public string TrailerPlate { get; set; } //falta
    public string ContainerPlate { get; set; } //falta
    public string Pilot { get; set; } //falta
    public string Responsible { get; set; } //falta
    public string Observation { get; set; }
    public int ClientId { get; set; }
    public List<ShipmentProducts> Products{ get; set; }
}

public class ShipmentEdit
{
    public int ShipmentId { get; set; }
    public string Number { get; set; }
    public string ShippingNote { get; set; }
    public DateTime? DepartureDate { get; set; }
    public byte? Week { get; set; }
    public string ClientName { get; set; }
    public string Destination { get; set; }
    public decimal? CubicMeters { get; set; }
    public string Label { get; set; }
    public string? DepartureTime { get; set; }
    public string Transportation { get; set; }
    public string TrailerPlate { get; set; }
    public string ContainerPlate { get; set; }
    public string Pilot { get; set; }
    public string Observation { get; set; }
    public int ClientId { get; set; }
    public List<ShipmentProducts> Products{ get; set; }
}

public class ShipmentGetById
{
    public int ShipmentId { get; set; }
    public string Number { get; set; }
    public string ShippingNote { get; set; }
    public DateTime? DepartureDate { get; set; }
    public byte? Week { get; set; }
    public string ClientName { get; set; }
    public string Destination { get; set; }
    public decimal? CubicMeters { get; set; }
    public string Label { get; set; }
    public string? DepartureTime { get; set; }
    public string Transportation { get; set; }
    public string TrailerPlate { get; set; }
    public string ContainerPlate { get; set; }
    public string Pilot { get; set; }
    public string Observation { get; set; }
    public int ClientId { get; set; }
    public List<ShipmentProducts> Products{ get; set; }
}

public class GetShipment {
    public int ShipmentId { get; set; }
    public string Number { get; set; }
    public string ClientName { get; set; }
    public DateTime? DepartureDate { get; set; }
}

public class ShipmentProducts {
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}