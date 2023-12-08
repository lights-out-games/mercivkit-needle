using UnityEngine;
using UnityEngine.InputSystem;

namespace MercivKit
{
    public class PlayerController : MonoBehaviour, MercivInputActions.IPlayerActions
    {
        [SerializeField]
        private Animator _animator;
        public Animator Animator
        {
            get => _animator;
            set => _animator = value;
        }

        [SerializeField]
        private Camera _camera;
        public Camera Camera
        {
            get => _camera;
            set => _camera = value;
        }

        [SerializeField]
        private float _moveSpeed = 5f;
        public float MoveSpeed
        {
            get => _moveSpeed;
            set => _moveSpeed = value;
        }

        [SerializeField]
        private float _cameraSpeed = 1f;
        public float CameraSpeed
        {
            get => _cameraSpeed;
            set => _cameraSpeed = value;
        }

        [SerializeField]
        private float _gravity = 9.8f;
        public float Gravity
        {
            get => _gravity;
            set => _gravity = value;
        }

        private Vector2 _moveInput;
        private CharacterController _characterController;
        private MercivInputActions _inputActions;
        private float _pitch;
        private float _yaw;
        private float _fallSpeed;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _inputActions = new MercivInputActions();
            _inputActions.Player.SetCallbacks(this);
            _camera.transform.rotation = Quaternion.identity;
        }

        private void OnEnable()
        {
            _inputActions.Player.Enable();
        }

        private void OnDisable()
        {
            _inputActions.Player.Disable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            var lookInput = context.ReadValue<Vector2>();
            _pitch += -lookInput.y * _cameraSpeed;
            _yaw += lookInput.x * _cameraSpeed;
            _pitch = Mathf.Clamp(_pitch, -10, 70f);
        }

        public void OnFire(InputAction.CallbackContext context)
        {
        }

        private void Update()
        {
            _characterController.Move(transform.forward * _moveInput.y * _moveSpeed * Time.deltaTime);
            _characterController.Move(Vector3.down * _gravity * Time.deltaTime);

            if (!_characterController.isGrounded)
            {
                _fallSpeed += _gravity * Time.deltaTime;
                _fallSpeed = Mathf.Min(_fallSpeed, 20f);
                _characterController.Move(Vector3.down * _fallSpeed * Time.deltaTime);
            }

            transform.Rotate(Vector3.up, _moveInput.x);
            _animator.SetFloat("MoveSpeed", Mathf.Abs(_moveInput.y));

            _camera.transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
            _camera.transform.position = transform.position - _camera.transform.forward * 3f + Vector3.up * 2f;
        }
    }
}