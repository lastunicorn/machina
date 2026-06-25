namespace DustInTheWind.Machina;

/// <summary>
/// Provides data for the <see cref="StateMachine{TStateId,TContext}.Transitioning"/> event.
/// </summary>
public class TransitioningEventArgs<TStateId> : EventArgs
    where TStateId : struct, Enum
{
    /// <summary>
    /// The state that is about to execute.
    /// </summary>
    public TStateId From { get; }

    public TransitioningEventArgs(TStateId from)
    {
        From = from;
    }
}
