namespace DustInTheWind.Machina.Demo.States;

internal class ShippingState : StateBase<OrderState, OrderContext>
{
    public override Task<OrderState?> ExecuteAsync(OrderContext context)
    {
        context.TrackingNumber = Guid.NewGuid().ToString("N")[..8].ToUpper();
        
        Console.WriteLine("[Shipping] Dispatched to: {0}", context.ShippingAddress);
        Console.WriteLine("[Shipping] Tracking number: {0}", context.TrackingNumber);
        
        return Next(OrderState.Delivering);
    }
}
