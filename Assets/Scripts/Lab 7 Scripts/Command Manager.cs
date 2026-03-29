using System.Collections.Generic;
using UnityEngine;

public class CommandManager : MonoBehaviour
{
    public Player2 player;

    private Stack<ICommand> commandHistory = new Stack<ICommand>();

    void Update()
    {
        // Move Left
        if (Input.GetKeyDown(KeyCode.A))
        {
            ICommand command = new CommandLeft(player);
            command.Execute();
            commandHistory.Push(command);
        }

        // Move Right
        if (Input.GetKeyDown(KeyCode.D))
        {
            ICommand command = new CommandRight(player);
            command.Execute();
            commandHistory.Push(command);
        }

        // Undo
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (commandHistory.Count > 0)
            {
                ICommand command = commandHistory.Pop();
                command.Undo();
            }
        }

        // Non-Undoable Action
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ColorChangerCommand colorCommand =
                new ColorChangerCommand(player.GetComponent<SpriteRenderer>());

            colorCommand.Execute();
        }
    }
}
