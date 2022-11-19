using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordService : MonoBehaviour
{
    string[] dictionary;
    System.Random rand;
    
    // Start is called before the first frame update
    void Start()
    {
        dictionary = System.IO.File.ReadAllLines("Assets/Dictionary.txt");
        rand = new System.Random();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //return a given number of random words
    public List<string> getWords(int n) {
        List<string> words = new List<string>();
        while(words.Count < n) {
            int i = rand.Next(dictionary.Length);
            if(!words.Contains(dictionary[i]))
                words.Add(dictionary[i]);
        }

        return words;
    }
}
