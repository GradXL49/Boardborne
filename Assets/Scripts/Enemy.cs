using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //variables
    PlayerCharacter player;
    [SerializeField] int health;
    int maxHealth;
    [SerializeField] int damage;
    [SerializeField] int reward;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("HeroKnight").GetComponent<PlayerCharacter>();

        maxHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void takeDamage(int amount) {
        health -= amount;
        if(health <= 0) die();
    }

    public void dealDamage() {
        player.takeDamage(damage);
    }

    private void die() {
        player.getReward(reward);
        transform.SetPositionAndRotation(new Vector3(transform.position.x, transform.position.y-100, transform.position.z), transform.rotation);
    }

    public bool isDead() {
        return health <= 0;
    }

    public void resurrect() {
        if(isDead()) {
            transform.SetPositionAndRotation(new Vector3(transform.position.x, transform.position.y+100, transform.position.z), transform.rotation);
        }
        health = maxHealth;
    }
}
