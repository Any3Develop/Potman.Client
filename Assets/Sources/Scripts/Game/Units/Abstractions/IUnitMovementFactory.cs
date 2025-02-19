namespace Potman.Game.Units.Abstractions
{
    public interface IUnitMovementFactory
    {
        IUnitMovement Create(params object[] args);
    }
}