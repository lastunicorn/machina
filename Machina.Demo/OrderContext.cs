namespace DustInTheWind.Machina.Demo;

internal class OrderContext
{
    public string CustomerName { get; init; }
    
    public List<string> Items { get; init; }
    
    public string ShippingAddress { get; init; }
    
    public string TrackingNumber { get; set; }
}
