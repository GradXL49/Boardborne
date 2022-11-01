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
    [SerializeField] Font customFont;

    string combatWord;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("HeroKnight").GetComponent<PlayerCharacter>();
        typing = "";
        combatWord = "attack";

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
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKeyDown) {
            if(Input.GetKeyDown(KeyCode.Backspace)) {
                typing = typing.Substring(0, typing.Length-1);
            }
            else {
                typing += Input.inputString;
            }
        }

        if(currentLocation.GetType() == typeof(RestPlace)) {
            resting();
        }
        else if(currentLocation.GetType() == typeof(EnemyLocation)) {
            combat();
        }
        
        for(int i=0; i<currentLocation.childWords.Count; i++) {
            if(typing == currentLocation.childWords[i]) {
                player.moveTo(currentLocation.children[i].getLocation().x);
                typing = "";
                currentLocation = currentLocation.children[i];
            }
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
        if(typing.Length == combatWord.Length) {
            for(int i=0; i<typing.Length && !((EnemyLocation)currentLocation).enemy.isDead(); i++) {
                if(typing.Substring(i, 1) == combatWord.Substring(i, 1)) {
                    player.attack(((EnemyLocation)currentLocation).enemy);
                }
                else {
                    ((EnemyLocation)currentLocation).enemy.dealDamage();
                }
            }
            typing = "";
        }
    }

    //get what's been entered
    public string getTyping() {
        return typing;
    }

    //update gui
    void OnGUI() {
        if(!player.isMoving()) {
            drawRestPlace();
            drawAllLocations();
        }
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
