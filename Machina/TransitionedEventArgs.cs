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

    public TransitionedEventArgs(Type from, Type to)
    {
        From = from;
        To = to;
    }
}
