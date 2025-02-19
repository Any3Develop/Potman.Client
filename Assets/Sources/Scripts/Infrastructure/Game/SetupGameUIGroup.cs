using Potman.Common.Audio;
using Potman.Common.UIService;
using Potman.Common.UIService.Abstractions;
using Potman.Game.UI;
using UnityEngine;
using Zenject;

namespace Potman.Infrastructure.Game
{
    public class SetupGameUIGroup : IInitializable
    {
        private readonly IUIService uiService;
        private readonly IUIAudioListener audioListener;

        public SetupGameUIGroup(IUIService uiService, IUIAudioListener audioListener)
        {
            this.uiService = uiService;
            this.audioListener = audioListener;
        }

        public void Initialize()
        {
            audioListener.Subscribe(uiService.CreateAll(UILayer.GameUIGroup));
            
            if (Application.isMobilePlatform)
                uiService.Begin<MobileInputWindow>().Show();
            
            // uiService.Begin<BackgroundWindow>()
            // 	.WithMove(uiService.UIRoot.ButtomContainer, 0)
            // 	.WithInit(window => window.SetDefaultImage())
            // 	.Show();
        }
    }
}