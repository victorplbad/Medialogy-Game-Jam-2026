using UnityEngine;

public class EndingThing : MonoBehaviour
{
    public int endingID;

    void Start()
    {
        gameObject.SetActive(endingID == Permanence.EndingID);
    }
}
