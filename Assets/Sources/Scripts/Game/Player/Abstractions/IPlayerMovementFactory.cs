namespace Potman.Game.Player.Abstractions
{
    public interface IPlayerMovementFactory
    {
        IPlayerMovement Create(params object[] args);
    }
}