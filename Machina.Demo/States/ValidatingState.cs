namespace DustInTheWind.Machina.Demo.States;

internal class ValidatingState : IState<OrderState, OrderContext>
{
    public Task<OrderState?> ExecuteAsync(OrderContext context)
    {
        Console.WriteLine("[Validating] Checking order for customer: {0}", context.CustomerName);

        if (context.Items == null || context.Items.Count == 0)
        {
            Console.WriteLine("[Validating] Order has no items. Stopping.");
            return StateResult.Stop<OrderState>();
        }

        Console.WriteLine("[Validating] Order contains {0} item(s). Valid.", context.Items.Count);

        if (context.IsPrepaid)
        {
            Console.WriteLine("[Validating] Payment already confirmed. Skipping payment step.");
            return StateResult.Next(OrderState.Packaging);
        }

        Console.WriteLine("[Validating] Payment pending. Proceeding to payment collection.");
        return StateResult.Next(OrderState.ChargingPayment);
    }
}
