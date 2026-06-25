namespace DustInTheWind.Machina.Demo.States;

internal class ChargingPaymentState : StateBase<OrderState, OrderContext>
{
    public override Task<OrderState?> ExecuteAsync(OrderContext context)
    {
        Console.WriteLine("[ChargingPayment] Charging card on file for customer: {0}", context.CustomerName);
        Console.WriteLine("[ChargingPayment] Payment successful.");
        
        return Next(OrderState.Packaging);
    }
}
