namespace DustInTheWind.Machina;

/// <summary>
/// Context-free variant of <see cref="IState{TContext}"/> for state machines with no shared data.
/// Register via <see cref="StateMachine.AddState{TState}"/>.
/// </summary>
public interface IState
{
    Task<Type> ExecuteAsync();
}
