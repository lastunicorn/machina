namespace DustInTheWind.Machina;

/// <summary>
/// Context-free variant of <see cref="IState{TContext}"/> for state machines with no shared data.
/// Register via <see cref="StateMachine.AddState{TState}"/>.
/// </summary>
public interface IState
{
    /// <summary>
    /// Executes the logic for this state.
    /// </summary>
    /// <returns>
    /// The <see cref="Type"/> of the next state to transition to,
    /// or <c>null</c> to stop the state machine after this state.
    /// Use <see cref="StateResult"/> factory methods to avoid <see cref="Task.FromResult"/> boilerplate.
    /// </returns>
    Task<Type> ExecuteAsync();
}
