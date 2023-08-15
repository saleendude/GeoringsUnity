using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackTest : MonoBehaviour
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
        animator.SetTrigger(isAttack1PressedHash);
    }

    void OnAttack2(InputAction.CallbackContext context)
    {
        isAttack2Pressed = context.ReadValueAsButton();
        animator.SetTrigger(isAttack2PressedHash);
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
