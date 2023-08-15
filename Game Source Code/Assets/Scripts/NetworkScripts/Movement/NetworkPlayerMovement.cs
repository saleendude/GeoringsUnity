using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using Cinemachine;

public class NetworkPlayerMovement : NetworkBehaviour
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
    public float RunMultiplier = 3.0f;
    public float WalkMultiplier = 2.0f;

    [SyncVar]
    public bool allowMove = true;
    [SyncVar]
    public bool isMoving = false;
    [SyncVar]
    public bool canAttack = true;

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
    [SerializeField] private CinemachineFreeLook thirdPersonCam;

    public static NetworkPlayerMovement Instance;

    private void Awake()
    {
        Instance = this;

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

    public override void OnStartLocalPlayer()
    {
        _cam = Camera.main;
        if (isLocalPlayer)
        {
            thirdPersonCam = CinemachineFreeLook.FindObjectOfType<CinemachineFreeLook>();
            thirdPersonCam.LookAt = this.transform;
            thirdPersonCam.Follow = this.transform;
        }
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
            CmdJump(_initialJumpVelocity);
            //canAttack = false;
            CmdSetCanAttack(false);
        }
        else if (!_isJumpPressed && _characterController.isGrounded && _isJumping)
        {
            _isJumping = false;
            //canAttack = true;
            CmdSetCanAttack(true);
        }
    }

    void OnJump(InputAction.CallbackContext context)
    {
        _isJumpPressed = context.ReadValueAsButton();
    }

    void HandleRotation()
    {
        _vertical = _currentMovement.z;
        _horizontal = _currentMovement.x;

        _moveDirection = Vector3.forward * _vertical + Vector3.right * _horizontal;

        projectedCameraForward = Vector3.ProjectOnPlane(_cam.transform.forward, Vector3.up);
        if (projectedCameraForward != Vector3.zero)
            rotationToCamera = Quaternion.LookRotation(projectedCameraForward, Vector3.up);


        _moveDirection = rotationToCamera * _moveDirection;

        if (_moveDirection != Vector3.zero)
            rotationToMoveDirection = Quaternion.LookRotation(_moveDirection, Vector3.up);

        if (_isMovementPressed)
            CmdRotate(rotationToMoveDirection);

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


        if (_isMovementPressed && !isWalking && !UIManager.uiTurnedOn)
        {
            CmdAnimateInt(_isWalkingHash, true);
            //isMoving = true;
            CmdSetIsMoving(true);
        }
        else if (!_isMovementPressed && isWalking && !UIManager.uiTurnedOn)
        {
            CmdAnimateInt(_isWalkingHash, false);
            //isMoving = false;
            CmdSetIsMoving(false);
        }

        if ((_isMovementPressed && _isRunPressed) && !isRunning && !UIManager.uiTurnedOn)
        {
            CmdAnimateInt(_isRunningHash, true);
            //isMoving = true;
            CmdSetIsMoving(true);
        }
        else if ((!_isMovementPressed || !_isRunPressed) && isRunning && !UIManager.uiTurnedOn)
        {
            CmdAnimateInt(_isRunningHash, false);
            //isMoving = false;
            CmdSetIsMoving(false);
        }

    }

    void HandleGravity()
    {
        bool isFalling = _currentMovement.y <= 0.0f || !_isJumpPressed;
        float fallMultiplier = 1.5f;
        CmdGravity(isFalling, fallMultiplier, _isJumpAnimating);
    }

    [ClientCallback]
    void Update()
    {
        if (!UIManager.uiTurnedOn)
        {
            if (!isLocalPlayer) { return; }

            HandleRotation();
            HandleAnimation();
            if (_isRunPressed)
            {
                _appliedMovement.x = _moveDirection.x * RunMultiplier;
                _appliedMovement.z = _moveDirection.z * RunMultiplier;
            }
            else
            {
                _appliedMovement.x = _moveDirection.x * WalkMultiplier;
                _appliedMovement.z = _moveDirection.z * WalkMultiplier;
            }

            if (!UIManager.uiTurnedOn)
                CmdMove(_appliedMovement);

            HandleGravity();
            HandleJump();

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

    [Command]
    public void CmdSetCanAttack(bool status)
    {
        canAttack = status;
    }

    [Command]
    public void CmdSetIsMoving(bool status)
    {
        isMoving = status;
    }

    [Command]
    public void CmdSetAllowMove(bool status)
    {
        allowMove = status;
    }

    [Command]
    private void CmdMove(Vector3 position)
    {
        if (allowMove)
            RpcMove(position);
    }

    [Command]
    private void CmdJump(float initialJumpVelocity)
    {
        RpcJump(initialJumpVelocity);
        canAttack = false;
    }

    [Command]
    private void CmdRotate(Quaternion rotationToMoveDirection)
    {
        RpcRotate(rotationToMoveDirection);
    }

    [Command]
    private void CmdGravity(bool isFalling, float fallMultiplier, bool _isJumpAnimating)
    {
        RpcHandleGravity(isFalling, fallMultiplier, _isJumpAnimating);
    }

    [Command]
    private void CmdAnimateBool(string animationName, bool animationBoolean)
    {
        RpcHandleAnimateBool(animationName, animationBoolean);
    }

    [ClientRpc]
    private void RpcHandleAnimateBool(string animationName, bool animationBoolean)
    {
        _animator.SetBool(animationName, animationBoolean);
    }
    [Command(requiresAuthority = false)]
    private void CmdAnimateInt(int animationName, bool animationBoolean)
    {
        RpcHandleAnimateInt(animationName, animationBoolean);
    }

    [ClientRpc]
    private void RpcHandleAnimateInt(int animationName, bool animationBoolean)
    {
        _animator.SetBool(animationName, animationBoolean);
    }

    [ClientRpc]
    private void RpcRotate(Quaternion rotationToMoveDirection)
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotationToMoveDirection, _rotationSpeed * Time.deltaTime);
    }

    [ClientRpc]
    private void RpcMove(Vector3 position)
    {
        _characterController.Move(position * Time.deltaTime);
    }

    [ClientRpc]
    private void RpcHandleGravity(bool isFalling, float fallMultiplier, bool _isJumpAnimating)
    {
        if (_characterController.isGrounded)
        {
            if (_isJumpAnimating)
            {
                CmdAnimateInt(_isJumpingHash, false);
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

    [ClientRpc]
    private void RpcJump(float initialJumpVelocity)
    {
        CmdAnimateInt(_isJumpingHash, true);
        _isJumpAnimating = true;
        _isJumping = true;
        _currentMovement.y += initialJumpVelocity;
        _appliedMovement.y += initialJumpVelocity;
    }
}
