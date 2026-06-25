namespace DustInTheWind.Machina;

/// <summary>
/// Encapsulates the behavior of a single state without binding it to a specific state identifier.
/// Register instances via <see cref="StateMachine{TState,TContext}.AddState(TState,IStateExecutor{TState,TContext})"/>.
/// </summary>
public interface IStateExecutor<TState, in TContext>
    where TState : struct, Enum
{
    Task<TState?> ExecuteAsync(TContext context);
}
