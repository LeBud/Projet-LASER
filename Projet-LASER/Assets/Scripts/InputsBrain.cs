using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputsBrain : MonoBehaviour
{
    public static InputsBrain Instance { get; private set; }

    PlayerControllerInputs inputs;
    [HideInInspector]
    public InputAction move, mouse, jump, teleport, laser, pause;

    private void Awake()
    {
        inputs = new PlayerControllerInputs();

        if (Instance == null)
            Instance = this;
    }

    private void OnEnable()
    {
        move = inputs.Player.Movement;
        jump = inputs.Player.Jump;
        mouse = inputs.Player.Mouse;
        teleport = inputs.Player.Teleport; 
        laser = inputs.Player.Laser;
        pause = inputs.Player.Pause;

        inputs.Enable();
    }

    private void OnDisable()
    {
        inputs.Disable();
    }
}
