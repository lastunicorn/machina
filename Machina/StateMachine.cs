namespace DustInTheWind.Machina;

/// <summary>
/// Context-free variant of <see cref="StateMachine{TContext}"/> for state machines with no shared data.
/// </summary>
public class StateMachine
{
	private readonly HashSet<Type> stateTypes = [];

	/// <summary>
	/// The first state the machine enters when <see cref="Start"/> or <see cref="RunAsync"/> is called.
	/// Automatically set to the first state registered via <see cref="AddState{TState}"/>
	/// unless assigned explicitly.
	/// </summary>
	public Type InitialState { get; set; }

	/// <summary>
	/// The type of the currently active state, or <c>null</c> if the machine has not been
	/// started or has stopped.
	/// </summary>
	public Type CurrentState { get; private set; }

	/// <summary>
	/// The factory used to instantiate state objects on each transition.
	/// Defaults to <see cref="DefaultStateFactory"/> when <c>null</c>, which requires
	/// state types to have a public parameterless constructor.
	/// Set this to integrate with a DI container.
	/// </summary>
	public IStateFactory StateFactory { get; set; }

	/// <summary>
	/// Raised when a new execution of the state machine is about to start, before the <see cref="CurrentState"/> is set to <see cref="InitialState"/>.
	/// </summary>
	public event EventHandler Starting;

	/// <summary>
	/// Raised when a new execution of the state machine has started.
	/// </summary>
	public event EventHandler Started;

	/// <summary>
	/// Raised after the last state has been executed, and <see cref="CurrentState"/> has transitioned to <c>null</c>,
	/// indicating the machine has finished executing all states.
	/// </summary>
	public event EventHandler Finished;

	/// <summary>
	/// Raised before each state's <see cref="IState.ExecuteAsync"/> is called.
	/// </summary>
	public event EventHandler<TransitioningEventArgs> Transitioning;

	/// <summary>
	/// Raised after each state's <see cref="IState.ExecuteAsync"/> returns and
	/// <see cref="CurrentState"/> has been updated to the next state.
	/// </summary>
	public event EventHandler<TransitionedEventArgs> Transitioned;

	/// <summary>
	/// Registers a state type with the machine.
	/// The first state added becomes <see cref="InitialState"/> unless that property has already been set.
	/// </summary>
	/// <typeparam name="TState">The state implementation to register. Each type may be registered at most once.</typeparam>
	/// <returns>This instance, to allow fluent chaining of <c>AddState</c> calls.</returns>
	/// <exception cref="ArgumentException">Thrown when <typeparamref name="TState"/> has already been registered.</exception>
	public StateMachine AddState<TState>()
		where TState : class, IState
	{
		Type stateType = typeof(TState);
		bool isFirstState = stateTypes.Count == 0;

		bool added = stateTypes.Add(stateType);

		if (!added)
			throw new ArgumentException($"State '{stateType.Name}' is already registered.");

		if (isFirstState && InitialState == null)
			InitialState = stateType;

		return this;
	}

	/// <summary>
	/// Starts the machine at <see cref="InitialState"/> and runs it to completion,
	/// executing states sequentially until one returns <c>null</c>.
	/// </summary>
	public async Task RunAsync()
	{
		Start();

		while (CurrentState != null)
			await MoveNextAsync();
	}

	/// <summary>
	/// Sets <see cref="CurrentState"/> to <see cref="InitialState"/> without executing any states.
	/// Call <see cref="MoveNextAsync"/> afterwards to step through states manually.
	/// Raises <see cref="Starting"/> before and <see cref="Started"/> after the state is set.
	/// </summary>
	public void Start()
	{
		OnStarting(EventArgs.Empty);
		CurrentState = InitialState;
		OnStarted(EventArgs.Empty);
	}

	/// <summary>
	/// Executes <see cref="CurrentState"/>, then advances <see cref="CurrentState"/> to
	/// whichever state <see cref="IState.ExecuteAsync"/> returned.
	/// Raises <see cref="Transitioning"/> before execution and <see cref="Transitioned"/> after.
	/// </summary>
	/// <returns>
	/// <c>true</c> if a state was executed; <c>false</c> if the machine had already stopped
	/// (<see cref="CurrentState"/> was <c>null</c>).
	/// </returns>
	/// <exception cref="InvalidOperationException">
	/// Thrown when <see cref="CurrentState"/> is a type that was not registered via <see cref="AddState{TState}"/>.
	/// </exception>
	public async Task<bool> MoveNextAsync()
	{
		Type fromState = CurrentState;

		if (fromState == null)
			return false;

		if (!stateTypes.Contains(fromState))
			throw new InvalidOperationException($"No state registered for '{fromState.Name}'.");

		TransitioningEventArgs transitioningEventArgs = new(fromState);
		OnTransitioning(transitioningEventArgs);

		IStateFactory factory = StateFactory ?? new DefaultStateFactory();
		IState state = (IState)factory.Create(fromState);
		CurrentState = await state.ExecuteAsync();

		TransitionedEventArgs transitionedEventArgs = new(fromState, CurrentState);
		OnTransitioned(transitionedEventArgs);

		if (CurrentState == null)
			OnFinished(EventArgs.Empty);

		return true;
	}

	/// <summary>
	/// Raises the <see cref="Starting"/> event. Override in a subclass to add
	/// behavior before the machine starts without subscribing to the event.
	/// </summary>
	protected virtual void OnStarting(EventArgs e)
	{
		Starting?.Invoke(this, e);
	}

	/// <summary>
	/// Raises the <see cref="Started"/> event. Override in a subclass to add
	/// behavior after the machine starts without subscribing to the event.
	/// </summary>
	protected virtual void OnStarted(EventArgs e)
	{
		Started?.Invoke(this, e);
	}

	/// <summary>
	/// Raises the <see cref="Finished"/> event. Override in a subclass to add
	/// behavior when the machine finishes without subscribing to the event.
	/// </summary>
	protected virtual void OnFinished(EventArgs e)
	{
		Finished?.Invoke(this, e);
	}

	/// <summary>
	/// Raises the <see cref="Transitioning"/> event. Override in a subclass to add
	/// pre-execution behavior without subscribing to the event.
	/// </summary>
	protected virtual void OnTransitioning(TransitioningEventArgs e)
	{
		Transitioning?.Invoke(this, e);
	}

	/// <summary>
	/// Raises the <see cref="Transitioned"/> event. Override in a subclass to add
	/// post-execution behavior without subscribing to the event.
	/// </summary>
	protected virtual void OnTransitioned(TransitionedEventArgs e)
	{
		Transitioned?.Invoke(this, e);
	}
}
