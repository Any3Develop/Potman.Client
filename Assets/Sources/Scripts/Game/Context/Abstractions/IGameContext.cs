using Potman.Game.Player.Abstractions;
using Potman.Game.Scenarios.Data;

namespace Potman.Game.Context.Abstractions
{
    public interface IGameContext
    {
        IPlayerViewModel Player { get; }
        int Time { get; }
        int TimeLeft { get; }
        
        int UnitsMax { get; }
        int UnitsAlive { get; }
        int UnitsSpawned { get; }
        int UnitsDied { get; }
        int UnitsLeft { get; }
        
        
        void Pause(bool value);
        void Start(ScenarioConfig config);
        void End();
    }
}