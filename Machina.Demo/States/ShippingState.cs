namespace DustInTheWind.Machina.Demo.States;

internal class ShippingState : StateBase<OrderContext>
{
    public override Task<Type> ExecuteAsync(OrderContext context)
    {
        context.TrackingNumber = Guid.NewGuid().ToString("N")[..8].ToUpper();
        
        Console.WriteLine("[Shipping] Dispatched to: {0}", context.ShippingAddress);
        Console.WriteLine("[Shipping] Tracking number: {0}", context.TrackingNumber);
        
        return Next<DeliveringState>();
    }
}
