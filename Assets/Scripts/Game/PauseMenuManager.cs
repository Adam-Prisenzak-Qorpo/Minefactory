using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public static bool isPaused = false;
    public GameObject pauseMenuUI;
    public GameObject instructionsUI;  // Reference to the instructions panel
    
    void Start()
    {
        // Ensure both menus are hidden at start
        pauseMenuUI.SetActive(false);
        instructionsUI.SetActive(false);
        isPaused = false;
    }

    void Update()
    {
        // Check for escape key press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape key pressed"); // This will show in the console when you press Escape
            if (isPaused)
            {
                 Debug.Log("Trying to resume"); // Debug pause state
                if (instructionsUI.activeSelf)
                {
                    Debug.Log("Hiding instructions");
                    // If instructions are showing, hide them first
                    HideInstructions();
                }
                else
                {
                    Resume();
                }
            }
            else
            {
                Debug.Log("Trying to pause"); // Debug pause state
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        instructionsUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    void Pause()
    {
        Debug.Log("Pause function called"); // Debug pause function
        if (pauseMenuUI == null)
        {
            Debug.LogError("pauseMenuUI is not assigned!"); // Check if reference is missing
            return;
        }
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ShowInstructions()
    {
        pauseMenuUI.SetActive(false);
        instructionsUI.SetActive(true);
        // Game remains paused while showing instructions
    }

    public void HideInstructions()
    {
        instructionsUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // Replace with your main menu scene name
    }

    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}