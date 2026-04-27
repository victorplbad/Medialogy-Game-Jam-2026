using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.TextCore.Text;

namespace DialougeSystem

{
    public class DialougeBaseClass : MonoBehaviour
    {
       protected IEnumerator WriteText(string input, TMP_Text textHolder, TMP_FontAsset tMP_Font)
        {
            textHolder.font= tMP_Font; 
        
            textHolder.text = ""; //nustil text først 
            for(int i=0; i < input.Length; i++)
            
            {
                textHolder.text += input[i];
                yield return new WaitForSeconds(0.035f);
            }
        }
    }
}
