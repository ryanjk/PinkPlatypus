using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

/*
 * PlayerMain
 @author Taha Ghassemi
 * 
 *
 *
 */
public class Talk : MonoBehaviour
{
    public Text text;
    void Start()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Player"||other.tag=="FPPlayer")
        text.text = "You need the key to open the door.";
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" || other.tag == "FPPlayer")
            text.text = "";
    }
    private Dictionary<string, string> _conversations;
}