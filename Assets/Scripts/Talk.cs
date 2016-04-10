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
        text.text = "You need the key.";
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" || other.tag == "FPPlayer")
            text.text = "";
    }

	public string merchantSchedule(string time){
		return "Who? The mysterious creep? I think he will be here around" + time;
	}
    private Dictionary<string, string> _conversations;
}