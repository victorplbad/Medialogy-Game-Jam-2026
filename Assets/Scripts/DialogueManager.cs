using System;
using System.Collections;
using System.Collections.Generic;
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
    public RawImage characterLeft;
    public RawImage characterRight;
    
    public Transform buttonPanel;
    public GameObject prefab;

    Dictionary<string, Action> actionMap = new();

    bool awaitInput = false;
    Action nextAction = null;

    readonly int Col_Text = 1;
    readonly int Col_DisplayCharacters = 2;
    readonly int Col_Background = 3;
    readonly int Col_Functions = 4;


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

        awaitInput = true;

        currentMessage = message;
        StartCoroutine(WriteText(data[currentMessage][Col_Text], textBox, null));
    }

    public IEnumerator WriteText(string input, TMP_Text textHolder, TMP_FontAsset tMP_Font)
    {
        if (tMP_Font != null) textHolder.font = tMP_Font;

        textHolder.text = ""; //nustil text først 
        for (int i = 0; i < input.Length; i++)
        {
            textHolder.text += input[i];
            yield return new WaitForSeconds(0.025f);
        }
        yield return new WaitForSeconds(0.5f);
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
    }
}