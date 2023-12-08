using UnityEngine;
using UnityEngine.InputSystem;

namespace MercivKit
{
    public class CameraController : MonoBehaviour, MercivInputActions.IPlayerActions
    {
        [SerializeField]
        Transform _target;
        public Transform target
        {
            get => _target;
            set => _target = value;
        }

        [SerializeField]
        private float _smoothing = 0.1f;
        public float Smoothing
        {
            get => _smoothing;
            set => _smoothing = value;
        }

        [SerializeField]
        private float _rotateSpeed = 0.1f;
        public float RotateSpeed
        {
            get => _rotateSpeed;
            set => _rotateSpeed = value;
        }

        [SerializeField]
        private float _pitch = 10;
        public float Pitch
        {
            get => _pitch;
            set => _pitch = value;
        }

        [SerializeField]
        private float _yaw;
        public float Yaw
        {
            get => _yaw;
            set => _yaw = value;
        }

        [SerializeField]
        private float _height = 2f;
        public float Height
        {
            get => _height;
            set => _height = value;
        }

        [SerializeField]
        private float _distance = 3f;
        public float Distance
        {
            get => _distance;
            set => _distance = value;
        }

        private MercivInputActions _inputActions;
        private float _currentPitch;
        private float _currentYaw;

        private void Awake()
        {
            _inputActions = new MercivInputActions();
            _inputActions.Player.SetCallbacks(this);
        }

        private void OnEnable()
        {
            _inputActions.Player.Enable();
        }

        private void OnDisable()
        {
            _inputActions.Player.Disable();
        }

        public void OnMove(InputAction.CallbackContext context) {}

        public void OnLook(InputAction.CallbackContext context)
        {
            if (!_target)
            {
                return;
            }

            var lookInput = context.ReadValue<Vector2>();
            _pitch += -lookInput.y * _rotateSpeed;
            _yaw += lookInput.x * _rotateSpeed;
            _pitch = Mathf.Clamp(_pitch, -30, 70f);
        }

        public void OnFire(InputAction.CallbackContext context) {}

        private void LateUpdate()
        {
            if (!_target)
            {
                return;
            }

            var smoothPitch = Mathf.Lerp(_currentPitch, _pitch, _smoothing * Time.smoothDeltaTime);
            var smoothYaw = Mathf.Lerp(_currentYaw, _yaw, _smoothing * Time.smoothDeltaTime);
            _currentPitch = smoothPitch;
            _currentYaw = smoothYaw;
            transform.rotation = Quaternion.Euler(smoothPitch, smoothYaw, 0);

            var heightPosition = _target.position + Vector3.up * _height;
            var currentDistance = Vector3.Distance(transform.position, heightPosition);
            var smoothDistance = Mathf.Lerp(currentDistance, _distance, _smoothing * Time.smoothDeltaTime);
            transform.position = heightPosition - transform.forward * smoothDistance;
        }
    }
}