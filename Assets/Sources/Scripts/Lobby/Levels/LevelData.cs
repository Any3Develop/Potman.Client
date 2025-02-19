using Potman.Game.Scenarios.Data;
using Potman.Lobby.Identity.Abstractions;

namespace Potman.Lobby.Levels
{
    public class LevelData : IRedirectionArg
    {
        public ScenarioId Id { get; set; }
        public int Difficulty { get; set; }
    }
}