using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MercivKit
{
    public class FreeLookCameraInput : MonoBehaviour, MercivInputActions.IPlayerActions
    {
        [SerializeField]
        private float _rotateSpeed = 0.1f;
        public float RotateSpeed
        {
            get => _rotateSpeed;
            set => _rotateSpeed = value;
        }

        private MercivInputActions _inputActions;
        private CinemachineFreeLook _freeLookComponent;
        private bool _isDragging;

        private void Awake()
        {
            _inputActions = new MercivInputActions();
            _freeLookComponent = GetComponent<CinemachineFreeLook>();
        }

        private void OnEnable()
        {
            _inputActions.Player.SetCallbacks(this);
            _inputActions.Player.Enable();
        }

        private void OnDisable()
        {
            _inputActions.Player.Disable();
        }

        public void OnMove(InputAction.CallbackContext context) {}

        public void OnLook(InputAction.CallbackContext context)
        {
            if (_isDragging)
            {
                var lookInput = context.ReadValue<Vector2>();
                _freeLookComponent.m_XAxis.Value += lookInput.x * 180 * _rotateSpeed * Time.deltaTime;
                _freeLookComponent.m_YAxis.Value -= lookInput.y * _rotateSpeed * Time.deltaTime;
            }
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            _isDragging = context.ReadValueAsButton();
        }
    }
}