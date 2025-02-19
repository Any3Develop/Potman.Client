namespace Potman.Game.Units.Abstractions
{
    public interface IUnitViewFactory
    {
        IUnitView Create(IUnitViewModel value);
    }
}