namespace DustInTheWind.Machina;

/// <summary>
/// Orchestrates a set of states that share a common context object.
/// States are registered via <see cref="AddState{TState}"/>. The first state added becomes
/// the initial state unless <see cref="InitialState"/> is set explicitly.
/// Call <see cref="RunAsync"/> to run the machine to completion, or
/// <see cref="Start"/> followed by repeated <see cref="MoveNextAsync"/> calls for manual stepping.
/// </summary>
/// <typeparam name="TContext">
/// The type of the shared context object passed to every state's <see cref="IState{TContext}.ExecuteAsync"/>.
/// Must be a reference type.
/// </typeparam>
public class StateMachine<TContext>
	where TContext : class
{
	private readonly HashSet<Type> states = [];
	private TContext context;

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
	/// Raised before each state's <see cref="IState{TContext}.ExecuteAsync"/> is called.
	/// </summary>
	public event EventHandler<TransitioningEventArgs> Transitioning;

	/// <summary>
	/// Raised after each state's <see cref="IState{TContext}.ExecuteAsync"/> returns and
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

	/// <summary>
	/// Starts the machine at <see cref="InitialState"/> and runs it to completion,
	/// executing states sequentially until one returns <c>null</c>.
	/// </summary>
	/// <param name="context">The shared context object passed to every state. Must not be <c>null</c>.</param>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is <c>null</c>.</exception>
	public async Task RunAsync(TContext context)
	{
		ArgumentNullException.ThrowIfNull(context);

		Start(context);

		while (CurrentState != null)
			await MoveNextAsync();
	}

	/// <summary>
	/// Sets <see cref="CurrentState"/> to <see cref="InitialState"/> and stores
	/// <paramref name="context"/> for subsequent <see cref="MoveNextAsync"/> calls,
	/// without executing any states.
	/// </summary>
	/// <param name="context">The shared context object passed to every state. Must not be <c>null</c>.</param>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is <c>null</c>.</exception>
	public void Start(TContext context)
	{
		this.context = context ?? throw new ArgumentNullException(nameof(context));
		CurrentState = InitialState;
	}

	/// <summary>
	/// Executes <see cref="CurrentState"/>, then advances <see cref="CurrentState"/> to
	/// whichever state <see cref="IState{TContext}.ExecuteAsync"/> returned.
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
