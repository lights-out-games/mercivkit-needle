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

        [SerializeField]
        private Animator _animator;
        public Animator Animator
        {
            get => _animator;
            set => _animator = value;
        }

        private Vector2 _moveInput;
        private MercivInputActions _inputActions;
        private CharacterController _characterController;
        private float _fallSpeed;

        [Networked]
        public Vector3 Velocity { get; set; }

        private void Awake()
        {
            _inputActions = new MercivInputActions();
            _inputActions.Player.SetCallbacks(this);
            _characterController = GetComponent<CharacterController>();
        }

        public override void Spawned()
        {
            if (HasStateAuthority)
            {
                Camera.main.GetComponent<CameraController>().target = GetComponent<NetworkTransform>().InterpolationTarget;
                GetComponent<NetworkTransform>().InterpolationDataSource = InterpolationDataSources.NoInterpolation;
            }
            else
            {
                GetComponent<NetworkTransform>().InterpolationDataSource = InterpolationDataSources.Auto;
            }
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

            var previousPos = transform.position;
            _characterController.Move(transform.forward * _moveInput.y * _moveSpeed * Runner.DeltaTime);
            transform.Rotate(Vector3.up, _moveInput.x * _rotateSpeed * Runner.DeltaTime * 100);
            Velocity = (transform.position - previousPos) * Runner.Simulation.Config.TickRate;
        }

        public override void Render()
        {
            var xzSpeed = new Vector2(Velocity.x, Velocity.z).magnitude;
            _animator.SetFloat("MoveSpeed", xzSpeed);
        }
    }
}