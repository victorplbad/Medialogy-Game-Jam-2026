using TMPro;
using UnityEngine;

public class fade_in : MonoBehaviour
{
    public TextMeshProUGUI text;

    float iniatialAlpha;
    float timer;

    // Start is called once befo
    // re the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        iniatialAlpha = text.alpha;
        text.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < 1) timer += Time.deltaTime;
        text.alpha = timer;
        text.transform.localScale = Vector3.one * timer;
        
    }
}
