namespace DustInTheWind.Machina;

/// <summary>
/// Context-free variant of <see cref="StateMachine{TStateId,TContext}"/> for state machines with no shared data.
/// </summary>
public class StateMachine<TStateId>
    where TStateId : struct, Enum
{
    private readonly object dummyContext = new();
    private readonly StateMachine<TStateId, object> inner = new();

    public TStateId? InitialStateId
    {
        get => inner.InitialStateId;
        set => inner.InitialStateId = value;
    }

    public TStateId? CurrentStateId => inner.CurrentStateId;

    public event EventHandler<TransitioningEventArgs<TStateId>> Transitioning
    {
        add => inner.Transitioning += value;
        remove => inner.Transitioning -= value;
    }

    public event EventHandler<TransitionedEventArgs<TStateId>> Transitioned
    {
        add => inner.Transitioned += value;
        remove => inner.Transitioned -= value;
    }

    public StateMachine<TStateId> AddState(TStateId key, IState<TStateId> state)
    {
        ArgumentNullException.ThrowIfNull(state);
        inner.AddState(key, _ => state.ExecuteAsync());
        return this;
    }

    public StateMachine<TStateId> AddState(TStateId key, Func<Task<TStateId?>> state)
    {
        ArgumentNullException.ThrowIfNull(state);
        inner.AddState(key, _ => state());
        return this;
    }

    public StateMachine<TStateId> AddState(TStateId key, Func<TStateId?> state)
    {
        ArgumentNullException.ThrowIfNull(state);
        inner.AddState(key, _ => state());
        return this;
    }

    public Task RunAsync()
    {
        return inner.RunAsync(dummyContext);
    }

    public void Start()
    {
        inner.Start(dummyContext);
    }

    public Task<bool> MoveNextAsync()
    {
        return inner.MoveNextAsync();
    }
}
