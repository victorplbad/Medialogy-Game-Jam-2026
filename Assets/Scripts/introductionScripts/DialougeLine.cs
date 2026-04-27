
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DialougeSystem
{
    public class DialougeLine : DialougeBaseClass
    {
        [Header("Text Options")]
        // private TMP_Text textHolder;
        [SerializeField] private TMP_Text textHolder;
        [SerializeField] private string input;
        [SerializeField]private TMP_FontAsset tmp_Font;

        private void Awake()
        {
            // textHolder = GetComponent<TMP_Text>(); // Get the TMP_Text component

            // Debug.Log(textHolder);

            StartCoroutine(WriteText(input, textHolder, tmp_Font)); // Start the coroutine to write text


        }
    }
}

