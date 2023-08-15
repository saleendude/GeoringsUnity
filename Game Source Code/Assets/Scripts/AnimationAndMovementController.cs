using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationAndMovementController : MonoBehaviour
{
    PlayerInputSystem _playerInput;
    CharacterController _characterController;
    Animator _animator;

    // camera variables
    private Camera _cam;

    // variables to store optimized setter/getter parameter IDs
    int _isWalkingHash;
    int _isRunningHash;
    int _isJumpingHash;

    // Variables to store player input values
    Vector2 _currentMovementInput;
    Vector3 _currentMovement;
    Vector3 _currentRunMovement;
    Vector3 _appliedMovement;
    bool _isMovementPressed;
    bool _isRunPressed;

    // movement constants
    //float _rotationFactorPerFrame = 15f;
    public float RunMultiplier = 3.0f;
    public float WalkMultiplier = 2.0f;

    // _gravity variables
    float _groundedGravity = -.05f;
    float _gravity = -9.8f;

    // jumping variables
    bool _isJumpPressed = false;
    bool _isJumping = false;
    bool _isJumpAnimating = false;
    float _initialJumpVelocity;
    float _maxJumpTime = .65f;
    public float MaxJumpHeight = 1f;

    // variables used in camera code
    float _horizontal;
    float _vertical;
    public float _rotationSpeed = 400f;
    Vector3 _moveDirection;
    Vector3 projectedCameraForward;
    Quaternion rotationToCamera;
    Quaternion rotationToMoveDirection;


    private void Awake()
    {
        _playerInput = new PlayerInputSystem();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();

        // set parameter hash reference
        _isWalkingHash = Animator.StringToHash("isWalking");
        _isRunningHash = Animator.StringToHash("isRunning");
        _isJumpingHash = Animator.StringToHash("isJumping");

        _playerInput.CharacterControls.Move.started += OnMovementInput;  // Start listening to context received from _playerInput object when key pressed.
        _playerInput.CharacterControls.Move.canceled += OnMovementInput; // Stop listening to context received from _playerInput object when key released (Keyboard).
        _playerInput.CharacterControls.Move.performed += OnMovementInput; // Stop listening to context received from _playerInput object when key released (Controller).
        _playerInput.CharacterControls.Run.started += OnRun;
        _playerInput.CharacterControls.Run.canceled += OnRun;
        _playerInput.CharacterControls.Jump.started += OnJump;
        _playerInput.CharacterControls.Jump.canceled += OnJump;
        SetupJumpVariables();
    }

    void Start()
    {
        _cam = Camera.main;
    }

    void OnRun(InputAction.CallbackContext context)
    {
        _isRunPressed = context.ReadValueAsButton();
    }

    void SetupJumpVariables()
    {
        float timeToApex = _maxJumpTime / 2;
        _gravity = (-2 * MaxJumpHeight) / Mathf.Pow(timeToApex, 2);
        _initialJumpVelocity = (2 * MaxJumpHeight) / timeToApex;
    }

    void HandleJump()
    {
        if (!_isJumping && _characterController.isGrounded && _isJumpPressed)
        {
            _animator.SetBool(_isJumpingHash, true);
            _isJumpAnimating = true;
            _isJumping = true;
            _currentMovement.y += _initialJumpVelocity;
            _appliedMovement.y += _initialJumpVelocity;
        }
        else if (!_isJumpPressed && _characterController.isGrounded && _isJumping)
        {
            _isJumping = false;
        }
    }

    void OnJump(InputAction.CallbackContext context)
    {
        _isJumpPressed = context.ReadValueAsButton();
    }

    void HandleRotation()
    {
        /*Vector3 positionToLookAt;
        // the change in position of character should point to
        positionToLookAt.x = _currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = _currentMovement.z;

        
        Quaternion currentRotation = transform.rotation; // Current rotation of character

        if (_isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt); // creates new rotation based on where the player is currently pressing
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _rotationFactorPerFrame * Time.deltaTime);
        }*/
        _vertical = _currentMovement.z;
        _horizontal = _currentMovement.x;

        _moveDirection = Vector3.forward * _vertical + Vector3.right * _horizontal;

        //projectedCameraForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
        projectedCameraForward = Vector3.ProjectOnPlane(_cam.transform.forward, Vector3.up);
        rotationToCamera = Quaternion.LookRotation(projectedCameraForward, Vector3.up);


        _moveDirection = rotationToCamera * _moveDirection;

        if (_moveDirection != Vector3.zero)
            rotationToMoveDirection = Quaternion.LookRotation(_moveDirection, Vector3.up);

        if (_isMovementPressed)
            //transform.rotation = Quaternion.RotateTowards(transform.rotation,rotationToCamera,_rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotationToMoveDirection, _rotationSpeed * Time.deltaTime);

    }

    // handler function to set player input values
    void OnMovementInput(InputAction.CallbackContext context)
    {
        _currentMovementInput = context.ReadValue<Vector2>();
        _currentMovement.x = _currentMovementInput.x * WalkMultiplier;
        _currentMovement.z = _currentMovementInput.y * WalkMultiplier;

        _currentRunMovement.x = _currentMovementInput.x * RunMultiplier;
        _currentRunMovement.z = _currentMovementInput.y * RunMultiplier;

        _isMovementPressed = _currentMovementInput.x != 0 || _currentMovementInput.y != 0;
    }

    void HandleAnimation()
    {

        // get parameter values from _animator
        bool isWalking = _animator.GetBool(_isWalkingHash);
        bool isRunning = _animator.GetBool(_isRunningHash);

        if (_isMovementPressed && !isWalking)
        {
            _animator.SetBool("isWalking", true);
        }
        else if (!_isMovementPressed && isWalking)
        {
            _animator.SetBool("isWalking", false);
        }

        if ((_isMovementPressed && _isRunPressed) && !isRunning)
        {
            _animator.SetBool(_isRunningHash, true);
        }
        else if ((!_isMovementPressed || !_isRunPressed) && isRunning)
        {
            _animator.SetBool(_isRunningHash, false);
        }
    }

    void HandleGravity()
    {
        bool isFalling = _currentMovement.y <= 0.0f || !_isJumpPressed;
        float fallMultiplier = 1.5f;

        if (_characterController.isGrounded)
        {
            if (_isJumpAnimating)
            {
                _animator.SetBool(_isJumpingHash, false);
                _isJumpAnimating = false;
            }
            _currentMovement.y = _groundedGravity;
            _appliedMovement.y = _groundedGravity;
        }
        else if (isFalling)
        {
            float previousYVelocity = _currentMovement.y;
            _currentMovement.y = _currentMovement.y + (_gravity * fallMultiplier * Time.deltaTime);
            _appliedMovement.y = Mathf.Max((previousYVelocity + _currentMovement.y) * 0.5f, -20.0f);
        }
        else
        {
            float previousYVelocity = _currentMovement.y;
            _currentMovement.y = _currentMovement.y + (_gravity * Time.deltaTime);
            _appliedMovement.y = (previousYVelocity + _currentMovement.y) * 0.5f;
        }
    }

    void Update()
    {

        HandleRotation();
        HandleAnimation();
        if (_isRunPressed)
        {
            _appliedMovement.x = _moveDirection.x * RunMultiplier;
            _appliedMovement.z = _moveDirection.z * RunMultiplier;
            //_characterController.Move(_currentRunMovement * Time.deltaTime);
        }
        else
        {
            _appliedMovement.x = _moveDirection.x * WalkMultiplier;
            _appliedMovement.z = _moveDirection.z * WalkMultiplier;
            //_characterController.Move(_currentMovement * Time.deltaTime);
        }

        _characterController.Move(_appliedMovement * Time.deltaTime);

        HandleGravity();
        HandleJump();
    }

    void OnEnable()
    {
        _playerInput.CharacterControls.Enable(); // Enable character controls Action Map
    }
    void OnDisable()
    {
        _playerInput.CharacterControls.Disable(); // Disable character controls Action Map
    }
}
