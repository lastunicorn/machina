namespace DustInTheWind.Machina;

public class StateMachine<TState, TContext>
	where TState : struct, Enum
{
	private readonly Dictionary<TState, IStateExecutor<TState, TContext>> statesById = new();
	private TContext context;

	public TState? InitialState { get; set; }

	public TState? CurrentState { get; private set; }

	public StateMachine()
	{
	}

	public StateMachine(IEnumerable<IState<TState, TContext>> states)
	{
		ArgumentNullException.ThrowIfNull(states);

		foreach (IState<TState, TContext> state in states)
			AddState(state);
	}

	public StateMachine<TState, TContext> AddState(IState<TState, TContext> state)
	{
		ArgumentNullException.ThrowIfNull(state);
		AddStateInternal(state.Id, state);
		return this;
	}

	public StateMachine<TState, TContext> AddState(IEnumerable<IState<TState, TContext>> states)
	{
		ArgumentNullException.ThrowIfNull(states);

		foreach (IState<TState, TContext> state in states)
		{
			ArgumentNullException.ThrowIfNull(state);
			AddStateInternal(state.Id, state);
		}

		return this;
	}

	public StateMachine<TState, TContext> AddState(TState key, IStateExecutor<TState, TContext> executor)
	{
		ArgumentNullException.ThrowIfNull(executor);
		AddStateInternal(key, executor);
		return this;
	}

	private void AddStateInternal(TState key, IStateExecutor<TState, TContext> executor)
	{
		bool isFirstState = statesById.Count == 0;

		bool success = statesById.TryAdd(key, executor);

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
		TState? currentState = CurrentState;

		if (!currentState.HasValue)
			return false;

		if (!statesById.TryGetValue(currentState.Value, out IStateExecutor<TState, TContext> state))
			throw new InvalidOperationException($"No state registered for '{currentState.Value}'.");

		CurrentState = await state.ExecuteAsync(context);

		return true;
	}
}