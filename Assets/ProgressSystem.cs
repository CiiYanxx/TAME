using UnityEngine;

public class ProgressSystem : MonoBehaviour
{
    public static ProgressSystem Instance { get; private set; }

    [Header("UI Reference")]
    // NEW: Reference to the script that manages the visual bar
    public RescuePointsBar rescuePointsBar; 

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

    public void Start()
    {
        // Optional: Initialize the bar display with the current points on startup.
        if (rescuePointsBar != null)
        {
            rescuePointsBar.currentPoints = rescueProgressPoints;
            // The bar's Start method will handle the initial display update.
        }
    }

    public void AddProgress(int points)
    {
        if (points > 0)
        {
            // 1. Update the internal state
            rescueProgressPoints += points;
            Debug.Log($"Gained {points} Progress Points. Total: {rescueProgressPoints}");

            // 2. Update the UI bar
            if (rescuePointsBar != null)
            {
                // The bar handles clamping/level-up visuals internally
                rescuePointsBar.AddRescuePoints(points); 
            }
        }
    }
    
    public void DeductProgress(int points)
    {
        if (points > 0)
        {
            // 1. Update the internal state
            rescueProgressPoints -= points;
            if (rescueProgressPoints < 0) rescueProgressPoints = 0; 
            Debug.LogWarning($"Deducted {points} Progress Points. Total: {rescueProgressPoints}");

            // 2. Update the UI bar
            if (rescuePointsBar != null)
            {
                rescuePointsBar.DeductRescuePoints(points);
            }
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