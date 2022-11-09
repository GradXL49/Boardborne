using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    //variables
    [SerializeField] PlayerCharacter player;
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
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("HeroKnight").GetComponent<PlayerCharacter>();
        typing = "";
        combatWord = null;
        playerDied = false;
        showDeathOptions = false;
        combatTimerLength = 0;
        timer = Resources.Load("stamina") as Texture2D;

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
                if(combatWord == null || player.isDead()) typing = typing.Substring(0, typing.Length-1);
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
                currentLocation = player.lastRestPlace;
                resurrectEnemies(currentLocation, new List<Location>());
            }
        }
        else {
            if(currentLocation.GetType()==typeof(EnemyLocation) && !((EnemyLocation)currentLocation).enemy.isDead()) {
                combat();
            }
            else {
                if(currentLocation.GetType() == typeof(RestPlace)) {
                    resting();
                }

                for(int i=0; i<currentLocation.childWords.Count; i++) {
                    if(typing == currentLocation.childWords[i]) {
                        player.moveTo(currentLocation.children[i].getLocation().x);
                        typing = "";
                        currentLocation = currentLocation.children[i];
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
        else if(currentLocation.GetType()==typeof(EnemyLocation) && !((EnemyLocation)currentLocation).enemy.isDead()) {
            drawCombat();
            if(combatTimerLength > 0) drawCombatTimer();
        }
        else if(!player.isMoving()) {
            drawRestPlace();
            drawAllLocations();
        }
    }

    //rest place logic
    private void resting() {
        if(typing == "rest") {
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

    //combat logic
    private void combat() {
        if(fighting == false) {
            if(combatWord == null) {
                getCombatWord();
                combatTimerLength = combatWord.Length * 1f;
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
        combatWord = "attack";
    }

    void drawCombat() {
        if(combatWord != null)
            drawLocation(((EnemyLocation)currentLocation).getCombatArea(), combatWord);
    }

    public void deathTrigger() {
        playerDied = true;
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
                GUI.Label(temp, currentLocation.childWords[i], borderStyle);
                GUI.Label(new Rect(temp.x-2, temp.y-2, temp.width, temp.height), currentLocation.childWords[i], textStyle);
            }
        }
    }

    void drawLocation(Rect area, string word) {
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

        GUI.Label(area, display, borderStyle);
        GUI.Label(new Rect(area.x-2, area.y-2, area.width, area.height), word, textStyle);
    }

    void drawRestPlace() {
        if(currentLocation.GetType() == typeof(RestPlace)) {
            Vector3 statue = ((RestPlace)currentLocation).getStatueLocation();
            Rect temp = new Rect(statue.x-10, Screen.height-(statue.y+90), 20, 20);

            drawLocation(temp, "rest");
        }
    }
}
