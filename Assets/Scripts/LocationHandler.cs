using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationHandler : MonoBehaviour
{
    PlayerCharacter player;
    MainGUI gui;
    List<string> areas;
    List<List<string>> locations;
    List<List<bool>> visited;
    
    [SerializeField] Font customFont;
    GUIStyle discoverStyle;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("HeroKnight").GetComponent<PlayerCharacter>();
        gui = GameObject.Find("GameLogic").GetComponent<MainGUI>();
        
        areas = new List<string>();
        areas.Add("Church");

        locations = new List<List<string>>();
        locations.Add(new List<string>());
        locations[0].Add("Altar");
        locations[0].Add("Basement");
        locations[0].Add("Cathedral");

        visited = new List<List<bool>>();
        visited.Add(new List<bool>());
        visited[0].Add(true);
        for(int i=0; i<areas.Count; i++) {
            if(i > 0) {
                visited.Add(new List<bool>());
                for(int j=0; j<locations[i].Count; j++) {
                    visited[i].Add(false);
                }
            }
            else {
                for(int j=1; j<locations[i].Count; j++) {
                    visited[i].Add(false);
                }
            }
        }

        discoverStyle = new GUIStyle();
        discoverStyle.richText = true;
        discoverStyle.normal.textColor = Color.yellow;
        discoverStyle.alignment = TextAnchor.MiddleCenter;
        discoverStyle.font = customFont;
        discoverStyle.fontSize = 100;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void visit(string area, string location) {
        int areaIndex = area.IndexOf(area);
        int locationIndex = locations[areaIndex].IndexOf(location);
        
        if(!visited[areaIndex][locationIndex]) {
            visited[areaIndex][locationIndex] = true;
            gui.discoverTrigger();
            player.gainFaith();
        }
    }

    public List<string> getAreas() {
        return areas;
    }

    public List<string> getLocations(string area) {
        int areaIndex = areas.IndexOf(area);
        List<string> discovered = new List<string>();

        for(int i=0; i<locations[areaIndex].Count; i++) {
            if(visited[areaIndex][i])
                discovered.Add(locations[areaIndex][i]);
        }

        return discovered;
    }

    public void goTo(string area, string location) {
        GameObject[] respawns = GameObject.FindGameObjectsWithTag("Respawn");

        foreach(GameObject o in respawns) {
            if(o.GetComponent<RestPlace>().name == location) {
                player.lastRestPlace = o.GetComponent<RestPlace>();
                player.reset();
                return;
            }
        }
    }
}
