using System.Collections;
using UnityEngine;

public class fallingObjectScript : MonoBehaviour
{
    public int scoreValue = 1; // points given when collected by player
    public string groundTag = "Ground";

    Rigidbody2D rb;
    bool hasLanded = false;
    Coroutine destroyCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // no unconditional Destroy here; lifetime will be handled on landing
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // award score (safe-call in case ScoreManager isn't present)
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.AddScore(scoreValue);

            // if we had a pending coroutine to reduce a life, cancel it (player collected)
            if (destroyCoroutine != null)
            {
                StopCoroutine(destroyCoroutine);
                destroyCoroutine = null;
            }

            Destroy(gameObject);
            return;
        }

        // handle landing on ground
        if (!hasLanded && collision.gameObject.CompareTag(groundTag))
        {
            hasLanded = true;

            // stop movement and freeze on ground
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.bodyType = RigidbodyType2D.Static;
            }

            float lifetime = 5f;
            if (ScoreManager.Instance != null)
                lifetime = ScoreManager.Instance.GetFallLifetime();

            destroyCoroutine = StartCoroutine(DestroyAfterDelay(lifetime));
        }
    }

    IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // when object self-destructs after landing, reduce player's life
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.ReduceLife(1);

        Destroy(gameObject);
    }
}
