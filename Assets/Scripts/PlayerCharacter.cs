using UnityEngine;
using System.Collections;

public class PlayerCharacter : MonoBehaviour {

    [SerializeField] float      m_speed = 4.0f;
    [SerializeField] float      m_rollForce = 6.0f;
    [SerializeField] bool       m_noBlood = false;

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
    private int strength;
    private int currency;
    private RestPlace lastRestPlace;


    // Use this for initialization
    void Start ()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        m_groundSensor_l = transform.Find("GroundSensor_L").GetComponent<Sensor_HeroKnight>();
        m_groundSensor_r = transform.Find("GroundSensor_R").GetComponent<Sensor_HeroKnight>();

        maxHealth = 100;
        currentHealth = maxHealth/2;
        maxStamina = 40;
        currentStamina = maxStamina/2;
        maxMana = 20;
        currentMana = maxMana/2;
        strength = 5;
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
        // float inputX = Input.GetAxis("Horizontal");
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

        // -- Handle Animations --
        //Death
        // if (Input.GetKeyDown("e") && !m_rolling)
        // {
        //     m_animator.SetBool("noBlood", m_noBlood);
        //     m_animator.SetTrigger("Death");
        // }
            
        //Hurt
        // else if (Input.GetKeyDown("q") && !m_rolling)
        //     m_animator.SetTrigger("Hurt");

        //Attack
        // else if(Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.25f && !m_rolling)
        // {
        //     m_currentAttack++;

        //     // Loop back to one after third attack
        //     if (m_currentAttack > 3)
        //         m_currentAttack = 1;

        //     // Reset Attack combo if time since last attack is too large
        //     if (m_timeSinceAttack > 1.0f)
        //         m_currentAttack = 1;

        //     // Call one of three attack animations "Attack1", "Attack2", "Attack3"
        //     m_animator.SetTrigger("Attack" + m_currentAttack);

        //     // Reset timer
        //     m_timeSinceAttack = 0.0f;
        // }

        // Block
        // else if (Input.GetMouseButtonDown(1) && !m_rolling)
        // {
        //     m_animator.SetTrigger("Block");
        //     m_animator.SetBool("IdleBlock", true);
        // }

        // else if (Input.GetMouseButtonUp(1))
        //     m_animator.SetBool("IdleBlock", false);

        // Roll
        // else if (Input.GetKeyDown("left shift") && !m_rolling)
        // {
        //     m_rolling = true;
        //     m_animator.SetTrigger("Roll");
        //     m_body2d.velocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.velocity.y);
        // }
            
        //Run
        /*else*/ if (Mathf.Abs(inputX) > Mathf.Epsilon)
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

    // Animation Events
    // Called in end of roll animation.
    void AE_ResetRoll()
    {
        m_rolling = false;
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
        lastRestPlace = location;
    }

    //attack the enemy
    public void attack(Enemy enemy) {
        enemy.takeDamage(strength);
    }

    //take damage from enemy
    public void takeDamage(int amount) {
        currentHealth -= amount;
        if(currentHealth <= 0) die();
    }

    private void die() {

    }

    //get reward from killing enemy
    public void getReward(int amount) {
        currency += amount;
    }
}
