namespace Potman.Game.Units.Abstractions.Swarming
{
    public interface ISwarmAgentFactory
    {
        ISwarmAgent Create(params object[] args);
    }
}