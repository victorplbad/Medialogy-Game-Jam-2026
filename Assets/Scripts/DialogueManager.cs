using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class DialogueManager : MonoBehaviour
{
    public TextAsset CSV;
    private List<string[]> data = new();

    private int currentMessage = 0;

    public TextMeshProUGUI textName;
    public TextMeshProUGUI textBox;
    public Image background;
    public CharacterSlide characterLeft;
    public CharacterSlide characterRight;
    
    public Transform buttonPanel;
    public GameObject prefab;

    Dictionary<string, Action> actionMap = new();
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

    // New Auto play mode
    [Header("Auto Play")]
    [SerializeField] private bool autoMode = false;
    [SerializeField] private float autoDelay = 2.5;
    private Coroutine autoCoroutine;

    // Long press on-screen-gesture for autoPlay
    private float pressTimer = 0f;
    private bool longPressedTriggered = false;

    [SerializeField] private float LongPressTime = 0.5;


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

        showMessage(1);
    }

    bool lClick;
    // Update is called once per frame
    void Update()
{
    // NEW: KEYBOARD INPUT, Q toggles auto mode

    if (Keyboard.current != null &&
        Keyboard.current.qKey.wasPressedThisFrame)
    {
        ToggleAutoMode();
    }

    // NEW: POINTER INPUT (mouse + touch)
    // Left click = next dialogue, Long press = toggle auto mode

    bool pressed = CheckInput();

    if (pressed)
    {
        pressTimer += Time.deltaTime;

        // LONG PRESS spottet
        if (!longPressTriggered &&
            pressTimer >= longPressTime)
        {
            longPressTriggered = true;

            ToggleAutoMode();
        }
    }

    if (!pressed && lClick)
    {
        // LEFT CLICK (tap) = NEXT DIALOGUE

        if (!longPressTriggered)
        {
            // Manual input disables auto mode
            autoMode = false;

            if (!blockInput && nextAction != null)
            {
                nextAction();
            }
        }

        // reset gesture
        pressTimer = 0f;
        longPressTriggered = false;
    }

    lClick = pressed;
}
// Method for AutoMode toggle on
void ToggleAutoMode()
{
    autoMode = !autoMode;

    Debug.Log("Auto Mode: " + autoMode);

    if (autoMode &&
        !blockInput &&
        nextAction != null)
    {
        if (autoCoroutine != null)
        {
            StopCoroutine(autoCoroutine);
        }

        autoCoroutine = StartCoroutine(AutoNext());
    }
}
// Logic behind Auto Play function 

IEnumerator AutoNext()
{
    string line = data[currentMessage][Col_Text];

    float delay = 1.5f + (line.Length * 0.04f);

    yield return new WaitForSeconds(delay);

    if (!blockInput &&
        autoMode &&
        nextAction != null)
    {
        nextAction();
    }
}

    void showMessage(int message) {
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
        if (!input.Contains(":")) return;

        input = input.Substring(1, input.Length - 2);
        var IDs = input.Split(":");

        if (IDs[0].Length == 0) characterLeft.SetCharacter();
        else
        {
            characterLeft.SetCharacter(int.Parse(IDs[0]));
        }
        if (IDs[1].Length == 0) characterRight.SetCharacter();
        else
        {
            characterRight.SetCharacter(int.Parse(IDs[1]));
        }
    }

    public IEnumerator WriteText(string input, TMP_Text textHolder, TMP_FontAsset tMP_Font)
    {
        if (tMP_Font != null) textHolder.font = tMP_Font;
        blockInput = true;

        textHolder.text = ""; //nustil text f�rst 
        for (int i = 0; i < input.Length; i++)
        {
            textHolder.text += input[i];
            yield return new WaitForSeconds(0.015f);
        }
        //yield return new WaitForSeconds(0.5f);
        blockInput = false;

        DoneWriting();
    }

    void DoneWriting()
    {
        string command = data[currentMessage][Col_Functions].ToLower();
        if (actionMap.TryGetValue(command, out nextAction));
        else nextAction = GoNext;

        // NEW: AUTO MODE TRIGGER AFTER TEXT FINISHES
        if (autoMode)
        {
        if (autoCoroutine != null)
        {
            StopCoroutine(autoCoroutine);
        }

        autoCoroutine = StartCoroutine(AutoNext());
        }
    }

    private bool CheckInput()
    {
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

    void GoJump()
    {
        showMessage(int.Parse(data[currentMessage][Col_Functions + 1]));
    }

    void ForkInRoade()
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
            int veryVeryLocalNumberForAReason = col + 1;
            button.onClick.AddListener(() => { ButtonListener(row[veryVeryLocalNumberForAReason]); });

            col += 2;
        }
    }

    public void ButtonListener(string a)
    {
        blockInput = false;
        showMessage(int.Parse(a));
        buttonPanel.gameObject.SetActive(false);
    }

    void End()
    {
        var ending = data[currentMessage][Col_Functions + 1].Trim().ToLower();

        if(ending == "bad_ending") Permanence.EndingID = 1;
        if(ending == "happy_ending") Permanence.EndingID = 2;
        SceneManager.LoadScene("Ending");
    }
}

public static class Permanence
{
    public static int EndingID;
}