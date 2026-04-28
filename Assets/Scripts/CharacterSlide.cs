using UnityEngine;
using UnityEngine.UI;

public class CharacterSlide : MonoBehaviour
{
    float timer;
    Vector2 initialPos;
    Vector2 farPos;

    private Image image;
    public int currentCharacter;
    public Sprite[] characterImages;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        initialPos = transform.position;
        farPos = transform.position;
        farPos.x = initialPos.x * 2;

        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        transform.position = Vector2.Lerp(farPos, initialPos, timer * 1.5f);
    }

    private void Animate(int ID = 0)
    {
        timer = 0;
        transform.position = farPos;
    }

    public void SetCharacter(int ID = 0)
    {
        if (ID == 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            image.sprite = characterImages[ID];
            if (ID != currentCharacter) Animate();
        }
        currentCharacter = ID;
    }
}