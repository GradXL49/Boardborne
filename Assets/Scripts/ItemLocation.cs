using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemLocation : Location
{
    bool taken;
    PlayerCharacter player;

    [SerializeField] string itemType;
    [SerializeField] int quantity;
    [SerializeField] GameObject item;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("HeroKnight").GetComponent<PlayerCharacter>();
        
        taken = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool isTaken() {
        return taken;
    }

    public void take() {
        taken = true;
        player.addItem(itemType, quantity);
        item.SetActive(false);
    }

    public Vector3 getItemLocation() {
        return cam.WorldToScreenPoint(item.transform.position);
    }
}
