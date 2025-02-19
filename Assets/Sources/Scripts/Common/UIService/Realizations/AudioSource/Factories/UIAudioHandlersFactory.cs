﻿using Potman.Common.UIService.Abstractions;
using Potman.Common.UIService.Abstractions.AudioSource;
using Potman.Common.UIService.AudioSource.Configs;
using Potman.Common.UIService.AudioSource.Handlers;

namespace Potman.Common.UIService.AudioSource.Factories
{
    public class UIAudioHandlersFactory : IUIAudioHandlersFactory
    {
        public virtual IUIAudioHandler Create(IUIWindow window, IUIAudioConfig config)
        {
            return config switch
            {
                UIButtonsAudioConfig => new UIButtonsAudioHandler().Init(window,config),
                UIDropDownAudioConfig => new UIDropDownAudioHandler().Init(window,config),
                UISlidersAudioConfig => new UISlidersAudioHandler().Init(window,config),
                UIScrollBarAudioConfig => new UIScrollBarAudioHandler().Init(window,config),
#if TEXTMESHPRO
                UITMPDropDownAudioConfig => new UITMPDropDownAudioHandler().Init(window,config),
                UITMPInputAudioConfig => new UITMPInputAudioHandler().Init(window,config),
#endif
                UIToggleAudioConfig => new UIToggleAudioHandler().Init(window,config),
                UILegacyInputAudioConfig => new UILeagcyInputAudioHandler().Init(window,config),
               
                _ => new UINoAudioHandler(),
            };
        }
    }
}