using UnityEngine;

public class Player2 : MonoBehaviour
{
    public float moveAmount = 1f;

    public void Move(Vector3 direction)
    {
        transform.position += direction * moveAmount;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }
}
