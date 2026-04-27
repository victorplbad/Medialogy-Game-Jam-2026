using UnityEngine;
using System.IO;

public class CSVRuntimeReader : MonoBehaviour
{
    void Start()
    {
        TextAsset file = Resources.Load<TextAsset>("test123");
        if (file != null)
        {
            string[] rows = file.text.Split('\n');
            foreach (string row in rows)
            {
                string[] fields = row.Split('\t');
                Debug.Log(string.Join(" | ", fields));
            }
        }
        else
        {
            Debug.LogError("File not found in Resources!");
        }
    }
}
