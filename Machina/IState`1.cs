namespace DustInTheWind.Machina;

/// <summary>
/// Context-free variant of <see cref="IState{TStateId,TContext}"/> for state machines with no shared data.
/// Register instances via <see cref="StateMachine{TStateId}.AddState(TStateId,IState{TStateId})"/>.
/// </summary>
public interface IState<TStateId>
	where TStateId : struct, Enum
{
	Task<TStateId?> ExecuteAsync();
}
