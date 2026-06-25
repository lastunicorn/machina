namespace DustInTheWind.Machina;

internal sealed class DelegateStateExecution<TStateId, TContext> : IStateExecution<TStateId, TContext>
    where TStateId : struct, Enum
{
    private readonly Func<TContext, Task<TStateId?>> execute;

    public DelegateStateExecution(Func<TContext, Task<TStateId?>> execute)
    {
        this.execute = execute;
    }

    public Task<TStateId?> ExecuteAsync(TContext context)
    {
        return execute?.Invoke(context);
    }
}
