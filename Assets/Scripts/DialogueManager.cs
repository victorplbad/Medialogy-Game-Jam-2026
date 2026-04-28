using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class Dialogue : MonoBehaviour
{
    public TextAsset CSV;
    private List<string[]> data = new();

    private int currentMessage = 0;

    public TextMeshProUGUI textBox;
    public Image background;
    public Image characterLeft;
    public Image characterRight;
    
    public Transform buttonPanel;
    public GameObject prefab;

    Dictionary<string, Action> actionMap = new();
    //Dictionary<string, Image> backgrounds = new();
    //Dictionary<string, Image> character = new();
    public string[] backgroundNames;
    public Sprite[] backgroundImages;
    public Sprite[] characterImages;

    bool awaitInput = false;
    Action nextAction = null;

    readonly int Col_Text = 1;
    readonly int Col_DisplayCharacters = 2;
    readonly int Col_Background = 3;
    readonly int Col_Functions = 4;

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

        nextAction = GoNext;
        actionMap.Add("fork", ForkInRoade);


    }

    // Update is called once per frame
    void Update()
    {
        //print(!typing + ":"+ currentMessage);
        //print(typing);
        //if (!typing) showMessage(currentMessage + 1);
        if(CheckInput() && nextAction != null) nextAction();
    }

    void showMessage(int message) {
        if (awaitInput) return;
        if (currentMessage == message) return;
        currentMessage = message;

        awaitInput = true;

        var img = GetImage(data[currentMessage][Col_Background]);
        if (img != null) { background.sprite = img; }
        else { background.sprite = null; }

        SetCharacterImages(data[currentMessage][Col_DisplayCharacters]);

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
        if (!input.Contains(":")) return;

        input = input.Substring(1, input.Length - 2);
        var IDs = input.Split(":");

        if (IDs[0].Length == 0) characterLeft.gameObject.SetActive(false);
        else
        {
            print(IDs[0]);
            characterLeft.gameObject.SetActive(true);
            characterLeft.sprite = characterImages[int.Parse(IDs[0])];
        }
        if (IDs[1].Length == 0) characterRight.gameObject.SetActive(false);
        else
        {
            print(IDs[1]);
            characterRight.gameObject.SetActive(true);
            characterRight.sprite = characterImages[int.Parse(IDs[1])];
        }
    }

    Sprite GetImage(string name)
    {
        for (int i = 0; i < backgroundNames.Length; i++)
        {
            if (backgroundNames[i] == name) return backgroundImages[i];
        }
        return null;
    }

    public IEnumerator WriteText(string input, TMP_Text textHolder, TMP_FontAsset tMP_Font)
    {
        if (tMP_Font != null) textHolder.font = tMP_Font;

        textHolder.text = ""; //nustil text først 
        for (int i = 0; i < input.Length; i++)
        {
            textHolder.text += input[i];
            yield return new WaitForSeconds(0.015f);
        }
        //yield return new WaitForSeconds(0.5f);
        awaitInput = false;

        DoneWriting();
    }

    void DoneWriting()
    {
        string command = data[currentMessage][Col_Functions].ToLower();
        if (actionMap.TryGetValue(command, out nextAction));
        else nextAction = GoNext;
    }

    private bool CheckInput()
    {
        if (awaitInput) return false;

        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            return true;
        }

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            return true;
        }

        return false;
    }

    void GoNext()
    {
        showMessage(currentMessage + 1);
    }

    void ForkInRoade()
    {
        awaitInput = true;
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
            int veryVeryLocalNumberForAReason = col + 1;
            button.onClick.AddListener(() => { ButtonListener(row[veryVeryLocalNumberForAReason]); });

            col += 2;
        }
    }

    public void ButtonListener(string a)
    {
        awaitInput = false;
        showMessage(int.Parse(a));
        buttonPanel.gameObject.SetActive(false);
    }
}