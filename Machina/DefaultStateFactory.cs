namespace DustInTheWind.Machina;

/// <summary>
/// Creates state instances using <see cref="Activator.CreateInstance"/>.
/// Requires states to have a public parameterless constructor.
/// </summary>
public class DefaultStateFactory : IStateFactory
{
    public object Create(Type stateType)
    {
        return Activator.CreateInstance(stateType);
    }
}
