namespace DustInTheWind.Machina;

/// <summary>
/// Creates state instances using <see cref="Activator.CreateInstance(Type)"/>.
/// Requires <see cref="IState"/> types being created to have a public parameterless constructor.
/// </summary>
public class DefaultStateFactory : IStateFactory
{
	/// <summary>
	/// Creates an instance of <paramref name="stateType"/> using <see cref="Activator.CreateInstance(Type)"/>.
	/// The state type must have a public parameterless constructor.
	/// </summary>
	/// <param name="stateType">The concrete state type to instantiate. Must implement <see cref="IState"/> or <see cref="IState{TContext}"/>.</param>
	/// <returns>A new instance of <paramref name="stateType"/>.</returns>
	/// <exception cref="ArgumentException">
	/// Thrown when <paramref name="stateType"/> does not implement <see cref="IState"/> or <see cref="IState{TContext}"/>.
	/// </exception>
	public object Create(Type stateType)
	{
		if (stateType == null) throw new ArgumentNullException(nameof(stateType));

		bool implementsIState = typeof(IState).IsAssignableFrom(stateType);
		bool implementsIStateOfT = stateType.GetInterfaces()
			.Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IState<>));

		if (!implementsIState && !implementsIStateOfT)
			throw new ArgumentException($"Type '{stateType.Name}' does not implement IState or IState<TContext>.", nameof(stateType));

		return Activator.CreateInstance(stateType);
	}
}