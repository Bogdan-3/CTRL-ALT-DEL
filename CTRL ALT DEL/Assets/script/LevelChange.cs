using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChange : MonoBehaviour
{
    public void Change(string NameScene)
    {
        SceneManager.LoadScene(NameScene);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
