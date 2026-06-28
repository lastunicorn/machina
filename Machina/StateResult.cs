namespace DustInTheWind.Machina;

/// <summary>
/// Factory methods for returning state transition results without verbose <see cref="Task.FromResult"/> boilerplate.
/// </summary>
public static class StateResult
{
    /// <summary>
    /// Transitions to the specified next state.
    /// </summary>
    public static Task<Type> Next<TNext>()
        where TNext : class
    {
        return Task.FromResult(typeof(TNext));
    }

    /// <summary>
    /// Stops the state machine.
    /// </summary>
    public static Task<Type> Stop()
    {
        return Task.FromResult<Type>(null);
    }
}
