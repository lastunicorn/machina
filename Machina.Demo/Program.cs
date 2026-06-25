using DustInTheWind.Machina.Demo.States;

namespace DustInTheWind.Machina.Demo;

/// <summary>
/// This demo shows how to use the <see cref="StateMachine{TContext}"/> class to implement
/// a state machine that processes orders.
/// It creates two orders, one prepaid and one unpaid, and processes them sequentially.
/// </summary>
internal static class Program
{
	private static async Task Main(string[] args)
	{
		StateMachine<OrderContext> machine = CreateStateMachine();

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

	private static StateMachine<OrderContext> CreateStateMachine()
	{
		StateMachine<OrderContext> machine = new();

		machine.Transitioned += (sender, e) =>
		{
			// Write a horizontal line after each step.
			Console.WriteLine("--------------------------------------------------");
		};

		machine.AddState<ValidatingState>();
		machine.AddState<ChargingPaymentState>();
		machine.AddState<PackagingState>();
		machine.AddState<ShippingState>();
		machine.AddState<DeliveringState>();

		return machine;
	}

	private static async Task ProcessOrder(StateMachine<OrderContext> machine, OrderContext order)
	{
		Console.WriteLine("=== Order Processing Demo ===");
		Console.WriteLine();

		await machine.RunAsync(order);

		Console.WriteLine();
		Console.WriteLine("=== Done ===");
	}
}
