using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManage : MonoBehaviour
{
    public Animator animator;
    public string triggerName = "Play";
    public string sceneToLoad;

    // Call this function to start the process (e.g., from a button click)
    public void PlayAnimationAndChangeScene()
    {
        if (animator != null)
        {
            animator.SetTrigger(triggerName);
        }
        else
        {
            Debug.LogError("Animator not assigned!");
        }
    }

    // This function will be called by the Animation Event at the end of the clip
    public void OnAnimationComplete()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("Scene name to load is empty!");
        }
    }

            public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Exited");
    }
}

