using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    protected PlayerCharacter player;
    protected int charges;
    protected Texture2D texture;
    protected Texture2D empty;
    
    public Item(PlayerCharacter p, int c, Texture2D t, Texture2D e) {
        player = p;
        charges = c;
        texture = t;
        empty = e;
    }

    public int getCharges() {
        return charges;
    }

    public void give(int x) {
        charges += x;
    }

    public void take(int x) {
        charges -= x;
    }

    public void setCharges(int x) {
        charges = x;
    }

    public Texture2D getTexture() {
        if(charges > 0) return texture;
        else return empty;
    }

    public virtual void use() {}
}
