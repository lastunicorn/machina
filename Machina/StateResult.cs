namespace DustInTheWind.Machina;

/// <summary>
/// Factory methods for returning state transition results without verbose Task.FromResult boilerplate.
/// </summary>
public static class StateResult
{
    /// <summary>
    /// Transitions to the specified next state.
    /// </summary>
    public static Task<TStateId?> Next<TStateId>(TStateId next)
        where TStateId : struct, Enum
    {
        return Task.FromResult<TStateId?>(next);
    }

    /// <summary>
    /// Stops the state machine.
    /// </summary>
    public static Task<TStateId?> Stop<TStateId>()
        where TStateId : struct, Enum
    {
        return Task.FromResult<TStateId?>(null);
    }
}
