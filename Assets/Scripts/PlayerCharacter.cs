using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerCharacter : MonoBehaviour {

    [SerializeField] float      m_speed = 4.0f;
    [SerializeField] float      m_rollForce = 6.0f;
    [SerializeField] bool       m_noBlood = false;
    [SerializeField] GameLogic  game;

    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private Sensor_HeroKnight   m_groundSensor;
    private Sensor_HeroKnight   m_groundSensor_l;
    private Sensor_HeroKnight   m_groundSensor_r;
    private bool                m_grounded = false;
    private bool                m_rolling = false;
    private int                 m_facingDirection = 1;
    private int                 m_currentAttack = 0;
    private float               m_timeSinceAttack = 0.0f;
    private float               m_delayToIdle = 0.0f;
    private bool                moving = false;
    private float               target;
    private int                 target_direction;

    private int currentHealth;
    private int maxHealth;
    private int currentStamina;
    private int maxStamina;
    private int currentMana;
    private int maxMana;
    private int currency;
    public RestPlace lastRestPlace;

    private int level;
    private int vitality;
    private int strength;
    private int endurance;
    private int faith;
    private int heal;
    
    List<Item> items;
    private int currentItem;
    private int healthFlasks;
    private int staminaFlasks;


    // Use this for initialization
    void Start ()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        m_groundSensor_l = transform.Find("GroundSensor_L").GetComponent<Sensor_HeroKnight>();
        m_groundSensor_r = transform.Find("GroundSensor_R").GetComponent<Sensor_HeroKnight>();
        game = GameObject.Find("GameLogic").GetComponent<GameLogic>();

        level = 0;
        vitality = 10;
        strength = 5;
        endurance = 5;
        faith = 3;
        
        healthFlasks = 2;
        staminaFlasks = 2;
        items = new List<Item>();
        items.Add(new HealthFlask(this, healthFlasks, Resources.Load("hflask") as Texture2D, Resources.Load("hflask-empty") as Texture2D));
        items.Add(new StaminaFlask(this, staminaFlasks, Resources.Load("sflask") as Texture2D, Resources.Load("sflask-empty") as Texture2D));
        currentItem = 0;

        maxHealth = vitality * 10;
        currentHealth = maxHealth/2;
        maxStamina = endurance * 10;
        currentStamina = maxStamina/2;
        heal = faith*5;
        maxMana = 20;
        currentMana = maxMana;
        currency = 0;
    }

    // Update is called once per frame
    void Update ()
    {
        //Check if character just landed on the ground
        if (!m_grounded && (m_groundSensor.State() || m_groundSensor_l.State() || m_groundSensor_r.State()))
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if (m_grounded && !(m_groundSensor.State() || m_groundSensor_l.State() || m_groundSensor_r.State()))
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // -- Handle input and movement --
        float inputX = 0;
        if(moving) {
            if(target_direction > 0) inputX = 1;
            else inputX = -1;

            if((target_direction>0 && transform.position.x>=target) || (target_direction<0 && transform.position.x<=target)) {
                moving = false;
                inputX = 0;
            }
        }

        // Swap direction of sprite depending on walk direction
        if (inputX > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
        }
            
        else if (inputX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
        }

        // Move
        if (!m_rolling )
            m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);
            
        //Run
        if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            // Reset timer
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }

        //Idle
        else
        {
            // Prevents flickering transitions to idle
            m_delayToIdle -= Time.deltaTime;
                if(m_delayToIdle < 0)
                    m_animator.SetInteger("AnimState", 0);
        }
    }

    //tell the model to move
    public void moveTo(float x) {
        moving = true;
        target = x;
        if(target - transform.position.x > 0) target_direction = 1;
        else target_direction = -1;
    }

    //get whether the character is moving
    public bool isMoving() {
        return moving;
    }

    //get current attribute values
    public Vector3 getStatus() {
        return new Vector3(currentHealth, currentStamina, currentMana);
    }

    public Vector3 getMax() {
        return new Vector3(maxHealth, maxStamina, maxMana);
    }

    //have the player rest
    public void rest(RestPlace location) {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        currentMana = maxMana;
        items[0].setCharges(healthFlasks);
        items[1].setCharges(staminaFlasks);
        lastRestPlace = location;
    }

    //attack the enemy
    public void attack(Enemy enemy) {
        enemy.takeDamage(strength);
        
        m_currentAttack++;

        // Loop back to one after third attack
        if (m_currentAttack > 3)
            m_currentAttack = 1;

        // Call one of three attack animations "Attack1", "Attack2", "Attack3"
        m_animator.SetTrigger("Attack" + m_currentAttack);
    }

    //attempt a dodge
    public bool dodge() {
        if(currentStamina-10 >= 0) {
            currentStamina -= 10;
            return true;
        }
        
        return false;
    }

    //take damage from enemy
    public void takeDamage(int amount) {
        currentHealth -= amount;
        if(currentHealth <= 0) {
            currentHealth = 0;
            m_animator.SetBool("noBlood", m_noBlood);
            m_animator.SetTrigger("Death");
            Invoke("sendDeathTrigger", 1f);
        }
        else m_animator.SetTrigger("Hurt");
    }

    void sendDeathTrigger() {
        game.deathTrigger();
    }

    //get reward from killing enemy
    public void getReward(int amount) {
        currency += amount;
    }

    //get total amount of currency
    public int getTotalCurrency() {
        return currency;
    }

    //check if dead
    public bool isDead() {
        return currentHealth == 0;
    }

    //take away currency
    public int punish() {
        int current = currency;
        currency = 0;
        return current;
    }

    //reset after death
    public void reset() {
        Vector2 respawn = lastRestPlace.getLocation();
        transform.SetPositionAndRotation(new Vector3(respawn.x, respawn.y, transform.position.z), transform.rotation);
        rest(lastRestPlace);
        m_animator.Rebind();
        m_animator.Update(0f);
    }

    //heal
    public void gainHealth(int x) {
        if(x < 0) currentHealth += heal;
        else currentHealth += x;
        if(currentHealth > maxHealth) currentHealth = maxHealth;
    }

    //regain stamina
    public void gainStamina(int x) {
        if(x < 0) currentStamina += heal;
        else currentStamina += x;
        if(currentStamina > maxStamina) currentStamina = maxStamina;
    }

    //use item
    public void useEquiped() {
        if(items[currentItem].getCharges() > 0) {
            m_animator.SetTrigger("Block");
            items[currentItem].use();
        }
    }

    //switch item
    public void switchItem() {
        currentItem++;
        if(currentItem > items.Count-1) currentItem = 0;
    }

    //get equiped
    public int getCurrentItem() {
        return currentItem;
    }

    //get inventory
    public List<Item> getItems() {
        return items;
    }
}
