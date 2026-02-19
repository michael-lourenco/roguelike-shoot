using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private Image hpBarFill;
    private Image xpBarFill;
    private Text levelText;
    private Text killText;
    private Text timerText;

    private GameObject upgradePanel;
    private Button[] upgradeButtons;
    private Text[] upgradeTexts;

    private GameObject gameOverPanel;
    private Text gameOverStatsText;

    private Canvas canvas;
    private float gameTimer;

    void Start()
    {
        CreateCanvas();
        CreateHUD();
        CreateUpgradePanel();
        CreateGameOverPanel();

        EventManager.OnLevelUp += ShowUpgradePanel;
        EventManager.OnUpgradeSelected += HideUpgradePanel;
        EventManager.OnPlayerDied += ShowGameOver;
    }

    void OnDestroy()
    {
        EventManager.OnLevelUp -= ShowUpgradePanel;
        EventManager.OnUpgradeSelected -= HideUpgradePanel;
        EventManager.OnPlayerDied -= ShowGameOver;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.State == GameState.Playing)
            gameTimer += Time.deltaTime;

        UpdateHUD();
        HandleInput();
    }

    void CreateCanvas()
    {
        GameObject canvasObj = new GameObject("UICanvas");
        canvasObj.transform.SetParent(transform);
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObj.AddComponent<GraphicRaycaster>();

        if (Object.FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }
    }

    #region UI Factory Helpers

    GameObject CreatePanel(string name, Transform parent,
        Vector2 anchorMin, Vector2 anchorMax,
        Vector2 offsetMin, Vector2 offsetMax, Color color)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);

        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = offsetMin;
        rt.offsetMax = offsetMax;

        Image img = panel.AddComponent<Image>();
        img.color = color;

        return panel;
    }

    Text CreateText(string name, Transform parent,
        Vector2 anchorMin, Vector2 anchorMax,
        Vector2 offsetMin, Vector2 offsetMax,
        string content, int fontSize,
        TextAnchor alignment = TextAnchor.MiddleCenter)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);

        RectTransform rt = textObj.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = offsetMin;
        rt.offsetMax = offsetMax;

        Text text = textObj.AddComponent<Text>();
        text.text = content;
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = Color.white;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;

        return text;
    }

    #endregion

    void CreateHUD()
    {
        // HP Bar
        GameObject hpBg = CreatePanel("HPBarBg", canvas.transform,
            new Vector2(0, 1), new Vector2(0, 1),
            new Vector2(20, -50), new Vector2(320, -20),
            new Color(0.3f, 0, 0, 0.8f));

        GameObject hpFill = CreatePanel("HPBarFill", hpBg.transform,
            Vector2.zero, Vector2.one,
            Vector2.zero, Vector2.zero,
            new Color(1f, 0.2f, 0.2f, 1f));
        hpBarFill = hpFill.GetComponent<Image>();
        hpBarFill.type = Image.Type.Filled;
        hpBarFill.fillMethod = Image.FillMethod.Horizontal;

        CreateText("HPLabel", hpBg.transform,
            Vector2.zero, Vector2.one,
            new Vector2(5, 0), new Vector2(-5, 0),
            "HP", 16, TextAnchor.MiddleLeft);

        // XP Bar
        GameObject xpBg = CreatePanel("XPBarBg", canvas.transform,
            new Vector2(0, 1), new Vector2(0, 1),
            new Vector2(20, -80), new Vector2(320, -55),
            new Color(0, 0.2f, 0, 0.8f));

        GameObject xpFill = CreatePanel("XPBarFill", xpBg.transform,
            Vector2.zero, Vector2.one,
            Vector2.zero, Vector2.zero,
            new Color(0.2f, 1f, 0.2f, 1f));
        xpBarFill = xpFill.GetComponent<Image>();
        xpBarFill.type = Image.Type.Filled;
        xpBarFill.fillMethod = Image.FillMethod.Horizontal;

        CreateText("XPLabel", xpBg.transform,
            Vector2.zero, Vector2.one,
            new Vector2(5, 0), new Vector2(-5, 0),
            "XP", 16, TextAnchor.MiddleLeft);

        // Level text (top-left)
        levelText = CreateText("LevelText", canvas.transform,
            new Vector2(0, 1), new Vector2(0, 1),
            new Vector2(20, -100), new Vector2(320, -80),
            "Nível 1", 20, TextAnchor.MiddleLeft);

        // Kill counter (top-right)
        killText = CreateText("KillText", canvas.transform,
            new Vector2(1, 1), new Vector2(1, 1),
            new Vector2(-320, -50), new Vector2(-20, -20),
            "Kills: 0", 22, TextAnchor.MiddleRight);

        // Timer (top-center)
        timerText = CreateText("TimerText", canvas.transform,
            new Vector2(0.5f, 1), new Vector2(0.5f, 1),
            new Vector2(-100, -50), new Vector2(100, -20),
            "00:00", 28, TextAnchor.MiddleCenter);
    }

    void CreateUpgradePanel()
    {
        upgradePanel = CreatePanel("UpgradePanel", canvas.transform,
            Vector2.zero, Vector2.one,
            Vector2.zero, Vector2.zero,
            new Color(0, 0, 0, 0.85f));

        CreateText("UpgradeTitle", upgradePanel.transform,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(-300, 180), new Vector2(300, 240),
            "SUBIU DE NÍVEL!", 40);

        CreateText("UpgradeSubtitle", upgradePanel.transform,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(-300, 140), new Vector2(300, 180),
            "Escolha um upgrade:", 24);

        upgradeButtons = new Button[3];
        upgradeTexts = new Text[3];

        for (int i = 0; i < 3; i++)
        {
            float yCenter = 60f - i * 90f;

            GameObject btnObj = CreatePanel($"UpgradeBtn{i}", upgradePanel.transform,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(-200, yCenter - 35), new Vector2(200, yCenter + 35),
                new Color(0.15f, 0.15f, 0.35f, 1f));

            Button btn = btnObj.AddComponent<Button>();
            ColorBlock colors = btn.colors;
            colors.normalColor = new Color(0.15f, 0.15f, 0.35f, 1f);
            colors.highlightedColor = new Color(0.25f, 0.25f, 0.55f, 1f);
            colors.pressedColor = new Color(0.1f, 0.1f, 0.25f, 1f);
            colors.selectedColor = new Color(0.2f, 0.2f, 0.45f, 1f);
            btn.colors = colors;

            Text btnText = CreateText($"BtnText{i}", btnObj.transform,
                Vector2.zero, Vector2.one,
                new Vector2(10, 5), new Vector2(-10, -5),
                "", 20);

            int index = i;
            btn.onClick.AddListener(() => OnUpgradeClicked(index));

            upgradeButtons[i] = btn;
            upgradeTexts[i] = btnText;
        }

        upgradePanel.SetActive(false);
    }

    void CreateGameOverPanel()
    {
        gameOverPanel = CreatePanel("GameOverPanel", canvas.transform,
            Vector2.zero, Vector2.one,
            Vector2.zero, Vector2.zero,
            new Color(0.4f, 0, 0, 0.9f));

        CreateText("GameOverTitle", gameOverPanel.transform,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(-300, 40), new Vector2(300, 120),
            "GAME OVER", 56);

        gameOverStatsText = CreateText("GameOverStats", gameOverPanel.transform,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(-300, -40), new Vector2(300, 40),
            "", 24);

        CreateText("RestartHint", gameOverPanel.transform,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(-300, -100), new Vector2(300, -60),
            "Pressione R para reiniciar", 22);

        gameOverPanel.SetActive(false);
    }

    void UpdateHUD()
    {
        if (GameManager.Instance == null) return;

        PlayerController player = GameManager.Instance.Player;
        if (player != null)
        {
            hpBarFill.fillAmount = player.currentHealth / player.maxHealth;
            killText.text = $"Kills: {player.killCount}";
        }

        LevelSystem levelSys = GetComponent<LevelSystem>();
        if (levelSys != null)
        {
            levelText.text = $"Nível {levelSys.currentLevel}";
            xpBarFill.fillAmount = levelSys.GetXPProgress();
        }

        int minutes = Mathf.FloorToInt(gameTimer / 60f);
        int seconds = Mathf.FloorToInt(gameTimer % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    void HandleInput()
    {
        if (GameManager.Instance.State == GameState.GameOver && Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            GameManager.Instance.SetState(GameState.Playing);
            Destroy(GameManager.Instance.gameObject);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void ShowUpgradePanel(int level)
    {
        UpgradeSystem upgSys = GetComponent<UpgradeSystem>();
        if (upgSys?.currentOptions == null) return;

        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            if (i < upgSys.currentOptions.Length)
            {
                var opt = upgSys.currentOptions[i];
                upgradeTexts[i].text = $"{opt.Name}\n<size=16>{opt.Description}</size>";
                upgradeButtons[i].gameObject.SetActive(true);
            }
            else
            {
                upgradeButtons[i].gameObject.SetActive(false);
            }
        }

        upgradePanel.SetActive(true);
    }

    void HideUpgradePanel(UpgradeType type, float value)
    {
        upgradePanel.SetActive(false);
    }

    void ShowGameOver()
    {
        PlayerController player = GameManager.Instance?.Player;
        LevelSystem lvl = GetComponent<LevelSystem>();

        int kills = player != null ? player.killCount : 0;
        int level = lvl != null ? lvl.currentLevel : 1;
        int minutes = Mathf.FloorToInt(gameTimer / 60f);
        int seconds = Mathf.FloorToInt(gameTimer % 60f);

        gameOverStatsText.text = $"Kills: {kills}  |  Nível: {level}  |  Tempo: {minutes:00}:{seconds:00}";
        gameOverPanel.SetActive(true);
        upgradePanel.SetActive(false);
    }

    void OnUpgradeClicked(int index)
    {
        UpgradeSystem upgSys = GetComponent<UpgradeSystem>();
        upgSys?.SelectUpgrade(index);
    }
}
