using Minefactory.Save;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    public GameObject furnaceUI;
    public GameObject crafterUI;
    public GameObject inventoryUI;
    public GameObject skillTreeUI;
    public GameObject pauseMenuUI;
    public GameObject howToPlayUI;


    private bool isPaused = false;
    private bool isModalOpen = false;

    public void OpenPauseMenu()
    {
        CloseAllUI();
        isModalOpen = true;
        pauseMenuUI.SetActive(true);
        Pause();
    }

    public void OpenFurnaceUI()
    {
        CloseAllUI();
        isModalOpen = true;
        furnaceUI.SetActive(true);
    }

    public void OpenCrafterUI()
    {
        CloseAllUI();
        isModalOpen = true;
        crafterUI.SetActive(true);
    }

    public void OpenInventoryUI()
    {
        CloseAllUI();
        isModalOpen = true;
        inventoryUI.SetActive(true);
    }

    public void OpenSkillTreeUI()
    {
        CloseAllUI();
        isModalOpen = true;
        skillTreeUI.SetActive(true);
    }

    public void CloseAllUI()
    {
        inventoryUI.SetActive(false);
        furnaceUI.SetActive(false);
        crafterUI.SetActive(false);
        skillTreeUI.SetActive(false);
        pauseMenuUI.SetActive(false);
        howToPlayUI.SetActive(false);
        isModalOpen = false;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else if (isModalOpen)
            {
                CloseAllUI();
            }
            else
            {
                OpenPauseMenu();
            }
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            OpenInventoryUI();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            OpenSkillTreeUI();
        }
    }


    public void Resume()
    {
        CloseAllUI();
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ShowInstructions()
    {
        pauseMenuUI.SetActive(false);
        howToPlayUI.SetActive(true);
    }

    public void HideInstructions()
    {
        howToPlayUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SaveManager.Instance.SaveGame();
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

