namespace DustInTheWind.Machina;

/// <summary>
/// Provides data for the <see cref="StateMachine{TContext}.Transitioned"/> event.
/// </summary>
public class TransitionedEventArgs : EventArgs
{
    /// <summary>
    /// The type of the state that just finished executing.
    /// </summary>
    public Type From { get; }

    /// <summary>
    /// The type of the state the machine is moving to, or <c>null</c> if the machine has stopped.
    /// </summary>
    public Type To { get; }

    /// <summary>
    /// Initializes a new instance with the source and destination state types.
    /// </summary>
    /// <param name="from">The state type that just finished executing.</param>
    /// <param name="to">The state type the machine is moving to, or <c>null</c> if the machine has stopped.</param>
    public TransitionedEventArgs(Type from, Type to)
    {
        From = from;
        To = to;
    }
}
