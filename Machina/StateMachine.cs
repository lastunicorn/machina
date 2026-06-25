namespace DustInTheWind.Machina;

/// <summary>
/// Context-free variant of <see cref="StateMachine{TContext}"/> for state machines with no shared data.
/// </summary>
public class StateMachine
{
	private readonly HashSet<Type> states = [];

	public Type InitialState { get; set; }

	public Type CurrentState { get; private set; }

	public event EventHandler<TransitioningEventArgs> Transitioning;

	public event EventHandler<TransitionedEventArgs> Transitioned;

	public StateMachine AddState<TState>()
		where TState : class, IState
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

	public async Task RunAsync()
	{
		Start();

		while (CurrentState != null)
			await MoveNextAsync();
	}

	public void Start()
	{
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

		IState state = (IState)Activator.CreateInstance(fromState);
		CurrentState = await state.ExecuteAsync();

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
