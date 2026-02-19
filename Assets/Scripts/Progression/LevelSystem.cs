using UnityEngine;

public class LevelSystem : MonoBehaviour
{
    public int currentLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 20;

    void Start()
    {
        EventManager.OnXPCollected += AddXP;
    }

    void OnDestroy()
    {
        EventManager.OnXPCollected -= AddXP;
    }

    void AddXP(int amount)
    {
        currentXP += amount;

        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            currentLevel++;
            xpToNextLevel = CalculateXPForLevel(currentLevel);
            EventManager.LevelUp(currentLevel);
        }
    }

    int CalculateXPForLevel(int level)
    {
        return 20 + (level - 1) * 15;
    }

    public float GetXPProgress()
    {
        return xpToNextLevel > 0 ? (float)currentXP / xpToNextLevel : 0f;
    }
}
