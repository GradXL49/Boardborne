using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthFlask : Item
{
    public HealthFlask(PlayerCharacter p, int c, Texture2D t, Texture2D e) : base(p, c, t, e) { }
    
    public override void use() {
        if(charges > 0) {
            player.gainHealth(-1);
            take(1);
        }
    }
}
