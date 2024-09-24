public class UserClass
{
    public int UserId { get; set; } 
    public int CompanyId { get; set; } 
    public string Username { get; set; } 
    public string Password { get; set; } 
    public string Name { get; set; } 
    public byte? Role { get; set; } 
    public string ProductsIdList { get; set; } 
    public DateTime? ExpirationDate { get; set; } 
}

public class UserCreation
{
    public int CompanyId { get; set; } 
    public string Username { get; set; } 
    public string Password { get; set; } 
    public string Name { get; set; } 
    public byte? Role { get; set; } 
    public string ProductsIdList { get; set; } 
    public DateTime? ExpirationDate { get; set; }
}

public class UserGet
{
    public int UserId { get; set; }
    public int CompanyId { get; set; } 
    public string Username { get; set; } 
    public string Name { get; set; } 
    public byte? Role { get; set; } 
    public string ProductsIdList { get; set; }
}

public class EditUser{
    public int UserId { get; set; }
    public string Username { get; set; } 
    public string Password { get; set; } 
    public string Name { get; set; } 
    public byte? Role { get; set; } 
    public string ProductsIdList { get; set; } 
    public DateTime? ExpirationDate { get; set; } 
}