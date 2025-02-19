namespace Potman.Game.Player.Abstractions
{
    public interface IPlayerAnimatorFactory
    {
        IPlayerAnimator Create(params object[] args);
    }
}