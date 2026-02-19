using UnityEngine;
using System.Collections.Generic;

public struct UpgradeOption
{
    public UpgradeType Type;
    public string Name;
    public string Description;
    public float Value;
}

public class UpgradeSystem : MonoBehaviour
{
    public UpgradeOption[] currentOptions;

    private List<UpgradeOption> allUpgrades;

    void Awake()
    {
        allUpgrades = new List<UpgradeOption>
        {
            new UpgradeOption { Type = UpgradeType.Damage, Name = "Dano+", Description = "Aumenta dano em +5", Value = 5f },
            new UpgradeOption { Type = UpgradeType.AttackSpeed, Name = "Vel. Ataque+", Description = "Ataca mais rápido", Value = 0.1f },
            new UpgradeOption { Type = UpgradeType.MoveSpeed, Name = "Vel. Movimento+", Description = "Move mais rápido", Value = 1f },
            new UpgradeOption { Type = UpgradeType.ProjectileSize, Name = "Projétil Maior", Description = "Projéteis maiores", Value = 0.05f },
            new UpgradeOption { Type = UpgradeType.ExtraProjectile, Name = "Projétil Extra", Description = "+1 projétil por tiro", Value = 1f },
        };

        EventManager.OnLevelUp += GenerateOptions;
    }

    void OnDestroy()
    {
        EventManager.OnLevelUp -= GenerateOptions;
    }

    void GenerateOptions(int level)
    {
        List<UpgradeOption> pool = new List<UpgradeOption>(allUpgrades);
        currentOptions = new UpgradeOption[Mathf.Min(3, pool.Count)];

        for (int i = 0; i < currentOptions.Length; i++)
        {
            int idx = Random.Range(0, pool.Count);
            currentOptions[i] = pool[idx];
            pool.RemoveAt(idx);
        }
    }

    public void SelectUpgrade(int index)
    {
        if (currentOptions == null || index < 0 || index >= currentOptions.Length) return;

        UpgradeOption selected = currentOptions[index];
        EventManager.UpgradeSelected(selected.Type, selected.Value);
        currentOptions = null;
    }
}
