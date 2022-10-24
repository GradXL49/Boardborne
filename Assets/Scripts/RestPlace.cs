using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestPlace : Location
{
    [SerializeField] Transform statue;
    PlayerCharacter player;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("HeroKnight").GetComponent<PlayerCharacter>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 getStatueLocation() {
        return cam.WorldToScreenPoint(statue.position);
    }
}
