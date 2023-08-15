using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    PlayerInputSystem _playerInput;
    CharacterController _characterController;
    Animator _animator;

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
    float _rotationFactorPerFrame = 15f;
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

    // state variables
    PlayerBaseState _currentState;
    PlayerStateFactory _states;

    // getters and setters
    public PlayerBaseState CurrentState { get { return _currentState; }  set { _currentState = value; } }
    public Animator Animator { get { return _animator; } }
    public CharacterController CharacterController { get { return _characterController; } }
    public int IsJumpingHash { get { return _isJumpingHash; } }
    public float GroundedGravity { get{ return _groundedGravity; } }
    public float Gravity { get{ return _gravity; } }
    public bool IsJumpAnimating { set { _isJumpAnimating = value; } }
    public bool IsJumping { set { _isJumping = value; } }
    public bool IsJumpPressed { get { return _isJumpPressed; } }
    public float InitialJumpVelocity { get { return _initialJumpVelocity; } set { _initialJumpVelocity = value; } }
    public float CurrentMovementY { get{ return _currentMovement.y; } set { _currentMovement.y = value; } }
    public float AppliedMovementY { get { return _appliedMovement.y; } set { _appliedMovement.y = value; } }

    private void Awake()
    {
        // initially set reference variables
        _playerInput = new PlayerInputSystem();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();

        // setup states
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();

        // set parameter hash reference
        _isWalkingHash = Animator.StringToHash("isWalking");
        _isRunningHash = Animator.StringToHash("isRunning");
        _isJumpingHash = Animator.StringToHash("isJumping");

        // set player input callbacks
        _playerInput.CharacterControls.Move.started += OnMovementInput;  // Start listening to context received from _playerInput object when key pressed.
        _playerInput.CharacterControls.Move.canceled += OnMovementInput; // Stop listening to context received from _playerInput object when key released (Keyboard).
        _playerInput.CharacterControls.Move.performed += OnMovementInput; // Stop listening to context received from _playerInput object when key released (Controller).
        _playerInput.CharacterControls.Run.started += OnRun;
        _playerInput.CharacterControls.Run.canceled += OnRun;
        _playerInput.CharacterControls.Jump.started += OnJump;
        _playerInput.CharacterControls.Jump.canceled += OnJump;
        SetupJumpVariables();
    }

    void OnMovementInput(InputAction.CallbackContext context)
    {
        _currentMovementInput = context.ReadValue<Vector2>();
        _isMovementPressed = _currentMovementInput.x != 0 || _currentMovementInput.y != 0;
    }

    void OnJump(InputAction.CallbackContext context)
    {
        _isJumpPressed = context.ReadValueAsButton();
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

    void Update()
    {
        HandleRotation();
        _currentState.UpdateState();
        _characterController.Move(_appliedMovement * Time.deltaTime);
    }

    void HandleRotation()
    {
        Vector3 positionToLookAt;
        // the change in position of character should point to
        positionToLookAt.x = _currentMovementInput.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = _currentMovementInput.y;


        Quaternion currentRotation = transform.rotation; // Current rotation of character

        if (_isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt); // creates new rotation based on where the player is currently pressing
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _rotationFactorPerFrame * Time.deltaTime);
        }
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
