namespace DustInTheWind.Machina;

/// <summary>
/// Encapsulates the behavior of a single state.
/// Register via <see cref="StateMachine{TContext}.AddState{TState}"/>.
/// </summary>
public interface IState<in TContext>
{
    /// <summary>
    /// Executes the logic for this state using the shared context.
    /// </summary>
    /// <param name="context">The shared data object passed through all states in the machine.</param>
    /// <returns>
    /// The <see cref="Type"/> of the next state to transition to,
    /// or <c>null</c> to stop the state machine after this state.
    /// </returns>
    Task<Type> ExecuteAsync(TContext context);
}
