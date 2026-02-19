using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Playing, Upgrading, GameOver }
public enum UpgradeType { Damage, AttackSpeed, MoveSpeed, ProjectileSize, ExtraProjectile }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState State { get; private set; } = GameState.Playing;
    public PlayerController Player { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoBootstrap()
    {
        EventManager.ClearAll();

        if (Instance != null) return;

        GameObject go = new GameObject("GameManager");
        go.AddComponent<GameManager>();
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetupCamera();
        SpawnPlayer();
        SetupSystems();

        EventManager.OnPlayerDied += HandlePlayerDied;
        EventManager.OnLevelUp += HandleLevelUp;
        EventManager.OnUpgradeSelected += HandleUpgradeSelected;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
        EventManager.OnPlayerDied -= HandlePlayerDied;
        EventManager.OnLevelUp -= HandleLevelUp;
        EventManager.OnUpgradeSelected -= HandleUpgradeSelected;
    }

    void SetupCamera()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        cam.orthographic = true;
        cam.orthographicSize = 10f;
        cam.backgroundColor = Color.black;
        cam.transform.position = new Vector3(0, 0, -10);
    }

    void SpawnPlayer()
    {
        GameObject playerObj = CircleFactory.CreateCircleWithPhysics(
            "Player", Color.blue, 0.4f, Vector2.zero
        );
        playerObj.tag = "Player";
        Player = playerObj.AddComponent<PlayerController>();
        playerObj.AddComponent<WeaponSystem>();
    }

    void SetupSystems()
    {
        gameObject.AddComponent<EnemySpawner>();
        gameObject.AddComponent<LevelSystem>();
        gameObject.AddComponent<UpgradeSystem>();
        gameObject.AddComponent<UIManager>();
    }

    void HandlePlayerDied()
    {
        State = GameState.GameOver;
        Time.timeScale = 0f;
    }

    void HandleLevelUp(int level)
    {
        State = GameState.Upgrading;
        Time.timeScale = 0f;
    }

    void HandleUpgradeSelected(UpgradeType type, float value)
    {
        State = GameState.Playing;
        Time.timeScale = 1f;
    }

    public void SetState(GameState state)
    {
        State = state;
    }
}
