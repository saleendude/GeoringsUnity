using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }

    public override void EnterState() {
        HandleJump();
    }

    public override void UpdateState() {
        CheckSwitchStates();
        HandleGravity();
    }

    public override void ExitState() {
        _ctx.Animator.SetBool(_ctx.IsJumpingHash, false);
        _ctx.IsJumpAnimating = false;
    }

    public override void CheckSwitchStates() 
    {
        if (_ctx.CharacterController.isGrounded)
        {
            SwitchState(_factory.Grounded());
        }
    }

    public override void InitializeSubState() { }

    void HandleJump()
    {
        _ctx.Animator.SetBool(_ctx.IsJumpingHash, true);
        _ctx.IsJumpAnimating = true;
        _ctx.IsJumping = true;
        _ctx.CurrentMovementY += _ctx.InitialJumpVelocity;
        _ctx.AppliedMovementY += _ctx.InitialJumpVelocity;
    }

    void HandleGravity()
    {
        bool isFalling = _ctx.CurrentMovementY <= 0.0f || !_ctx.IsJumpPressed;
        float fallMultiplier = 2.0f;

        if (isFalling)
        {
            float previousYVelocity = _ctx.CurrentMovementY;
            _ctx.CurrentMovementY = _ctx.CurrentMovementY + (_ctx.Gravity * fallMultiplier * Time.deltaTime);
            _ctx.AppliedMovementY = Mathf.Max((previousYVelocity + _ctx.CurrentMovementY) * 0.5f, -20.0f);
        }
        else
        {
            float previousYVelocity = _ctx.CurrentMovementY;
            _ctx.CurrentMovementY = _ctx.CurrentMovementY + (_ctx.Gravity * Time.deltaTime);
            _ctx.AppliedMovementY = (previousYVelocity + _ctx.CurrentMovementY) * 0.5f;
        }
    }
}
