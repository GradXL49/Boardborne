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

    //check if the location has child words
    public bool hasChildWords() {
        return (childWords!=null && childWords.Count>0);
    }

    //receive new child words
    public void setChildWords(List<string> words) {
        childWords = new List<string>();
        for(int i=0; i<words.Count; i++) {
            childWords.Add(words[i]);
        }
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
