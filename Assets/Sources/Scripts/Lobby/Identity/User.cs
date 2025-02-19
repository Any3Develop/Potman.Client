using System.Collections.Generic;
using System.Linq;
using Potman.Game.Scenarios.Data;
using Potman.Lobby.Identity.Abstractions;
using Potman.Lobby.Levels;

namespace Potman.Lobby.Identity
{
    public class RedirectionCollection : List<IRedirectionArg>
    {
        public T GetArg<T>() where T : IRedirectionArg => (T)this.FirstOrDefault(x => x is T);
        public T[] GetArgs<T>() where T : IRedirectionArg => this.OfType<T>().ToArray();
    }
    
    public static class User
    {
        public static readonly RedirectionCollection Redirections = new()
        {
            new LevelData{ Id = ScenarioId.Default, Difficulty = 0} //TODO Test
        };
    }
}