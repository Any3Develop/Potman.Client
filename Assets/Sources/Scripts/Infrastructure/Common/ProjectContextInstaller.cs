using Potman.Infrastructure.Common.InputSystem;
using Potman.Infrastructure.Common.UIService;
using Zenject;

namespace Potman.Infrastructure.Common
{
    public class ProjectContextInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            InputSystemInstaller.Install(Container);
            UIServiceInstaller.Install(Container);
        }
    }
}