using UnityEngine;

namespace MercivKit
{
    public class PlayerAnimator : MonoBehaviour
    {
        private CharacterController _characterController;

        [SerializeField]
        private Animator _animator;
        public Animator Animator
        {
            get => _animator;
            set => _animator = value;
        }

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        void Update()
        {
            _animator.SetFloat("MoveSpeed", Mathf.Abs(_characterController.velocity.magnitude));
        }
    }
}