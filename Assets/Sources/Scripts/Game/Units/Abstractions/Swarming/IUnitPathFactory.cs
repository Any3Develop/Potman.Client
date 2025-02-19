namespace Potman.Game.Units.Abstractions.Swarming
{
    public interface IUnitPathFactory
    {
        IUnitPath Create(int owner);
    }
}