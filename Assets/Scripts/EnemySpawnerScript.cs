using UnityEngine;

public class EnemySpawnerScript : MonoBehaviour
{
    public GameObject fallingObjectPrefab;
    public float spawnrate = 5f;
    float activeSpawnRate;
  
    void Start()
    {
        // prefer ScoreManager-provided rate if available
        if (ScoreManager.Instance != null)
            activeSpawnRate = ScoreManager.Instance.GetSpawnRate();
        else
            activeSpawnRate = spawnrate;

        InvokeRepeating(nameof(SpawnFallingObject), 1f, activeSpawnRate);
    }

    public void SetSpawnRate(float rate)
    {
        // restart invoke with new rate
        CancelInvoke(nameof(SpawnFallingObject));
        activeSpawnRate = Mathf.Max(0.1f, rate);
        InvokeRepeating(nameof(SpawnFallingObject), 0.1f, activeSpawnRate);
    }

    void SpawnFallingObject()
    {
        float randomX = Random.Range(-8f, 8f);
        Vector3 spawnPos = new Vector3(randomX, 6f, 0f);
        Instantiate(fallingObjectPrefab, spawnPos, Quaternion.identity);
    }
}
