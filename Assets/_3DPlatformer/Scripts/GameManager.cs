using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameState { Playing, Paused, Won, Lost }
    public GameState state;

    [Header("Scenes")]
    public string titleSceneName = "TitleScreen";

    [Header("Player")]
    public PlayerControllerScript player;
    public Transform startPoint;

    [Header("UI")]
    public TMP_Text coinText;
    public TMP_Text healthText;
    public TMP_Text winCoinText;
    public GameObject pausePanel;
    public GameObject winPanel;
    public GameObject losePanel;

    public int coins;

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1;
    }

    private void Start()
    {
        // Move the player to the start point
        if (player != null && startPoint != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            cc.enabled = false; // CharacterController must be off to teleport
            player.transform.SetPositionAndRotation(startPoint.position, startPoint.rotation);
            cc.enabled = true;
        }

        pausePanel.SetActive(false);
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        SetCursor(false);
        state = GameState.Playing;
        UpdateCoinUI();
    }

    private void Update()
    {
        if (Keyboard.current == null) return;
        if (Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.pKey.wasPressedThisFrame)
        {
            if (state == GameState.Playing) PauseGame();
            else if (state == GameState.Paused) ResumeGame();
        }
    }

    public void AddCoin(int amount)
    {
        coins += amount;
        UpdateCoinUI();
    }

    public void UpdateHealthUI(int current, int max)
    {
        if (healthText != null) healthText.text = "Health: " + current + " / " + max;
    }

    void UpdateCoinUI()
    {
        if (coinText != null) coinText.text = "Coins: " + coins;
    }

    public void PauseGame()
    {
        state = GameState.Paused;
        Time.timeScale = 0;
        player.enabled = false;
        pausePanel.SetActive(true);
        SetCursor(true);
    }

    public void ResumeGame()
    {
        state = GameState.Playing;
        Time.timeScale = 1;
        player.enabled = true;
        pausePanel.SetActive(false);
        SetCursor(false);
    }

    public void WinLevel()
    {
        if (state != GameState.Playing) return;
        state = GameState.Won;
        if (winCoinText != null) winCoinText.text = "Coins Collected: " + coins;
        EndGame(winPanel);
    }

    public void GameOver()
    {
        if (state != GameState.Playing) return;
        state = GameState.Lost;
        EndGame(losePanel);
    }

    void EndGame(GameObject panel)
    {
        player.enabled = false;
        panel.SetActive(true);
        Time.timeScale = 0;
        SetCursor(true);
    }

    // Button functions
    public void RestartLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadTitle()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(titleSceneName);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // NEW - stops Play mode in the Editor
#else
        Application.Quit(); // Closes the real built game
#endif
    }

    void SetCursor(bool show)
    {
        Cursor.visible = show;
        Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
    }
}