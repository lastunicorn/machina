namespace DustInTheWind.Machina;

public abstract class StateBase<TStateId, TContext> : IState<TStateId, TContext>
	where TStateId : struct, Enum
	where TContext : class
{
	public abstract Task<TStateId?> ExecuteAsync(TContext context);

	protected Task<TStateId?> Next(TStateId nextState)
	{
		return StateResult.Next(nextState);
	}

	protected Task<TStateId?> Stop()
	{
		return StateResult.Stop<TStateId>();
	}
}