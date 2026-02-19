---
name: unity-game-systems
description: Sistemas de jogo reutilizáveis para o projeto SHOOT. Use quando precisar criar spawners, sistemas de armas, progressão, ou novos sistemas de gameplay.
---

# Unity Game Systems

## Comunicação via Eventos

Todos os sistemas se comunicam via `EventManager`:

```csharp
// Disparar evento
EventManager.EnemyDied(position, xpValue);

// Ouvir evento
void Start() { EventManager.OnEnemyDied += HandleEnemyDied; }
void OnDestroy() { EventManager.OnEnemyDied -= HandleEnemyDied; }
```

Eventos disponíveis:
- `OnEnemyDied(Vector2, int)` — posição e valor de XP
- `OnXPCollected(int)` — quantidade de XP
- `OnLevelUp(int)` — novo nível
- `OnPlayerDied()` — game over
- `OnUpgradeSelected(UpgradeType, float)` — upgrade aplicado

## Padrão: Spawner

```csharp
public class MeuSpawner : MonoBehaviour
{
    float timer;
    float interval = 1.5f;
    
    void Update()
    {
        if (GameManager.Instance.State != GameState.Playing) return;
        timer += Time.deltaTime;
        if (timer >= interval) { timer = 0; Spawn(); }
    }
    
    void Spawn()
    {
        Vector2 pos = CalcularPosicao();
        GameObject go = CircleFactory.CreateCircleWithPhysics(...);
        go.AddComponent<ComponenteDeComportamento>();
    }
}
```

## Padrão: Nova Arma

```csharp
public class MinhaArma : MonoBehaviour
{
    float timer, interval = 1f;
    float damage = 10f;
    
    void Update()
    {
        if (GameManager.Instance.State != GameState.Playing) return;
        timer += Time.deltaTime;
        if (timer >= interval) { timer = 0; Atacar(); }
    }
    
    void Atacar()
    {
        // Lógica de ataque usando CircleFactory para projéteis
    }
    
    void Start() { EventManager.OnUpgradeSelected += HandleUpgrade; }
    void OnDestroy() { EventManager.OnUpgradeSelected -= HandleUpgrade; }
}
```

## Padrão: Upgrade

Tipos disponíveis em `UpgradeType`:
- `Damage`, `AttackSpeed`, `MoveSpeed`, `ProjectileSize`, `ExtraProjectile`

Para adicionar novo tipo:
1. Adicionar ao enum `UpgradeType` em `GameManager.cs`
2. Adicionar `UpgradeOption` na lista em `UpgradeSystem.Awake()`
3. Tratar no `HandleUpgrade` do sistema alvo
