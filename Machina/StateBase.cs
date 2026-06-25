namespace DustInTheWind.Machina;

public abstract class StateBase<TContext> : IState<TContext>
    where TContext : class
{
    public abstract Task<Type> ExecuteAsync(TContext context);

    protected Task<Type> Next<TNext>()
        where TNext : class, IState<TContext>
    {
        return StateResult.Next<TNext>();
    }

    protected Task<Type> Stop()
    {
        return StateResult.Stop();
    }
}
