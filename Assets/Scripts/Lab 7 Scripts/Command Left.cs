using UnityEngine;

public class CommandLeft : ICommand
{
    private Player2 player;
    private Vector3 previousPosition;

    public CommandLeft(Player2 player)
    {
        this.player = player;
    }

    public void Execute()
    {
        previousPosition = player.transform.position;
        player.Move(Vector3.left);
    }

    public void Undo()
    {
        player.SetPosition(previousPosition);
    }
}
