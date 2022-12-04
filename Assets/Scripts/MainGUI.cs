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
    int currentItem;
    Rect itemArea;
    List<Item> items;
    List<Texture2D> itemPics;
    GUIStyle itemStyle;

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

    GUIStyle menuStyle;
    Rect restMenuArea;
    Rect restMenuTop;
    List<Rect> tabAreas;
    Texture2D background;
    int restTab;
    List<string> restTabs;
    List<string> statNames;
    List<int> playerStats;
    List<int> statDeltas;
    int levelCost;
    bool discover;
    GUIStyle discoverStyle;
    List<string> areas;
    List<string> locations;
    string currentArea;
    LocationHandler locationHandler;

    bool bossKill;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("HeroKnight").GetComponent<PlayerCharacter>();
        game = GameObject.Find("GameLogic").GetComponent<GameLogic>();
        locationHandler = GameObject.Find("GameLogic").GetComponent<LocationHandler>();
        areas = locationHandler.getAreas();
        currentArea = areas[0];

        border = Resources.Load("border") as Texture2D;
        health = Resources.Load("health") as Texture2D;
        stamina = Resources.Load("stamina") as Texture2D;
        mana = Resources.Load("mana") as Texture2D;
        currency = Resources.Load("currency") as Texture2D;
        background = Resources.Load("background") as Texture2D;

        currencyStyle = new GUIStyle();
        currencyStyle.richText = true;
        currencyStyle.normal.textColor = Color.white;
        currencyStyle.alignment = TextAnchor.MiddleRight;
        currencyStyle.font = customFont;
        currencyStyle.fontSize = 30;

        itemStyle = new GUIStyle();
        itemStyle.richText = true;
        itemStyle.normal.textColor = Color.white;
        itemStyle.alignment = TextAnchor.LowerRight;
        itemStyle.font = customFont;
        itemStyle.fontSize = 30;

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

        menuStyle = new GUIStyle();
        menuStyle.richText = true;
        menuStyle.normal.textColor = Color.white;
        menuStyle.font = customFont;
        menuStyle.fontSize = 50;

        discoverStyle = new GUIStyle();
        discoverStyle.richText = true;
        discoverStyle.normal.textColor = Color.yellow;
        discoverStyle.alignment = TextAnchor.MiddleCenter;
        discoverStyle.font = customFont;
        discoverStyle.fontSize = 100;

        statNames = new List<string>();
        statNames.Add("level");
        statNames.Add("vitality");
        statNames.Add("strength");
        statNames.Add("endurance");
        statNames.Add("faith");
        statNames.Add("cost");
    }

    // Update is called once per frame
    void Update()
    {
        barSize = Screen.height*0.03f;
        updateStatusBars();
        updateTypingArea();
        updateCurrencyArea();
        updateInventory();
        updateRestMenu();
    }

    void OnGUI() {
        drawStatusBars();
        drawCurrencyArea();
        drawInventory();
        drawRestMenu();
        drawTyping();
        drawBossKill();
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
        //GUI.DrawTexture(currentMana, mana);
        
        GUI.DrawTexture(healthBar, border);
        GUI.DrawTexture(staminaBar, border);
        //GUI.DrawTexture(manaBar, border);
    }

    void updateTypingArea() {
        typing = game.getTyping();
        typingArea = new Rect(Screen.width/4f, staminaBar.y, Screen.width/2, staminaBar.height);
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

    void updateInventory() {
        if(items == null) {
            items = player.getItems();
        }

        itemPics = new List<Texture2D>();
        for(int i=0; i<items.Count; i++) {
            itemPics.Add(items[i].getTexture());
        }

        currentItem = player.getCurrentItem();
        itemArea = new Rect(healthBar.x, Screen.height-barSize*3, barSize*2, barSize*2);
    }

    void drawInventory() {
        GUI.DrawTexture(itemArea, itemPics[currentItem]);
        GUI.Label(itemArea, items[currentItem].getCharges()+"", itemStyle);
        
        int i = currentItem+1;
        if(i > items.Count-1) i = 0;
        Rect temp = new Rect(itemArea.x+barSize*2, itemArea.y, barSize, barSize);
        while(i != currentItem) {
            GUI.DrawTexture(temp, itemPics[i]);
            temp = new Rect(temp.x+barSize, temp.y, temp.width, temp.height);
            
            i++;
            if(i > items.Count-1) i = 0;
        }
    }

    private void updateRestMenu() {
        if(game.playerResting()) {
            playerStats = player.getStats();
            
            float width = Screen.height*0.8f;
            restMenuArea = new Rect((Screen.width-width)/2, Screen.height*0.1f, width, width);
            restMenuTop = new Rect(restMenuArea.x, restMenuArea.y, width*0.98f, width*0.05f);

            restTab = game.getCurrentTab();
            restTabs = game.getRestTabs();
            tabAreas = new List<Rect>();
            float tabWidth = width*0.2f;
            for(int i=0; i<restTabs.Count; i++) {
                tabAreas.Add(new Rect(restMenuTop.x+i*tabWidth, restMenuTop.y, tabWidth, restMenuTop.height));
            }

            if(restTab == 0) {
                updateStatsTab();
            }
            else {
                if(locations == null) {
                    locations = locationHandler.getLocations(currentArea);
                }
                updateTravelTab();
            }
        }
        else {
            levelCost = 0;
            statDeltas = new List<int>();
            for(int i=0; i<statNames.Count; i++) {
                statDeltas.Add(0);
            }
            locations = null;
        }
    }

    private void updateStatsTab() {
        float col1Width = 225f;
        float colHeight = 75f;
        Rect col1 = new Rect(restMenuArea.x+restMenuArea.width*0.05f, restMenuTop.y+restMenuArea.width*0.05f, col1Width, colHeight);
        Rect col2 = new Rect(col1.x+col1Width, col1.y, colHeight, colHeight);
        Rect col3 = new Rect(col2.x+colHeight, col2.y, colHeight, colHeight);
        Rect col4 = new Rect(col3.x+colHeight, col3.y, colHeight, colHeight);

        if(Input.GetMouseButtonUp(0)) {
            Vector3 mousePos = Input.mousePosition;

            for(int i=0; i<statNames.Count; i++) {
                col2 = new Rect(col2.x, col2.y+colHeight, colHeight, colHeight);
                col4 = new Rect(col4.x, col4.y+colHeight, colHeight, colHeight);

                if(i>0 && i<statNames.Count-2) {
                    if(checkMouse(mousePos, col2) && statDeltas[i]>0) {
                        levelCost -= player.getLevelCost(statDeltas[0]);
                        statDeltas[statDeltas.Count-1] = levelCost;
                        statDeltas[0]--;
                        statDeltas[i]--;
                    }

                    if(checkMouse(mousePos, col4)) {
                        statDeltas[0]++;
                        statDeltas[i]++;
                        levelCost += player.getLevelCost(statDeltas[0]);
                        statDeltas[statDeltas.Count-1] = levelCost;
                    }
                }
            }
        }

        if(game.getSubmitFlag()) {
            submitLevelUp();
            game.toggleSubmitFlag();
        }
    }

    public void submitLevelUp() {
        if(levelCost <= player.getTotalCurrency()) {
            player.levelUp(statDeltas, levelCost);
            levelCost = 0;
            statDeltas = new List<int>();
            for(int i=0; i<statNames.Count; i++) {
                statDeltas.Add(0);
            }
        }
    }

    private void updateTravelTab() {
        float colWidth = 225f;
        float colHeight = 75f;
        Rect col1 = new Rect(restMenuArea.x+restMenuArea.width*0.05f, restMenuTop.y+restMenuArea.width*0.05f, colWidth, colHeight);
        Rect col2 = new Rect(col1.x+colWidth, col1.y, colWidth, colHeight);

        if(Input.GetMouseButtonUp(0)) {
            Vector3 mousePos = Input.mousePosition;

            for(int i=0; i<areas.Count; i++) {
                col1 = new Rect(col1.x, col1.y+colHeight, colWidth, colHeight);
                if(checkMouse(mousePos, col1)) {
                    currentArea = areas[i];
                    locations = locationHandler.getLocations(currentArea);
                }
            }

            for(int i=0; i<locations.Count; i++) {
                col2 = new Rect(col2.x, col2.y+colHeight, colWidth, colHeight);
                if(checkMouse(mousePos, col2)) {
                    locationHandler.goTo(currentArea, locations[i]);
                    game.toggleResting();
                }
            }
        }
    }
    
    private void drawRestMenu() {
        if(discover) {
            GUI.Label(new Rect(0, 0, Screen.width, Screen.height*0.9f), "SHRINE DISCOVERED", discoverStyle);
        }
        else if(game.playerResting()) {
            GUI.DrawTexture(restMenuArea, background);
            GUI.Label(restMenuTop, game.colorWord("exit"), currencyStyle);

            try {
                for(int i=0; i<restTabs.Count; i++) {
                    if(i != restTab) GUI.DrawTexture(tabAreas[i], background);
                    GUI.Label(tabAreas[i], game.colorWord(restTabs[i]), borderStyle);
                }

                if(restTab == 0) {
                    drawStatsTab();
                }
                else {
                    drawTravelTab();
                }
            }
            catch {
                //do nothing
            }
        }
    }

    public void discoverTrigger() {
        discover = true;
        Invoke("toggleDiscover", 1.5f);
    }

    private void toggleDiscover() {
        discover = !discover;
    }

    private void drawStatsTab() {
        float col1Width = 225f;
        float colHeight = 75f;
        Rect col1 = new Rect(restMenuArea.x+restMenuArea.width*0.05f, restMenuTop.y+restMenuArea.width*0.05f, col1Width, colHeight);
        Rect col2 = new Rect(col1.x+col1Width, col1.y, colHeight, colHeight);
        Rect col3 = new Rect(col2.x+colHeight, col2.y, colHeight, colHeight);
        Rect col4 = new Rect(col3.x+colHeight, col3.y, colHeight, colHeight);

        string stat = "";
        for(int i=0; i<statNames.Count; i++) {
            col1 = new Rect(col1.x, col1.y+colHeight, col1Width, colHeight);
            GUI.Label(col1, statNames[i], menuStyle);

            col3 = new Rect(col3.x, col3.y+colHeight, colHeight, colHeight);
            if(i == statDeltas.Count-1) {
                if(levelCost > 0) {
                    if(levelCost<=player.getTotalCurrency())
                        stat = "<color=green>"+(statDeltas[i])+"</color>";
                    else stat = "<color=red>"+(statDeltas[i])+"</color>";
                }
                else stat = "" + statDeltas[i];
            }
            else if(statDeltas[i] > 0) {
                if(levelCost<=player.getTotalCurrency())
                    stat = "<color=green>"+(playerStats[i]+statDeltas[i])+"</color>";
                else stat = "<color=red>"+(playerStats[i]+statDeltas[i])+"</color>";
            }
            else stat = "" + playerStats[i];
            GUI.Label(col3, stat, menuStyle);

            col2 = new Rect(col2.x, col2.y+colHeight, colHeight, colHeight);
            col4 = new Rect(col4.x, col4.y+colHeight, colHeight, colHeight);
            if(i>0 && i<statNames.Count-2) {
                GUI.Label(col2, "-", menuStyle);
                GUI.Label(col4, "+", menuStyle);
            }
        }

        Rect submit = new Rect(restMenuTop.x, restMenuTop.y+restMenuArea.height*0.9f, restMenuTop.width, restMenuTop.height);
        if(levelCost>0 && levelCost<=player.getTotalCurrency())
            GUI.Label(submit, game.colorWord("submit"), currencyStyle);
        else GUI.Label(submit, "<color=grey>submit</color>", currencyStyle);
    }

    private void drawTravelTab() {
        float colWidth = 225f;
        float colHeight = 75f;
        Rect col1 = new Rect(restMenuArea.x+restMenuArea.width*0.05f, restMenuTop.y+restMenuArea.width*0.05f, colWidth, colHeight);
        Rect col2 = new Rect(col1.x+colWidth, col1.y, colWidth, colHeight);

        for(int i=0; i<areas.Count; i++) {
            col1 = new Rect(col1.x, col1.y+colHeight, colWidth, colHeight);
            if(areas[i] != currentArea) GUI.DrawTexture(col1, background);
            GUI.Label(col1, areas[i], menuStyle);
        }

        for(int i=0; i<locations.Count; i++) {
            col2 = new Rect(col2.x, col2.y+colHeight, colWidth, colHeight);
            GUI.Label(col2, locations[i], menuStyle);
        }
    }

    private void drawBossKill() {
        if(bossKill)
            GUI.Label(new Rect(0, 0, Screen.width, Screen.height*0.9f), "ENEMY DELETED", discoverStyle);
    }

    public void bossKillTrigger() {
        bossKill = true;
        Invoke("toggleBossKill", 2.5f);
    }

    private void toggleBossKill() {
        bossKill = !bossKill;
    }

    //check button areas for click
	bool checkMouse(Vector3 mousePos, Rect area) {
		return mousePos.y < Screen.height-area.y && mousePos.y > Screen.height-(area.y + area.height) && mousePos.x > area.x && mousePos.x < area.x + area.width;
	}
}
