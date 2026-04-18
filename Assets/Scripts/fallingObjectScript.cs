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
        // lifetime handled on landing now
    }

    // Robust helper that detects whether a Collider2D belongs to the player object
    bool IsPlayerCollider(Collider2D col)
    {
        if (col == null) return false;

        if (col.CompareTag("Player"))
            return true;

        // if the collider is on a child object, check the root GameObject
        if (col.transform.root != null && col.transform.root.CompareTag("Player"))
            return true;

        // if collider has an attached Rigidbody2D, check its GameObject
        if (col.attachedRigidbody != null && col.attachedRigidbody.gameObject.CompareTag("Player"))
            return true;

        return false;
    }

    // Handle collision (non-trigger)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // player collection
        if (IsPlayerCollider(collision.collider))
        {
            HandlePlayerCollection(collision.collider.gameObject);
            return;
        }

        // landing on ground
        if (!hasLanded && collision.collider != null && collision.collider.CompareTag(groundTag))
        {
            HandleLanding();
        }
    }

    // Handle trigger interactions (in case colliders are triggers)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsPlayerCollider(other))
        {
            HandlePlayerCollection(other.gameObject);
            return;
        }

        if (!hasLanded && other != null && other.CompareTag(groundTag))
        {
            HandleLanding();
        }
    }

    void HandlePlayerCollection(GameObject playerObject)
    {
        // award score (safe-call in case ScoreManager isn't present)
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore(scoreValue);

        // Cancel pending life-reduction/destroy coroutine if any
        if (destroyCoroutine != null)
        {
            StopCoroutine(destroyCoroutine);
            destroyCoroutine = null;
        }

        Destroy(gameObject);
    }

    void HandleLanding()
    {
        hasLanded = true;

        // stop movement and keep collisions active by switching to Kinematic
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic; // changed from Static to Kinematic
        }

        float lifetime = 5f;
        if (ScoreManager.Instance != null)
            lifetime = ScoreManager.Instance.GetFallLifetime();

        destroyCoroutine = StartCoroutine(DestroyAfterDelay(lifetime));
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
