namespace Potman.Game.Player.Abstractions
{
    public interface IPlayerViewFactory
    {
        IPlayerView Create(IPlayerViewModel viewModel);
    }
}