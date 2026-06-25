namespace DustInTheWind.Machina.Demo.States;

internal class DeliveringState : StateBase<OrderContext>
{
    public override Task<Type> ExecuteAsync(OrderContext context)
    {
        Console.WriteLine("[Delivering] Package {0} delivered to {1}.", context.TrackingNumber, context.CustomerName);
        
        return Stop();
    }
}
