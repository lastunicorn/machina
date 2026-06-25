namespace DustInTheWind.Machina.Demo.States;

internal class DeliveringState : IState<OrderState, OrderContext>
{
    public Task<OrderState?> ExecuteAsync(OrderContext context)
    {
        Console.WriteLine("[Delivering] Package {0} delivered to {1}.", context.TrackingNumber, context.CustomerName);
        
        return StateResult.Stop<OrderState>();
    }
}
