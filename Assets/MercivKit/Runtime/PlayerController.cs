using UnityEngine;
using UnityEngine.InputSystem;
using Fusion;

namespace MercivKit
{
    public class PlayerController : NetworkBehaviour, MercivInputActions.IPlayerActions
    {
        [SerializeField]
        private float _moveSpeed = 5f;
        public float MoveSpeed
        {
            get => _moveSpeed;
            set => _moveSpeed = value;
        }

        [SerializeField]
        private float _rotateSpeed = 1f;
        public float RotateSpeed
        {
            get => _rotateSpeed;
            set => _rotateSpeed = value;
        }

        [SerializeField]
        private float _gravity = 9.8f;
        public float Gravity
        {
            get => _gravity;
            set => _gravity = value;
        }

        private Vector2 _moveInput;
        private MercivInputActions _inputActions;
        private CharacterController _characterController;
        private float _fallSpeed;

        private void Awake()
        {
            _inputActions = new MercivInputActions();
            _inputActions.Player.SetCallbacks(this);
            _characterController = GetComponent<CharacterController>();
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

        public void OnLook(InputAction.CallbackContext context) {}
        public void OnFire(InputAction.CallbackContext context) {}

        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority)
            {
                return;
            }

            if (!_characterController.isGrounded)
            {
                _fallSpeed += _gravity * Runner.DeltaTime;
                _characterController.Move(Vector3.down * _fallSpeed * Runner.DeltaTime);
            }
            else
            {
                _fallSpeed = 0;
            }

            _characterController.Move(transform.forward * _moveInput.y * _moveSpeed * Runner.DeltaTime);
            transform.Rotate(Vector3.up, _moveInput.x * _rotateSpeed * Runner.DeltaTime * 100);
        }
    }
}