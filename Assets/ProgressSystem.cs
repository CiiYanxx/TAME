using UnityEngine;

public class ProgressSystem : MonoBehaviour
{
    public static ProgressSystem Instance { get; private set; }

    [Header("Progress & Currency")]
    public int rescueProgressPoints = 0;
    public int goldCoins = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: keep across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddProgress(int points)
    {
        if (points > 0)
        {
            rescueProgressPoints += points;
            Debug.Log($"Gained {points} Progress Points. Total: {rescueProgressPoints}");
        }
    }
    
    public void DeductProgress(int points)
    {
        if (points > 0)
        {
            rescueProgressPoints -= points;
            if (rescueProgressPoints < 0) rescueProgressPoints = 0; 
            Debug.LogWarning($"Deducted {points} Progress Points. Total: {rescueProgressPoints}");
        }
    }

    public void AddCoins(int amount)
    {
        if (amount > 0)
        {
            goldCoins += amount;
            Debug.Log($"Gained {amount} Gold Coins. Total: {goldCoins}");
        }
    }
}