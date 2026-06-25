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

    public TransitioningEventArgs(Type from)
    {
        From = from;
    }
}
