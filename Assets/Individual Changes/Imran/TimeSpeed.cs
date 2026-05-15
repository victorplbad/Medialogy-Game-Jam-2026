using UnityEngine;
using UnityEngine.InputSystem;

public class TimeSpeed : MonoBehaviour
{
    private bool isFast = false;
    public GameObject Double;

    void Start()
    {
       
        if (Double != null)
        {
            Double.SetActive(false);
        }
    }

    void Update()
    {
        
        if (Keyboard.current == null) return;

      
        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            if (isFast)
            {
                Time.timeScale = 1.0f; // Normal Speed
                isFast = false;
                if (Double != null) Double.SetActive(false);

            }
            else
            {
                Time.timeScale = 2.0f; // 2x Speed
                isFast = true;
                if (Double != null) Double.SetActive(true);
            }

            
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            Debug.Log("TimeScale is now: " + Time.timeScale);
        }
    }
}