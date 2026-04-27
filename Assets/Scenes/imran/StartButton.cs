using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class StartButton : MonoBehaviour
{

    public void MoveToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Exited");
    }
}
