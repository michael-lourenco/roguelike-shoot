using System;
using UnityEngine;

public static class EventManager
{
    public static event Action<Vector2, int> OnEnemyDied;
    public static event Action<int> OnXPCollected;
    public static event Action<int> OnLevelUp;
    public static event Action OnPlayerDied;
    public static event Action<UpgradeType, float> OnUpgradeSelected;

    public static void EnemyDied(Vector2 position, int xpValue) => OnEnemyDied?.Invoke(position, xpValue);
    public static void XPCollected(int amount) => OnXPCollected?.Invoke(amount);
    public static void LevelUp(int newLevel) => OnLevelUp?.Invoke(newLevel);
    public static void PlayerDied() => OnPlayerDied?.Invoke();
    public static void UpgradeSelected(UpgradeType type, float value) => OnUpgradeSelected?.Invoke(type, value);

    public static void ClearAll()
    {
        OnEnemyDied = null;
        OnXPCollected = null;
        OnLevelUp = null;
        OnPlayerDied = null;
        OnUpgradeSelected = null;
    }
}
