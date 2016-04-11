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
    private Text _text;
    private Image _container;
    public string message;
    void Start()
    {
        _text = GameObject.FindGameObjectWithTag("Messages").GetComponent<Text>();
        _container = GameObject.FindGameObjectWithTag("MessageBox").GetComponent<Image>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Player"||other.tag=="FPPlayer") {
            _container.enabled = true;
            _text.enabled = true;
            _text.text = message;

        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" || other.tag == "FPPlayer") {
            _container.enabled = false;
            _text.enabled = false;

            _text.text = "";
        }
    }

	public string merchantSchedule(string time){
        string[] _messages = {"Who? The mysterious creep? I think he will be here around TIME",
                              "Stick around until about TIME, that's when the caravan rolls in",
                              "I hear the merchant stops here at TIME",
                              "TIME is a pretty interesting time of the day for this place",
                              "Need some keys? Wait until TIME",
                              "I can't wait until TIME",
                              "TIME. Remember it"};
        Random random = new Random();
        string returnString = _messages[_messages.Length * (int)Random.value - 1];
        returnString.Replace("TIME", time);
		return returnString;
	}
    private Dictionary<string, string> _conversations;
}