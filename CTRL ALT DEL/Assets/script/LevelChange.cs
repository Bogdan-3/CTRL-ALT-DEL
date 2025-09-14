using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChange : MonoBehaviour
{
    public string Level;
    public void Change(string NameScene)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(NameScene);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        SceneManager.LoadScene(Level);
    }
}
