# Machina

[![GitHub Repo](https://img.shields.io/badge/github-repo-blue?logo=github)](https://github.com/lastunicorn/machina) [![GitHub Build](https://img.shields.io/github/actions/workflow/status/lastunicorn/machina/build-master.yml?logo=github)](https://github.com/lastunicorn/machina/actions/workflows/build-master.yml) [![NuGet Version](https://img.shields.io/nuget/v/DustInTheWind.Machina.Toolkit?logo=nuget)](https://www.nuget.org/packages/DustInTheWind.Machina.Toolkit) [![NuGet Downloads](https://img.shields.io/nuget/dt/DustInTheWind.Machina.Toolkit?logo=nuget)](https://www.nuget.org/packages/DustInTheWind.Machina.Toolkit)

A lightweight .NET library for building and running state machines.

## Installation

```
dotnet add package DustInTheWind.Machina
```

## Usage

### 1. Define the states as an enum

```csharp
public enum OrderStep
{
    Validate,
    Reserve,
    Charge,
    Ship
}
```

### 2. Define a context

```csharp
public class OrderContext
{
    public Order Order { get; init; }
    public bool PaymentSucceeded { get; set; }
}
```

### 3. Implement a state

Each state returns the next `OrderStep` to transition to, or `null` to stop.

```csharp
public class ValidateState : IState<OrderStep, OrderContext>
{
    public OrderStep Id => OrderStep.Validate;

    public Task<OrderStep?> ExecuteAsync(OrderContext context)
    {
        if (context.Order.Items.Count == 0)
            return Task.FromResult<OrderStep?>(null); // stop — nothing to process

        return Task.FromResult<OrderStep?>(OrderStep.Reserve);
    }
}
```

### 4. Run the state machine

```csharp
StateMachine<OrderStep, OrderContext> machine = new(new IState<OrderStep, OrderContext>[]
{
    new ValidateState(),
    new ReserveState(),
    new ChargeState(),
    new ShipState()
});

OrderContext context = new() { Order = order };
await machine.ExecuteAllAsync(context);
```

The machine starts at `Validate` (the first state added) and follows the transitions returned by each state until one returns `null`.

### Manual stepping

Use `Start` and `MoveNextAsync` to drive execution one step at a time:

```csharp
machine.Start(context);

while (await machine.MoveNextAsync())
{
    Console.WriteLine($"Completed: {machine.CurrentState}");
}
```

### Overriding the initial state

```csharp
machine.InitialState = OrderStep.Reserve;
```

## License

MIT
