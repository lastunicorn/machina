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

		await DemoPrepaidOrder(machine);
		Console.WriteLine();
		await DemoUnpaidOrder(machine);
	}

	private static StateMachine<OrderContext> CreateStateMachine()
	{
		StateMachine<OrderContext> machine = new();
		machine.Transitioned += HandleMachineTransitioned;
		machine.Starting += HandleMachineStarting;
		machine.Finished += HandleMachineFinished;

		machine.AddState<ValidatingState>();
		machine.AddState<ChargingPaymentState>();
		machine.AddState<PackagingState>();
		machine.AddState<ShippingState>();
		machine.AddState<DeliveringState>();

		return machine;
	}

	private static void HandleMachineStarting(object sender, EventArgs e)
	{
		Console.WriteLine("=== Order Processing Demo ===");
		Console.WriteLine();
	}

	private static void HandleMachineFinished(object sender, EventArgs e)
	{
		Console.WriteLine();
		Console.WriteLine("=== Done ===");
	}

	private static void HandleMachineTransitioned(object sender, TransitionedEventArgs e)
	{
		// Write a horizontal line after each step except the last one.
		
		if (e.To != null)
			Console.WriteLine("--------------------------------------------------");
	}

	private static async Task DemoPrepaidOrder(StateMachine<OrderContext> machine)
	{
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
		
		await machine.RunAsync(prepaidOrder);
	}

	private static async Task DemoUnpaidOrder(StateMachine<OrderContext> machine)
	{
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

		await machine.RunAsync(unpaidOrder);
	}
}