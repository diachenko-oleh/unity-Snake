using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void SwitchToScene(int index)
    {
        SceneManager.LoadScene(index);
    }
}
