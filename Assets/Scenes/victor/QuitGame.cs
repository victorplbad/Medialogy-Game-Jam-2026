using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitGame : MonoBehaviour
{
    public string sceneToLoad;

    public void Quit()
    {

        Debug.Log("closing game");
        Application.Quit();

    }


    public void RestartGame()
    {

        Debug.Log("Restarting game");
        SceneManager.LoadScene(sceneToLoad);

    }
}
