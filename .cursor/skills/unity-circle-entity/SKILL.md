---
name: unity-circle-entity
description: Cria entidades visuais baseadas em círculos para o jogo SHOOT. Use quando precisar criar GameObjects visuais, entidades de jogo, ou qualquer elemento visual no projeto.
---

# Unity Circle Entity

## CircleFactory API

Toda entidade visual deve ser criada usando `CircleFactory`:

### Criar círculo simples (sem física)

```csharp
GameObject go = CircleFactory.CreateCircle("Nome", Color.blue, 0.4f, Vector2.zero);
```

Parâmetros: nome, cor, raio, posição

### Criar círculo com física

```csharp
GameObject go = CircleFactory.CreateCircleWithPhysics(
    "Nome", Color.red, 0.35f, posição,
    isTrigger: false,
    isKinematic: false
);
```

Parâmetros adicionais:
- `isTrigger`: true para XP orbs e projéteis
- `isKinematic`: true para objetos que não recebem forças físicas

### Entidades padrão

| Entidade   | Cor           | Raio  | Trigger | Kinematic |
|------------|---------------|-------|---------|-----------|
| Player     | Color.blue    | 0.4f  | false   | false     |
| Enemy      | Color.red     | 0.35f | false   | false     |
| Projectile | Color.yellow  | 0.15f | true    | true      |
| XP Orb     | Color.green   | 0.15f | true    | true      |

## Composição de Componentes

Após criar a entidade, adicionar comportamentos:

```csharp
// Inimigo
GameObject enemy = CircleFactory.CreateCircleWithPhysics("Enemy", Color.red, 0.35f, pos);
EnemyAI ai = enemy.AddComponent<EnemyAI>();
ai.moveSpeed = 2f;
ai.maxHealth = 30f;

// Projétil
GameObject proj = CircleFactory.CreateCircleWithPhysics("Projectile", Color.yellow, 0.15f, pos, true, true);
Projectile p = proj.AddComponent<Projectile>();
p.damage = 10f;
p.speed = 10f;
p.direction = dir;
```

## Detecção de Colisão

- Usar componentes (`GetComponent<T>()`) para identificar entidades, não tags
- Projéteis e XP usam `OnTriggerEnter2D`
- Player vs Enemy usa `OnCollisionStay2D`
