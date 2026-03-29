using UnityEngine;

public class CommandRight : MonoBehaviour
{
    private Player2 player;
    private Vector3 previousPosition;

    public CommandRight(Player2 player)
    {
        this.player = player;
    }

    public void Execute()
    {
        previousPosition = player.transform.position;
        player.Move(Vector3.right);
    }

    public void Undo()
    {
        player.SetPosition(previousPosition);
    }
}
