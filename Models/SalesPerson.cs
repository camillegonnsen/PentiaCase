using Microsoft.AspNetCore.SignalR;

public class SalesPerson{
    public int Id {get; set;}
    public string? Name{get; set;}
    public string? HireDate{get; set;}
    public string? Address{get; set;}
    public string? City{get; set;}
    public string? ZipCode{get; set;}
    
    public IEnumerable<Order>? Orders {get; set;}
}