using DustInTheWind.Machina.Demo.States;

namespace DustInTheWind.Machina.Demo;

internal static class Program
{
	private static async Task Main(string[] args)
	{
		OrderContext order = new()
		{
			CustomerName = "Alice",
			Items =
			[
				"Wireless Keyboard",
				"USB Hub",
				"Mousepad"
			],
			ShippingAddress = "123 Main St, Springfield"
		};

		StateMachine<OrderState, OrderContext> machine = CreateStateMachine();

		Console.WriteLine("=== Order Processing Demo ===");
		Console.WriteLine();

		await machine.RunAsync(order);

		Console.WriteLine();
		Console.WriteLine("=== Done ===");
	}

	private static StateMachine<OrderState, OrderContext> CreateStateMachine()
	{
		StateMachine<OrderState, OrderContext> machine = new();

		machine.AddState(OrderState.Validating, new ValidatingState());
		machine.AddState(OrderState.Packaging, new PackagingState());
		machine.AddState(OrderState.Shipping, new ShippingState());
		machine.AddState(OrderState.Delivering, new DeliveringState());
		return machine;
	}
}