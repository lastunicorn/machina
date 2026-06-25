namespace DustInTheWind.Machina;

/// <summary>
/// Creates state instances on demand. Implement this to control how states are instantiated
/// (e.g. via a DI container).
/// </summary>
public interface IStateFactory
{
    object Create(Type stateType);
}
