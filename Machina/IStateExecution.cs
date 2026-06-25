namespace DustInTheWind.Machina;

/// <summary>
/// Encapsulates the behavior of a single state without binding it to a specific state identifier.
/// Register instances via <see cref="StateMachine{TState,TContext}.AddState(TState,IStateExecution{TState,TContext})"/>.
/// </summary>
public interface IStateExecution<TStateId, in TContext>
    where TStateId : struct, Enum
{
    Task<TStateId?> ExecuteAsync(TContext context);
}
