namespace DustInTheWind.Machina;

/// <summary>
/// Provides data for the <see cref="StateMachine{TContext}.Transitioning"/> event.
/// </summary>
public class TransitioningEventArgs : EventArgs
{
    /// <summary>
    /// The type of the state that is about to execute.
    /// </summary>
    public Type From { get; }

    /// <summary>
    /// Initializes a new instance with the type of the state that is about to execute.
    /// </summary>
    /// <param name="from">The state type that is about to execute.</param>
    public TransitioningEventArgs(Type from)
    {
        From = from;
    }
}
