using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location : MonoBehaviour
{
    //variables
    [SerializeField] protected Camera cam;
    public List<Location> children;
    public List<string> childWords;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //get location for player to move to
    public Vector2 getLocation() {
        return new Vector2(transform.position.x, transform.position.y);
    }

    //get text location
    public Vector3 getTextLocation() {
        return cam.WorldToScreenPoint(transform.position);
    }
}
