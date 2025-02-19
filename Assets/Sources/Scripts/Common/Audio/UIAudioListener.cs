using System.Collections.Generic;
using Potman.Common.UIService.Abstractions;

namespace Potman.Common.Audio
{
	public class UIAudioListener : IUIAudioListener
	{
		// private readonly IAudioApplication audioApplication;
		// public UIAudioListener (IAudioApplication audioApplication)
		// {
		// 	this.audioApplication = audioApplication;
		// }

		public void Subscribe(IEnumerable<IUIWindow> windows)
		{
			// windows?.ForEach(x => x.AudioSource.OnPlayAudio += data => audioApplication.PlaySound(data));
		}
	}
}