namespace DustInTheWind.Machina;

public class StateMachine<TContext>
	where TContext : class
{
	private readonly HashSet<Type> states = [];
	private TContext context;

	public Type InitialState { get; set; }

	public Type CurrentState { get; private set; }

	public IStateFactory StateFactory { get; set; }

	public event EventHandler<TransitioningEventArgs> Transitioning;

	public event EventHandler<TransitionedEventArgs> Transitioned;

	public StateMachine<TContext> AddState<TState>()
		where TState : class, IState<TContext>
	{
		Type stateType = typeof(TState);
		bool isFirstState = states.Count == 0;

		bool added = states.Add(stateType);

		if (!added)
			throw new ArgumentException($"State '{stateType.Name}' is already registered.");

		if (isFirstState && InitialState == null)
			InitialState = stateType;

		return this;
	}

	public async Task RunAsync(TContext context)
	{
		ArgumentNullException.ThrowIfNull(context);

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
		Type fromState = CurrentState;

		if (fromState == null)
			return false;

		if (!states.Contains(fromState))
			throw new InvalidOperationException($"No state registered for '{fromState.Name}'.");

		TransitioningEventArgs transitioningEventArgs = new(fromState);
		OnTransitioning(transitioningEventArgs);

		IStateFactory factory = StateFactory ?? new DefaultStateFactory();
		IState<TContext> state = (IState<TContext>)factory.Create(fromState);
		CurrentState = await state.ExecuteAsync(context);

		TransitionedEventArgs transitionedEventArgs = new(fromState, CurrentState);
		OnTransitioned(transitionedEventArgs);

		return true;
	}

	protected virtual void OnTransitioning(TransitioningEventArgs e)
	{
		Transitioning?.Invoke(this, e);
	}

	protected virtual void OnTransitioned(TransitionedEventArgs e)
	{
		Transitioned?.Invoke(this, e);
	}
}
