using Potman.Common.UIService.Abstractions;
using Potman.Game.Context.Abstractions;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Potman.Game.Context
{
    public class DefaultCameraProvider : ICameraProvider
    {
        public Camera Camera { get; }
        public CinemachineCamera VirtualCamera { get; }

        public DefaultCameraProvider(IUIRoot uiRoot)
        {
            Camera = Camera.main;
            VirtualCamera = Object.FindAnyObjectByType<CinemachineCamera>();
            var cameraData = Camera!.GetComponent<UniversalAdditionalCameraData>();
            cameraData.cameraStack.Add(uiRoot.UICamera);
        }

        public void SetupPlayerCameraRig(Transform playerRoot)
        {
            VirtualCamera.Target.TrackingTarget = playerRoot;
        }
    }
}