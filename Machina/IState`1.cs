namespace DustInTheWind.Machina;

/// <summary>
/// Encapsulates the behavior of a single state.
/// Register via <see cref="StateMachine{TContext}.AddState{TState}"/>.
/// </summary>
public interface IState<in TContext>
{
    Task<Type> ExecuteAsync(TContext context);
}
