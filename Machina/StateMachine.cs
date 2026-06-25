namespace DustInTheWind.Machina;

public class StateMachine<TStateId, TContext>
	where TStateId : struct, Enum
{
	private readonly Dictionary<TStateId, IStateExecution<TStateId, TContext>> statesById = new();
	private TContext context;

	public TStateId? InitialState { get; set; }

	public TStateId? CurrentState { get; private set; }

	public StateMachine<TStateId, TContext> AddState(TStateId key, IStateExecution<TStateId, TContext> execution)
	{
		ArgumentNullException.ThrowIfNull(execution);
		AddStateInternal(key, execution);
		return this;
	}

	public StateMachine<TStateId, TContext> AddState(TStateId key, Func<TContext, Task<TStateId?>> execution)
	{
		ArgumentNullException.ThrowIfNull(execution);
		AddStateInternal(key, new DelegateStateExecution<TStateId, TContext>(execution));
		return this;
	}

	public StateMachine<TStateId, TContext> AddState(TStateId key, Func<TContext, TStateId?> execution)
	{
		ArgumentNullException.ThrowIfNull(execution);
		DelegateStateExecution<TStateId, TContext> delegateStateExecution = new(ctx => Task.FromResult(execution(ctx)));
		AddStateInternal(key, delegateStateExecution);
		return this;
	}

	private void AddStateInternal(TStateId key, IStateExecution<TStateId, TContext> execution)
	{
		bool isFirstState = statesById.Count == 0;

		bool success = statesById.TryAdd(key, execution);

		if (!success)
			throw new ArgumentException($"A state with id '{key}' is already registered.");

		if (isFirstState && InitialState == null)
			InitialState = key;
	}

	public async Task ExecuteAllAsync(TContext context)
	{
		Start(context);

		while (CurrentState != null)
			await MoveNextAsync();
	}

	public void Start(TContext context)
	{
		this.context = context ?? throw new ArgumentNullException(nameof(context));
		CurrentState = InitialState;
	}

	public async Task<bool> MoveNextAsync()
	{
		TStateId? currentState = CurrentState;

		if (!currentState.HasValue)
			return false;

		bool stateFound = statesById.TryGetValue(currentState.Value, out IStateExecution<TStateId, TContext> stateExecution);
		if (!stateFound)
			throw new InvalidOperationException($"No state registered for '{currentState.Value}'.");

		CurrentState = await stateExecution.ExecuteAsync(context);

		return true;
	}
}