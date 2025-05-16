using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void SwitchToScene(int index)
    {
        SceneManager.LoadScene(index);
    }
    public void QuitGame()
    {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}
