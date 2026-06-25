namespace DustInTheWind.Machina.Demo.States;

internal class PackagingState : StateBase<OrderContext>
{
    public override Task<Type> ExecuteAsync(OrderContext context)
    {
        Console.WriteLine("[Packaging] Packing items:");
        
        foreach (string item in context.Items)
            Console.WriteLine("  - {0}", item);

        return Next<ShippingState>();
    }
}
