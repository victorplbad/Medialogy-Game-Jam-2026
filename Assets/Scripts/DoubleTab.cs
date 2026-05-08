using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class Gestures_DoubleTap : MonoBehaviour
{
    [SerializeField] GameObject square;
    [SerializeField] float maxTapDelay = 0.3f; // seconds between taps
    public DialogueManager dialogueManager;

    float lastTapTime = 0f;

    void Awake()
    {
        EnhancedTouchSupport.Enable();
    }

    void Update()
    {
        if (touch.activeTouches.Count < 1)
            return;

        var touch1 = touch.activeTouches[0];

        if (touch1.phase == UnityEngine.InputSystem.TouchPhase.Began)
        {
            float timeSinceLastTap = Time.time - lastTapTime;

            if (timeSinceLastTap <= maxTapDelay)
            {
                // Double tap detected!
                dialogueManager.DoubleTab();
                
                lastTapTime = 0f; // reset
            }
            else
            {
                lastTapTime = Time.time;
            }
        }
    }
}
