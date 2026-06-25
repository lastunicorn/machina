using DustInTheWind.Machina.Demo.States;

namespace DustInTheWind.Machina.Demo;

/// <summary>
/// This demo shows how to use the <see cref="StateMachine{TStateId,TContext}"/> class to implement
/// a state machine that processes orders.
/// It creates two orders, one prepaid and one unpaid, and processes them sequentially.
/// </summary>
internal static class Program
{
	private static async Task Main(string[] args)
	{
		StateMachine<OrderState, OrderContext> machine = CreateStateMachine();

		// Create and process a prepaid order.
		// The processing of the prepaid order is linear, going through a number of steps in sequence:
		// 1. Validating
		// 2. Packaging
		// 3. Shipping
		// 4. Delivering

		OrderContext prepaidOrder = new()
		{
			CustomerName = "Alice",
			Items =
			[
				"Wireless Keyboard",
				"USB Hub",
				"Mousepad"
			],
			ShippingAddress = "123 Main St, Springfield",
			IsPrepaid = true
		};
		await ProcessOrder(machine, prepaidOrder);

		Console.WriteLine();

		// Create and process an unpaid order.
		// The processing of the unpaid order requires an additional step (the payment step) before
		// packaging that the state machine is able to handle automatically:
		// 1. Validating
		// 2. ChargingPayment
		// 3. Packaging
		// 4. Shipping
		// 5. Delivering
		

		OrderContext unpaidOrder = new()
		{
			CustomerName = "Bob",
			Items =
			[
				"Standing Desk"
			],
			ShippingAddress = "456 Elm Ave, Shelbyville",
			IsPrepaid = false
		};

		await ProcessOrder(machine, unpaidOrder);
	}

	private static StateMachine<OrderState, OrderContext> CreateStateMachine()
	{
		StateMachine<OrderState, OrderContext> machine = new(OrderState.Validating);

		machine.Transitioned += (sender, e) =>
		{
			// Write a horizontal line after each step.
			Console.WriteLine("--------------------------------------------------");
		};

		machine.AddState(OrderState.Validating, new ValidatingState());
		machine.AddState(OrderState.ChargingPayment, new ChargingPaymentState());
		machine.AddState(OrderState.Packaging, new PackagingState());
		machine.AddState(OrderState.Shipping, new ShippingState());
		machine.AddState(OrderState.Delivering, new DeliveringState());

		return machine;
	}

	private static async Task ProcessOrder(StateMachine<OrderState, OrderContext> machine, OrderContext order)
	{
		Console.WriteLine("=== Order Processing Demo ===");
		Console.WriteLine();

		await machine.RunAsync(order);

		Console.WriteLine();
		Console.WriteLine("=== Done ===");
	}
}