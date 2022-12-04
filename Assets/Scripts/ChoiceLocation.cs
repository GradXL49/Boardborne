using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceLocation : Location
{
    [SerializeField] string activateTag;
    [SerializeField] string deactivateTag;
    
    GameObject[] activate;
    GameObject[] deactivate;
    bool enabled;
    
    // Start is called before the first frame update
    void Start()
    {
        activate = GameObject.FindGameObjectsWithTag(activateTag);
        deactivate = GameObject.FindGameObjectsWithTag(deactivateTag);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void triggerActivation() {
        enabled = true;
        
        foreach(GameObject g in activate) {
            g.GetComponent<EdgeCollider2D>().enabled = true;
        }

        foreach(GameObject g in deactivate) {
            g.GetComponent<EdgeCollider2D>().enabled = false;
        }
    }

    public void reset() {
        if(enabled) {
            enabled = false;

            foreach(GameObject g in activate) {
                g.GetComponent<EdgeCollider2D>().enabled = true;
            }

            foreach(GameObject g in deactivate) {
                g.GetComponent<EdgeCollider2D>().enabled = true;
            }
        }
    }
}
