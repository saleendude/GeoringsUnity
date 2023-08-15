using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;


public class NetworkPlayerAttack : NetworkBehaviour
{
    PlayerInputSystem playerInput;
    private Animator animator;
    int isAttack1PressedHash;
    int isAttack2PressedHash;

    bool isAttack1Pressed = false;
    bool isAttack2Pressed = false;

    private void Awake()
    {
        playerInput = new PlayerInputSystem();
        animator = GetComponent<Animator>();

    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        isAttack1PressedHash = Animator.StringToHash("Attack1");
        isAttack2PressedHash = Animator.StringToHash("Attack2");

        playerInput.CharacterControls.Attack1.started += OnAttack1;
        playerInput.CharacterControls.Attack1.canceled += OnAttack1;
        playerInput.CharacterControls.Attack2.started += OnAttack2;
        playerInput.CharacterControls.Attack2.canceled += OnAttack2;
    }

    void OnAttack1(InputAction.CallbackContext context)
    {
        isAttack1Pressed = context.ReadValueAsButton();
    }

    void OnAttack2(InputAction.CallbackContext context)
    {
        isAttack2Pressed = context.ReadValueAsButton();
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (isAttack1Pressed && !UIManager.uiTurnedOn)
        {
            CmdAnimateInt(isAttack1PressedHash);
        }

        if (isAttack2Pressed && !UIManager.uiTurnedOn)
        {
            CmdAnimateInt(isAttack2PressedHash);
        }
    }

    public void AttackAnimationStarted()
    {
        if (!isLocalPlayer) return;
        gameObject.GetComponent<NetworkPlayerMovement>().CmdSetAllowMove(false);
    }
    public void AttackAnimationEnded()
    {
        if (!isLocalPlayer) return;
        gameObject.GetComponent<NetworkPlayerMovement>().CmdSetAllowMove(true);
    }

    [Command]
    void CmdSetPlayerAllowMove(bool status)
    {
        RpcSetPlayerAllowMove(status);
    }

    [TargetRpc]
    void RpcSetPlayerAllowMove(bool status)
    {
        NetworkPlayerMovement.Instance.allowMove = status;
    }

    [Command]
    private void CmdAnimateInt(int animationName)
    {
        if (gameObject.GetComponent<NetworkPlayerMovement>().canAttack)
            RpcHandleAnimateInt(animationName);
    }

    [ClientRpc]
    private void RpcHandleAnimateInt(int animationName)
    {
        animator.Play(animationName);
    }


    void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }
    void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }
}
