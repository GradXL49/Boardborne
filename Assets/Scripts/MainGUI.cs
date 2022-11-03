using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGUI : MonoBehaviour
{
    //variables
    PlayerCharacter player;
    Vector3 max;
    Vector3 current;
    Rect healthBar;
    Rect currentHealth;
    Rect staminaBar;
    Rect currentStamina;
    Rect manaBar;
    Rect currentMana;
    Texture2D border;
    Texture2D health;
    Texture2D stamina;
    Texture2D mana;

    Rect currencyIcon;
    Rect currencyArea;
    Texture2D currency;
    int currencyTotal;
    GUIStyle currencyStyle;

    GameLogic game;
    GUIStyle textStyle;
    GUIStyle borderStyle;
    [SerializeField] Font customFont;
    Rect typingArea;
    string typing;

    float barSize;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("HeroKnight").GetComponent<PlayerCharacter>();
        game = GameObject.Find("GameLogic").GetComponent<GameLogic>();

        border = Resources.Load("border") as Texture2D;
        health = Resources.Load("health") as Texture2D;
        stamina = Resources.Load("stamina") as Texture2D;
        mana = Resources.Load("mana") as Texture2D;
        currency = Resources.Load("currency") as Texture2D;

        currencyStyle = new GUIStyle();
        currencyStyle.richText = true;
        currencyStyle.normal.textColor = Color.white;
        currencyStyle.alignment = TextAnchor.MiddleRight;
        currencyStyle.font = customFont;
        currencyStyle.fontSize = 30;

        textStyle = new GUIStyle();
        textStyle.richText = true;
        textStyle.normal.textColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        textStyle.alignment = TextAnchor.MiddleCenter;
        textStyle.font = customFont;
        textStyle.fontSize = 50;

        borderStyle = new GUIStyle();
        borderStyle.richText = true;
        borderStyle.normal.textColor = Color.white;
        borderStyle.alignment = TextAnchor.MiddleCenter;
        borderStyle.font = customFont;
        borderStyle.fontSize = 50;
    }

    // Update is called once per frame
    void Update()
    {
        barSize = Screen.height*0.03f;
        updateStatusBars();
        updateTypingArea();
        updateCurrencyArea();
    }

    void OnGUI() {
        drawStatusBars();
        drawTyping();
        drawCurrencyArea();
    }

    void updateStatusBars() {
        max = player.getMax();
        current = player.getStatus();

        healthBar = new Rect(Screen.height*0.05f, barSize, Screen.width*0.2f*(max.x/100), barSize);
        staminaBar = new Rect(Screen.height*0.05f, Screen.height*0.065f, Screen.width*0.2f*(max.y/100), barSize);
        manaBar = new Rect(Screen.height*0.05f, Screen.height*0.10f, Screen.width*0.2f*(max.z/100), barSize);

        currentHealth = new Rect(healthBar.x, healthBar.y, healthBar.width*(current.x/max.x), healthBar.height);
        currentStamina = new Rect(staminaBar.x, staminaBar.y, staminaBar.width*(current.y/max.y), staminaBar.height);
        currentMana = new Rect(manaBar.x, manaBar.y, manaBar.width*(current.z/max.z), manaBar.height);
    }

    void drawStatusBars() {
        GUI.DrawTexture(currentHealth, health);
        GUI.DrawTexture(currentStamina, stamina);
        GUI.DrawTexture(currentMana, mana);
        
        GUI.DrawTexture(healthBar, border);
        GUI.DrawTexture(staminaBar, border);
        GUI.DrawTexture(manaBar, border);
    }

    void updateTypingArea() {
        typing = game.getTyping();
        typingArea = new Rect(Screen.width/4f, manaBar.y, Screen.width/2, manaBar.height);
    }

    void drawTyping() {
        Rect temp = new Rect(typingArea.x-2, typingArea.y-2, typingArea.width, typingArea.height);
        GUI.Label(typingArea, typing, borderStyle);
        GUI.Label(temp, typing, textStyle);
    }

    void updateCurrencyArea() {
        currencyTotal = player.getTotalCurrency();
        currencyIcon = new Rect(Screen.width*0.85f, Screen.height-barSize*2, barSize, barSize);
        currencyArea = new Rect(currencyIcon.x+currencyIcon.width, currencyIcon.y, Screen.width*0.1f, currencyIcon.height);
    }

    void drawCurrencyArea() {
        GUI.DrawTexture(currencyIcon, currency);
        GUI.DrawTexture(currencyArea, border);
        GUI.Label(new Rect(currencyArea.x, currencyArea.y, currencyArea.width*0.95f, currencyArea.height), ""+currencyTotal, currencyStyle);
    }
}
