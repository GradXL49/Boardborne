using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //variables
    protected PlayerCharacter player;
    [SerializeField] protected  int health;
    protected int maxHealth;
    [SerializeField] protected  int damage;
    [SerializeField] protected int reward;
    public float timeFactor;
    protected int startReward;

    [SerializeField] protected Animator m_animator;
    Vector3 start;
    [SerializeField] protected CircleCollider2D collider;

    MainGUI gui;
    [SerializeField] bool boss;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("HeroKnight").GetComponent<PlayerCharacter>();
        gui =  GameObject.Find("GameLogic").GetComponent<MainGUI>();
        start = transform.position;

        maxHealth = health;
        startReward = reward;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void takeDamage(int amount) {
        health -= amount;
        if(health <= 0) {
            die();
            m_animator.SetTrigger("Death");
        }
        else m_animator.SetTrigger("Hurt");
    }

    public void dealDamage() {
        m_animator.SetTrigger("Attack");
        player.takeDamage(damage);
        if(player.isDead()) reward = startReward + player.punish();
    }

    protected void die() {
        player.getReward(reward);
        if(collider != null) InvokeRepeating("fall", 0, 0.1f);
        if(boss) gui.bossKillTrigger();
    }

    private void fall() {
        List<Collider2D> collisions = new List<Collider2D>();
        collider.OverlapCollider(new ContactFilter2D(), collisions);
        if(collisions.Count > 0) {
            CancelInvoke();
            return;
        }

        transform.SetPositionAndRotation(new Vector3(transform.position.x, transform.position.y-0.1f, transform.position.x), transform.rotation);
    }

    public bool isDead() {
        return health <= 0;
    }

    public void resurrect() {
        if(isDead()) {
            m_animator.SetTrigger("Idle");
            transform.SetPositionAndRotation(start, transform.rotation);
        }
        health = maxHealth;
    }

    public Vector2 getHealthValues() {
        return new Vector2(health, maxHealth);
    }
}
