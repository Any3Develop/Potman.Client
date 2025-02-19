namespace Potman.Game.Units.Abstractions
{
    public interface IUnitViewFollower
    {
        bool Enabled { get; }
        float SpeedDumping { get; }
        float TurnDumping { get; }
        float Altitude { get; }
        IUnitView View { get; }
       
        void Enable(bool value);
    }
}