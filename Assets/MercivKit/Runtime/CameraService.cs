using Cinemachine;
using UnityEngine;

namespace MercivKit
{
    public class CameraService : ServiceBase
    {
        [SerializeField]
        private CinemachineFreeLook _camera;

        public void SetFollowTarget(Transform transform)
        {
            _camera.Follow = transform;
            _camera.LookAt = transform;
        }
    }
}