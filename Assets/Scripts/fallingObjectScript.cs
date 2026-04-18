using UnityEngine;

public class fallingObjectScript : MonoBehaviour
{
    public int scoreValue = 1; // points given when collected by player

    void Start()
    {
        Destroy(gameObject, 5f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // award score (safe-call in case ScoreManager isn't present)
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.AddScore(scoreValue);

            Destroy(gameObject);
            return;
        }
    }
}
