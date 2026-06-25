namespace DustInTheWind.Machina.Demo.States;

internal class ChargingPaymentState : IState<OrderState, OrderContext>
{
    public Task<OrderState?> ExecuteAsync(OrderContext context)
    {
        Console.WriteLine("[ChargingPayment] Charging card on file for customer: {0}", context.CustomerName);
        Console.WriteLine("[ChargingPayment] Payment successful.");
        
        return StateResult.Next(OrderState.Packaging);
    }
}
