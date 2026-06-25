namespace DustInTheWind.Machina;

public class StateMachine<TStateId, TContext>
	where TStateId : struct, Enum
{
	private readonly Dictionary<TStateId, IState<TStateId, TContext>> statesById = new();
	private TContext context;

	public TStateId? InitialStateId { get; set; }

	public TStateId? CurrentStateId { get; private set; }

	public StateMachine()
	{
	}

	public StateMachine(TStateId initialStateId)
	{
		InitialStateId = initialStateId;
	}

	public event EventHandler<TransitioningEventArgs<TStateId>> Transitioning;

	public event EventHandler<TransitionedEventArgs<TStateId>> Transitioned;

	public StateMachine<TStateId, TContext> AddState(TStateId key, IState<TStateId, TContext> state)
	{
		ArgumentNullException.ThrowIfNull(state);
		AddStateInternal(key, state);
		return this;
	}

	public StateMachine<TStateId, TContext> AddState(TStateId key, Func<TContext, Task<TStateId?>> state)
	{
		ArgumentNullException.ThrowIfNull(state);
		AddStateInternal(key, new DelegateStateExecution<TStateId, TContext>(state));
		return this;
	}

	public StateMachine<TStateId, TContext> AddState(TStateId key, Func<TContext, TStateId?> state)
	{
		ArgumentNullException.ThrowIfNull(state);
		DelegateStateExecution<TStateId, TContext> delegateStateExecution = new(ctx => Task.FromResult(state(ctx)));
		AddStateInternal(key, delegateStateExecution);
		return this;
	}

	private void AddStateInternal(TStateId key, IState<TStateId, TContext> state)
	{
		bool isFirstState = statesById.Count == 0;

		bool success = statesById.TryAdd(key, state);

		if (!success)
			throw new ArgumentException($"A state with id '{key}' is already registered.");

		if (isFirstState && InitialStateId == null)
			InitialStateId = key;
	}

	public async Task RunAsync(TContext context)
	{
		Start(context);

		while (CurrentStateId != null)
			await MoveNextAsync();
	}

	public void Start(TContext context)
	{
		this.context = context ?? throw new ArgumentNullException(nameof(context));
		CurrentStateId = InitialStateId;
	}

	public async Task<bool> MoveNextAsync()
	{
		TStateId? fromStateId = CurrentStateId;

		if (!fromStateId.HasValue)
			return false;

		bool stateFound = statesById.TryGetValue(fromStateId.Value, out IState<TStateId, TContext> state);
		if (!stateFound)
			throw new InvalidOperationException($"No state registered for '{fromStateId.Value}'.");

		TransitioningEventArgs<TStateId> transitioningEventArgs = new(fromStateId.Value);
		OnTransitioning(transitioningEventArgs);

		CurrentStateId = await state.ExecuteAsync(context);

		TransitionedEventArgs<TStateId> transitionedEventArgs = new(fromStateId.Value, CurrentStateId);
		OnTransitioned(transitionedEventArgs);

		return true;
	}

	protected virtual void OnTransitioning(TransitioningEventArgs<TStateId> e)
	{
		Transitioning?.Invoke(this, e);
	}

	protected virtual void OnTransitioned(TransitionedEventArgs<TStateId> e)
	{
		Transitioned?.Invoke(this, e);
	}
}