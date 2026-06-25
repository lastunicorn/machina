namespace DustInTheWind.Machina;

/// <summary>
/// Provides data for the <see cref="StateMachine{TStateId,TContext}.Transitioned"/> event.
/// </summary>
public class TransitionedEventArgs<TStateId> : EventArgs
    where TStateId : struct, Enum
{
    /// <summary>
    /// The state that just finished executing.
    /// </summary>
    public TStateId From { get; }

    /// <summary>
    /// The state the machine is moving to, or <c>null</c> if the machine has stopped.
    /// </summary>
    public TStateId? To { get; }

    public TransitionedEventArgs(TStateId from, TStateId? to)
    {
        From = from;
        To = to;
    }
}
