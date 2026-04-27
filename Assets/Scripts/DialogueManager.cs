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

    Dictionary<string, Action> actionMap = new();

    bool typing = false;
    Action nextAction = null;

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

        actionMap.Add("", GoNext);
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
        if (typing) return;
        if (currentMessage == message) return;

        typing = true;

        currentMessage = message;
        StartCoroutine(WriteText(data[currentMessage][1], textBox, null));
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
        typing = false;

        DoneWriting();
    }

    void DoneWriting()
    {
        string command = data[currentMessage][3].ToLower();
        actionMap.TryGetValue(command, out nextAction);
    }

    private bool CheckInput()
    {
        if (typing) return false;

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
        print("OH NO!!");
        print(data[currentMessage][4]);
        currentMessage = int.Parse(data[currentMessage][4]);
    }
}