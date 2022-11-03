using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLocation : Location
{
    [SerializeField] float delta;
    public Enemy enemy;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Rect getCombatArea() {
        Vector3 enemyLocation = cam.WorldToScreenPoint(enemy.transform.position);
        return new Rect(enemyLocation.x-10, Screen.height-(enemyLocation.y+delta+10), 20, 20);
    }
}
