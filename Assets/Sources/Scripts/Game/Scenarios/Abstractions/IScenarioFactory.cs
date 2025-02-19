using Potman.Game.Scenarios.Data;

namespace Potman.Game.Scenarios.Abstractions
{
    public interface IScenarioFactory
    {
        IScenario Create(ScenarioConfig config);
    }
}