using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public Player2 player;

    private PlayerControls controls;
    private Stack<ICommand> commandHistory = new Stack<ICommand>();

    private float moveThreshold = 0.5f;

    // Remote phone way to handle input, works for both keyboard and touch (mobile)
    void Awake()
    {
        controls = new PlayerControls();

        // Movement input (keyboard + touch)
        controls.Player.Move.performed += ctx =>
        {
            float input = ctx.ReadValue<Vector2>().x;

            if (input < -moveThreshold)
            {
                ICommand command = new CommandLeft(player);
                command.Execute();
                commandHistory.Push(command);
            }
            else if (input > moveThreshold)
            {
                ICommand command = new CommandRight(player);
                command.Execute();
                commandHistory.Push(command);
            }
        };
    }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void Update()
    {
        // Undo (keyboard)
        if (Keyboard.current.zKey.wasPressedThisFrame)
        {
            if (commandHistory.Count > 0)
            {
                ICommand command = commandHistory.Pop();
                command.Undo();
            }
        }

        // Non-undo command
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            ColorChangerCommand cmd =
                new ColorChangerCommand(player.GetComponent<SpriteRenderer>());

            cmd.Execute();
        }
    }

    // UI Button methods
    public void MoveLeftButton()
    {
        ICommand command = new CommandLeft(player);
        command.Execute();
        commandHistory.Push(command);
    }

    public void MoveRightButton()
    {
        ICommand command = new CommandRight(player);
        command.Execute();
        commandHistory.Push(command);
    }

}
