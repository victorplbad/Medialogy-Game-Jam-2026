using UnityEngine;

public class EndingThing : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(Permanence.ending.ToLower() == gameObject.name.ToLower());
    }
}
