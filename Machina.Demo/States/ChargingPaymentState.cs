namespace DustInTheWind.Machina.Demo.States;

internal class ChargingPaymentState : StateBase<OrderContext>
{
    public override Task<Type> ExecuteAsync(OrderContext context)
    {
        Console.WriteLine("[ChargingPayment] Charging card on file for customer: {0}", context.CustomerName);
        Console.WriteLine("[ChargingPayment] Payment successful.");

        return Next<PackagingState>();
    }
}
