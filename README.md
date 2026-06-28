# Machina

[![GitHub Repo](https://img.shields.io/badge/github-repo-blue?logo=github)](https://github.com/lastunicorn/machina) [![GitHub Build](https://img.shields.io/github/actions/workflow/status/lastunicorn/machina/build-master.yml?logo=github)](https://github.com/lastunicorn/machina/actions/workflows/build-master.yml) [![NuGet Version](https://img.shields.io/nuget/v/DustInTheWind.Machina?logo=nuget)](https://www.nuget.org/packages/DustInTheWind.Machina) [![NuGet Downloads](https://img.shields.io/nuget/dt/DustInTheWind.Machina?logo=nuget)](https://www.nuget.org/packages/DustInTheWind.Machina)

A lightweight .NET library for building and running state machines.

## Installation

```
dotnet add package DustInTheWind.Machina
```

## Quick Start

### 1. Define a context

The context carries shared data through every state:

```csharp
public class OrderContext
{
    public Order Order { get; init; }
    public bool PaymentSucceeded { get; set; }
}
```

### 2. Implement the states

Each state is a class. The state's own **type** acts as its identifier. Inherit from `StateBase<TContext>` for concise transition returns, or implement `IState<TContext>` directly. Return the type of the next state to transition to, or `null` to stop the machine.

```csharp
public class ValidateState : StateBase<OrderContext>
{
    public override Task<Type> ExecuteAsync(OrderContext context)
    {
        if (context.Order.Items.Count == 0)
            return Stop(); // signals the machine to stop

        return Next<ReserveState>();
    }
}
```

### 3. Run the state machine

Register states with `AddState<T>()` and call `RunAsync` with the context:

```csharp
StateMachine<OrderContext> machine = new();
machine.AddState<ValidateState>();
machine.AddState<ReserveState>();
machine.AddState<ChargeState>();
machine.AddState<ShipState>();

OrderContext context = new()
{
    Order = order
};
await machine.RunAsync(context);
```

The machine starts at the first registered state and follows the types returned by each state until one returns `null`.

## Other Features

### Manual stepping

Use `Start` and `MoveNextAsync` to drive execution one step at a time:

```csharp
machine.Start(context);

while (await machine.MoveNextAsync())
    Console.WriteLine($"Completed: {machine.CurrentState?.Name}");
```

### Overriding the initial state

By default the first state registered becomes the initial state. Override it by assigning a type:

```csharp
machine.InitialState = typeof(ReserveState);
```

### Events

The following events let you observe the machine's life cycle:

| Event | When it fires |
|---|---|
| `Starting` | When a new execution of the state machine is about to start, before `Start` sets `CurrentState` |
| `Started` | After a new execution of the state machine has started, after `Start` sets `CurrentState` to `InitialState` |
| `Transitioning` | Before the machine transitions to a new state, before the new state is executed |
| `Transitioned` | After the machine transitioned to a new state |
| `Finished` | After the last state was executed. The `CurrentState` is `null` (machine has stopped) |

### Custom state factory (DI integration)

Assign `StateFactory` to control how state objects are created — useful for injecting dependencies:

```csharp
machine.StateFactory = new MyDiStateFactory(serviceProvider);
```

Implement `IStateFactory.Create(Type)` to return an instance from your container. The default factory uses `Activator.CreateInstance` and requires a public parameterless constructor on each state.

## License

MIT
