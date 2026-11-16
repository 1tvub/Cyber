using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovemenController : MonoBehaviour
{
    [SerializeField] private CharacterController _controller;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _walkSpeed = 4f;
    [SerializeField] private float _sprintSpeed = 7f;
    [SerializeField] private float _turnSpeed = 120f;
    [SerializeField] private float _jumpHeight = 1.5f;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _groundDistance = 0.25f;
    [SerializeField] private LayerMask _groundMask;

    private Vector2 _moveInput;
    private Vector3 _verticalVelocity;
    private bool _isGrounded;
    private bool _isSprinting;

    private void Reset()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Awake()
    {
        if (_controller == null)
        {
            _controller = GetComponent<CharacterController>();
        }

        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
        }
    }

    private void Update()
    {
        HandleGroundCheck();

        ReadKeyboardInput();

        HandleRotation();

        HandleHorizontalMovement();

        HandleGravity();

        HandleAnimations();
    }

    private void ReadKeyboardInput()
    {
        if (Keyboard.current == null) return;

        _moveInput = Vector2.zero;

        if (Keyboard.current.wKey.isPressed) _moveInput.y = 1f;
        if (Keyboard.current.sKey.isPressed) _moveInput.y = -1f;
        if (Keyboard.current.dKey.isPressed) _moveInput.x = 1f;
        if (Keyboard.current.aKey.isPressed) _moveInput.x = -1f;

        _isSprinting = Keyboard.current.leftShiftKey.isPressed;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Jump();
        }
    }

    private void HandleGroundCheck()
    {
        Vector3 checkPosition = _groundCheck != null ? _groundCheck.position : transform.position;

        _isGrounded = Physics.CheckSphere(checkPosition, _groundDistance, _groundMask);
        if (_isGrounded && _verticalVelocity.y < 0)
        {
            _verticalVelocity.y = -2f;
        }
    }

    private void HandleRotation()
    {
        transform.Rotate(Vector3.up * (_moveInput.x * _turnSpeed * Time.deltaTime));
    }

    private void HandleHorizontalMovement()
    {
        Vector3 moveDirection = transform.forward * _moveInput.y;

        float currentSpeed = _isSprinting ? _sprintSpeed : _walkSpeed;

        _controller.Move(moveDirection * (currentSpeed * Time.deltaTime));
    }

    private void HandleGravity()
    {
        _verticalVelocity.y += _gravity * Time.deltaTime;
        _controller.Move(_verticalVelocity * Time.deltaTime);
    }

    private void HandleAnimations()
    {
        if (_animator == null) return;

        bool isMoving = _moveInput.y != 0f;

        _animator.SetBool("isWalking", isMoving);
        _animator.SetBool("isRunning", isMoving && _isSprinting);
        _animator.SetBool("isJumping", !_isGrounded);
    }

    public void Jump()
    {
        if (_isGrounded)
        {
            _verticalVelocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
        }
    }
}
