namespace DustInTheWind.Machina;

/// <summary>
/// Creates state instances on demand. Implement this to control how states are instantiated
/// (e.g. via a DI container).
/// </summary>
public interface IStateFactory
{
    /// <summary>
    /// Creates and returns an instance of the specified state type.
    /// </summary>
    /// <param name="stateType">
    /// The concrete state type to instantiate. The state machine passes the registered state type here
    /// on each transition; the returned object must implement <see cref="IState"/> or <see cref="IState{TContext}"/>.
    /// </param>
    /// <returns>A new or resolved instance of <paramref name="stateType"/>.</returns>
    object Create(Type stateType);
}
