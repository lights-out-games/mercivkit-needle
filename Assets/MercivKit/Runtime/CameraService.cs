using Cinemachine;
using UnityEngine;

namespace MercivKit
{
    public class CameraService : ServiceBase
    {
        [SerializeField]
        private CinemachineFreeLook _camera;

        [SerializeField]
        private CinemachineVirtualCamera _introCamera;

        public void SetFollowTarget(Transform transform)
        {
            _camera.Follow = transform;
            _camera.LookAt = transform;
            _introCamera.gameObject.SetActive(false);
            _camera.gameObject.SetActive(true);
        }
    }
}