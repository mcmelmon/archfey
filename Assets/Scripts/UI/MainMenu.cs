using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Inspector settings

    public GameObject menu_panel;


    // Unity


    private void Update()
    {
        if (Input.GetKeyDown("escape")) {
            menu_panel.SetActive(!menu_panel.activeSelf);
        }
    }


    // public


    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }


    public void ShowMenu()
    {
        LoadByIndex(0);
    }


    public void StartGame()
    {
        LoadByIndex(1);
    }


    // private


    private void LoadByIndex(int scene_index)
    {
        SceneManager.LoadScene(scene_index);
    }
}
