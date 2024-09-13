public class Company
{
    public int CompanyId { get; set; }
    public int OwnerId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
}

public class CreateCompany
{
    public int OwnerId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}