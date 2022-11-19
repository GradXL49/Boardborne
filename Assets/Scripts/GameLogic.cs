using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    //variables
    PlayerCharacter player;
    WordService wordService;
    [SerializeField] Location currentLocation;
    [SerializeField] string typing;

    GUIStyle textStyle;
    GUIStyle borderStyle;
    GUIStyle deathStyle;
    [SerializeField] Font customFont;

    string combatWord;
    List<string> words;
    int combatLength;
    bool fighting;
    bool playerDied;
    bool showDeathOptions;
    float combatTimerLength;
    float combatTime;
    Texture2D timer;
    Texture2D health;

    bool resting;
    int restTab;
    List<string> restTabs;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("HeroKnight").GetComponent<PlayerCharacter>();
        wordService = GameObject.Find("GameLogic").GetComponent<WordService>();
        typing = "";
        combatWord = null;
        playerDied = false;
        showDeathOptions = false;
        combatTimerLength = 0;
        timer = Resources.Load("stamina") as Texture2D;
        health = Resources.Load("health") as Texture2D;

        textStyle = new GUIStyle();
        textStyle.richText = true;
        textStyle.normal.textColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        textStyle.alignment = TextAnchor.MiddleCenter;
        textStyle.font = customFont;

        borderStyle = new GUIStyle();
        borderStyle.richText = true;
        borderStyle.normal.textColor = Color.white;
        borderStyle.alignment = TextAnchor.MiddleCenter;
        borderStyle.font = customFont;

        deathStyle = new GUIStyle();
        deathStyle.richText = true;
        deathStyle.normal.textColor = Color.red;
        deathStyle.alignment = TextAnchor.MiddleCenter;
        deathStyle.font = customFont;
        deathStyle.fontSize = 100;

        restTabs = new List<string>();
        restTabs.Add("stats");
        restTabs.Add("travel");

        setLocation(currentLocation);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift)) {
            player.switchItem();
        }
        else if(Input.GetKeyDown(KeyCode.Tab)) {
            player.useEquiped();
        }
        else if(!fighting && Input.anyKeyDown) {
            if(Input.GetKeyDown(KeyCode.Backspace)) {
                if(combatWord == null) typing = typing.Substring(0, typing.Length-1);
            }
            else {
                typing += Input.inputString;
            }
        }
        
        if(playerDied && showDeathOptions) {
            if(typing == "respawn") {
                typing = "";
                playerDied = false;
                showDeathOptions = false;
                player.reset();
                setLocation(player.lastRestPlace);
                resurrectEnemies(currentLocation, new List<Location>());
            }
        }
        else {
            if(currentLocation.GetType()==typeof(EnemyLocation) && !((EnemyLocation)currentLocation).enemy.isDead() && !player.isDead()) {
                combat();
            }
            else {
                if(currentLocation.GetType() == typeof(RestPlace)) {
                    rest();
                }

                for(int i=0; i<currentLocation.childWords.Count; i++) {
                    if(typing == currentLocation.childWords[i]) {
                        player.moveTo(currentLocation.children[i].getLocation().x);
                        typing = "";
                        setLocation(currentLocation.children[i]);
                    }
                }
            }
        }
    }

    //update gui
    void OnGUI() {
        if(playerDied) {
            drawDeathScreen();
            if(showDeathOptions) drawDeathOptions();
        }
        else if(resting) {
            //handled by MainGUI
        }
        else if(currentLocation.GetType()==typeof(EnemyLocation) && !((EnemyLocation)currentLocation).enemy.isDead()) {
            drawCombat();
        }
        else if(!player.isMoving()) {
            drawRestPlace();
            drawAllLocations();
        }
    }

    //set location and check for child words
    public void setLocation(Location l) {
        if(!l.hasChildWords()) {
            l.setChildWords(wordService.getWords(l.children.Count));
        }

        currentLocation = l;
    }

    //rest place logic
    private void rest() {
        if(resting) {
            if(typing == "exit") {
                resting = false;
                typing = "";
            }
            else {
                for(int i=0; i<restTabs.Count; i++) {
                    if(typing == restTabs[i]) {
                        restTab = i;
                        typing = "";
                    }
                }

                if(restTab == 0) {
                    
                }
                else {

                }
            }
        }
        else if(typing == "rest") {
            resting = true;
            restTab = 0;
            player.rest((RestPlace)currentLocation);
            resurrectEnemies(currentLocation, new List<Location>());
            typing = "";
        }
    }

    private void resurrectEnemies(Location current, List<Location> visited) {
        visited.Add(current);
        
        if(current.GetType() == typeof(EnemyLocation)) {
            ((EnemyLocation)current).enemy.resurrect();
        }

        for(int i=0; i<current.children.Count; i++) {
            if(!visited.Contains(current.children[i])) {
                resurrectEnemies(current.children[i], visited);
            }
        }
    }

    public bool playerResting() {
        return resting;
    }

    public List<string> getRestTabs() {
        return restTabs;
    }

    public int getCurrentTab() {
        return restTab;
    }

    //combat logic
    private void combat() {
        if(fighting == false) {
            if(combatWord == null) {
                getCombatWord();
                combatTimerLength = combatWord.Length * ((EnemyLocation)currentLocation).enemy.timeFactor;
                combatTime = combatTimerLength;
                InvokeRepeating("combatTimer", 0, 0.1f);
            }

            if(Input.GetKeyDown(KeyCode.Backspace) && player.dodge())
                typing = typing.Substring(0, typing.Length-1);

            if(typing.Length == combatWord.Length || combatTime <= 0) {
                CancelInvoke();
                combatTimerLength = 0;
                words = new List<string>();
                words.Add(typing);
                words.Add(combatWord);
                combatLength = combatWord.Length+1;
                typing = "";
                combatWord = null;
                fighting = true;
                InvokeRepeating("fight", 0, 0.75f);
            }
        }
    }

    private void fight() {
        combatLength--;
        if(combatLength == 0 || ((EnemyLocation)currentLocation).enemy.isDead() || player.isDead()) {
            fighting = false;
            CancelInvoke();
            return;
        }

        int i = words[1].Length - combatLength;
        
        if(words[0].Length>i && words[0].Substring(i, 1) == words[1].Substring(i, 1)) {
            player.attack(((EnemyLocation)currentLocation).enemy);
        }
        else {
            ((EnemyLocation)currentLocation).enemy.dealDamage();
        }
    }

    void getCombatWord() {
        combatWord = wordService.getWords(1)[0];
    }

    void drawCombat() {
        if(combatTimerLength > 0) drawCombatTimer();
        
        if(combatWord != null) {
            drawLocation(((EnemyLocation)currentLocation).getCombatArea(), combatWord);
        }

        drawEnemyHealth();
    }

    public void deathTrigger() {
        playerDied = true;
        combatWord = null;
        Invoke("triggerDeathOptions", 1f);
    }

    void triggerDeathOptions() {
        showDeathOptions = true;
    }

    void drawDeathScreen() {
        GUI.Label(new Rect(0, 0, Screen.width, Screen.height*0.9f), "YOU MISTYPED", deathStyle);
    }

    void drawDeathOptions() {
        drawLocation(new Rect(0, Screen.height*0.1f, Screen.width, Screen.height*0.9f), "respawn");
    }

    void drawCombatTimer() {
        Rect timerArea = ((EnemyLocation)currentLocation).getCombatArea();
        timerArea.y -= 20;
        timerArea.x -= 40;
        timerArea.height = 10;
        timerArea.width = 100 * (combatTime/combatTimerLength);
        GUI.DrawTexture(timerArea, timer);
    }

    void drawEnemyHealth() {
        Rect eHealthArea = ((EnemyLocation)currentLocation).getHealthArea();
        Vector2 healthValues = ((EnemyLocation)currentLocation).enemy.getHealthValues();
        eHealthArea.y -= 20;
        eHealthArea.x -= 40;
        eHealthArea.height = 10;
        eHealthArea.width = 100 * (healthValues.x/healthValues.y);
        GUI.DrawTexture(eHealthArea, health);
    }

    void combatTimer() {
        combatTime -= 0.1f;
    }

    //get what's been entered
    public string getTyping() {
        return typing;
    }

    //draw location text
    void drawAllLocations() {
        Vector3 child;
        Rect temp;
        
        for(int i=0; i<currentLocation.children.Count; i++) {
            child = currentLocation.children[i].getTextLocation();
            temp = new Rect(child.x-10, Screen.height-(child.y+10), 20, 20);
            
            if(typing.Length>0 && currentLocation.childWords[i].StartsWith(typing.Substring(0, 1))) drawLocation(temp, currentLocation.childWords[i]);
            else {
                GUI.Label(temp, currentLocation.childWords[i], textStyle);
                GUI.Label(new Rect(temp.x-4, temp.y-4, temp.width, temp.height), currentLocation.childWords[i], borderStyle);
            }
        }
    }

    void drawLocation(Rect area, string word) {
        string display = colorWord(word);

        GUI.Label(area, display, textStyle);
        GUI.Label(new Rect(area.x-4, area.y-4, area.width, area.height), word, borderStyle);
    }

    public string colorWord(string word) {
        string display = "";
        
        for(int i=0; i<word.Length; i++) {
            if(typing.Length > i) {
                if(word.Substring(i, 1) == typing.Substring(i, 1))
                    display += "<color=green>"+word.Substring(i, 1)+"</color>";
                else display += "<color=red>"+word.Substring(i, 1)+"</color>";
            }
            else {
                display += word.Substring(i, 1);
            }
        }

        return display;
    }

    void drawRestPlace() {
        if(currentLocation.GetType() == typeof(RestPlace)) {
            Vector3 statue = ((RestPlace)currentLocation).getStatueLocation();
            Rect temp = new Rect(statue.x-10, Screen.height-(statue.y+90), 20, 20);

            drawLocation(temp, "rest");
        }
    }
}
