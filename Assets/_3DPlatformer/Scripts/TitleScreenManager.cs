using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    // Must match the level scene's name exactly
    public string firstLevelName = "SampleScene";

    private void Start()
    {
        // Make sure the game isn't paused and the mouse is usable on the menu
        Time.timeScale = 1;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(firstLevelName);
    }

    public void QuitGame()
    {
        Debug.Log("Quit button pressed"); // NEW - shows in the Console so you know the button works
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // NEW - stops Play mode in the Editor
#else
        Application.Quit(); // Closes the real built game
#endif
    }
}