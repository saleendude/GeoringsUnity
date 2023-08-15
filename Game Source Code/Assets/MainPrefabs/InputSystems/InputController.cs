using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[Serializable] public class MoveInputEvent: UnityEvent<float, float> { }

public class InputController : MonoBehaviour
{
    Controller controls;
    public MoveInputEvent moveInputEvent;

    private void Awake()
    {
        controls = new Controller();
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
        controls.Gameplay.Movement.performed += OnMovePerfom;
        controls.Gameplay.Movement.canceled += OnMovePerfom;
    }

    private void OnMovePerfom(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
        moveInputEvent.Invoke(moveInput.x, moveInput.y);
       // Debug.Log("Move Input: "+ moveInput);
    }
}
