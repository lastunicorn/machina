namespace DustInTheWind.Machina;

/// <summary>
/// Optional base class for context-aware states. Provides <see cref="Next{TNext}"/> and
/// <see cref="Stop"/> helpers so state implementations can avoid <see cref="Task.FromResult"/> boilerplate.
/// </summary>
/// <typeparam name="TContext">
/// The type of the shared context object. Must be a reference type.
/// </typeparam>
public abstract class StateBase<TContext> : IState<TContext>
    where TContext : class
{
    /// <inheritdoc/>
    public abstract Task<Type> ExecuteAsync(TContext context);

    /// <summary>
    /// Returns a completed task signaling a transition to <typeparamref name="TNext"/>.
    /// </summary>
    /// <typeparam name="TNext">The state to transition to. Must implement <see cref="IState{TContext}"/>.</typeparam>
    /// <returns>A task whose result is <c>typeof(<typeparamref name="TNext"/>)</c>.</returns>
    protected Task<Type> Next<TNext>()
        where TNext : class, IState<TContext>
    {
        return StateResult.Next<TNext>();
    }

    /// <summary>
    /// Returns a completed task signaling the state machine to stop after this state.
    /// </summary>
    /// <returns>A task whose result is <c>null</c>.</returns>
    protected Task<Type> Stop()
    {
        return StateResult.Stop();
    }
}
