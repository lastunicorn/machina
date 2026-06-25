namespace DustInTheWind.Machina;

/// <summary>
/// A self-identifying state that carries both its enum key and its execution logic.
/// </summary>
public interface IState<TState, in TContext> : IStateExecutor<TState, TContext>
    where TState : struct, Enum
{
    TState Id { get; }
}
