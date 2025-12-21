using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;

    [Header("Score")]
    private int currentScore = 0;
    public int activeTargets = 0;

    [Header("Targets")]
    public int maxTargets = 5;
    public int currentTargets = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Ajout de score
    public void AddScore(int amount)
    {
        currentScore += amount;
    }

    // Accès au score
    public int GetScore()
    {
        return currentScore;
    }

    public void RegisterTargetSpawn()
    {
        currentTargets++;
    }

    public void RegisterTargetDespawn()
    {
        currentTargets = Mathf.Max(0, currentTargets - 1);
    }
}
