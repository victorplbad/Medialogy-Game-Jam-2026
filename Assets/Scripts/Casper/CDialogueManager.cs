using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class CDialogueManager : MonoBehaviour
{
    public TextAsset CSV;
    private readonly List<string[]> data = new();

    private int currentMessage = 0;

    public TextMeshProUGUI textName;
    public TextMeshProUGUI textBox;
    public Image background;
    public CharacterSlide characterLeft;
    public CharacterSlide characterRight;

    public Transform buttonPanel;
    public GameObject prefab;

    public float letterDelay = 0.015f;
    float _letterDelay;

    private readonly Dictionary<string, Action> actionMap = new();
    //Dictionary<string, Image> backgrounds = new();
    //Dictionary<string, Image> character = new();
    public Sprite[] backgroundImages;

    bool blockInput = false;
    Action nextAction = null;

    readonly int Col_Name = 1;
    readonly int Col_Text = 2;
    readonly int Col_DisplayCharacters = 3;
    readonly int Col_Background = 4;
    readonly int Col_Functions = 5;

    //DirectoryInfo backgroundPath = new("/Assets/Images/Backgrounds/");
    //DirectoryInfo characterPath = new("/Assets/Images/Backgrounds/");


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string[] rows = CSV.text.Split('\n');
        for (int y = 0; y < rows.Length; y++)
        {
            string[] fields = rows[y].Split(',');

            for (int x = 0; x < fields.Length; x++)
            {
                fields[x] = fields[x].Replace(';', ',');
            }

            //print(fields.ToString());
            data.Add(fields);
        }

        actionMap.Add("jump", GoJump);
        actionMap.Add("fork", ForkInRoade);
        actionMap.Add("end", End);

        ShowMessage(1);

        FindFirstObjectByType<CInputManager>().onClickHandlers.Add(InputHandler);
    }

    void InputHandler()
    {
        if (blockInput) _letterDelay = 0;
        else nextAction();
    }

    void ShowMessage(int message)
    {
        //if (blockInput) return;
        if (currentMessage == message) return;
        currentMessage = message;

        var img = backgroundImages[int.Parse(data[currentMessage][Col_Background])];
        if (img == null) background.sprite = backgroundImages[0];
        else { background.sprite = img; }

        SetCharacterImages(data[currentMessage][Col_DisplayCharacters]);

        textName.text = data[currentMessage][Col_Name];
        StartCoroutine(WriteText(data[currentMessage][Col_Text], textBox, null));
    }

    void SetCharacterImages(string input)
    {
        if (input == null) return;
        if (input.Length == 0)
        {
            characterLeft.gameObject.SetActive(false);
            characterRight.gameObject.SetActive(false);
            return;
        }
        if (!input.Contains(":") && !input.StartsWith("[") && !input.EndsWith("]")) return; //Bad input

        input = input.Substring(1, input.Length - 2);   //Remove brackets(why did i decide to have those
        var IDs = input.Split(":");                     //Split left right

        if (IDs[0].Length == 0) characterLeft.SetCharacter();   //No ID, No Image
        else characterLeft.SetCharacter(int.Parse(IDs[0]));     //Set image
        if (IDs[1].Length == 0) characterRight.SetCharacter();
        else characterRight.SetCharacter(int.Parse(IDs[1]));
    }

    public IEnumerator WriteText(string input, TMP_Text textHolder, TMP_FontAsset tMP_Font)
    {
        if (tMP_Font != null) textHolder.font = tMP_Font;
        blockInput = true;
        _letterDelay = letterDelay;
        textHolder.text = "";

        for (int i = 0; i < input.Length; i++)
        {
            textHolder.text += input[i];
            if (_letterDelay > 0) yield return new WaitForSeconds(_letterDelay);
        }

        blockInput = false;
        DoneWriting();
    }

    void DoneWriting()
    {
        string command = data[currentMessage][Col_Functions].ToLower();
        if (actionMap.TryGetValue(command, out nextAction)) ;
        else nextAction = GoNext;
    }

    void GoNext()           //Default
    {
        ShowMessage(currentMessage + 1);
    }

    void GoJump()           //Jump
    {
        ShowMessage(int.Parse(data[currentMessage][Col_Functions + 1]));
    }

    void ForkInRoade()      //Fork
    {
        blockInput = true;
        for (int i = buttonPanel.childCount - 1; i >= 0; i--)
        {
            Destroy(buttonPanel.GetChild(i).gameObject);
        }

        buttonPanel.gameObject.SetActive(true);
        string[] row = data[currentMessage];
        int col = Col_Functions + 1;
        while (col + 1 <= row.Length && row[col].Length > 0 && row[col + 1].Length > 0)
        {
            GameObject GO = Instantiate(prefab, buttonPanel);

            Button button = GO.GetComponent<Button>();
            TextMeshProUGUI tmPro = GO.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

            tmPro.text = row[col];
            int veryVeryLocalNumberForAReason = col + 1;    //C# is weird ok
            button.onClick.AddListener(() => { ButtonListener(row[veryVeryLocalNumberForAReason]); });

            col += 2;
        }
    }

    void End()              //End
    {
        var ending = data[currentMessage][Col_Functions + 1].Trim().ToLower();

        if (ending == "bad_ending") Permanence.EndingID = 1;
        if (ending == "happy_ending") Permanence.EndingID = 2;
        SceneManager.LoadScene("Ending");
    }

    public void ButtonListener(string a)
    {
        blockInput = false;
        ShowMessage(int.Parse(a));
        buttonPanel.gameObject.SetActive(false);
    }
}