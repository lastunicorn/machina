namespace DustInTheWind.Machina.Demo.States;

internal class PackagingState : IState<OrderState, OrderContext>
{
    public Task<OrderState?> ExecuteAsync(OrderContext context)
    {
        Console.WriteLine("[Packaging] Packing items:");
        
        foreach (string item in context.Items)
            Console.WriteLine("  - {0}", item);

        return StateResult.Next(OrderState.Shipping);
    }
}
