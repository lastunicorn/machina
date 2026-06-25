namespace DustInTheWind.Machina;

/// <summary>
/// Encapsulates the behavior of a single state.
/// Register instances via <see cref="StateMachine{TStateId,TContext}.AddState(TStateId,IState{TStateId,TContext})"/>.
/// </summary>
public interface IState<TStateId, in TContext>
    where TStateId : struct, Enum
{
    Task<TStateId?> ExecuteAsync(TContext context);
}
