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
public class NTalk : MonoBehaviour
{
    private Text _text;
    private Image _container;
    private bool is_overworld_npc = false;

    private string message;
    void Start()
    {
        _text = GameObject.FindGameObjectWithTag("Messages").GetComponent<Text>();
        _container = GameObject.FindGameObjectWithTag("MessageBox").GetComponent<Image>();
        _container.enabled = false;
        _text.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "NetworkPlayer") {
            _container.enabled = true;
            _text.enabled = true;
            _text.text = mpDialogue(other.GetComponent<NPlayerController>().isHost());
        }

    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "NetworkPlayer") {
            _container.enabled = false;
            _text.enabled = false;

            _text.text = "";
        }
    }

    public void set_overworld_npc(bool value) {
        is_overworld_npc = value;
    }

	private string merchantSchedule(string time){
        string[] _messages = {"Who? The mysterious creep? I think he will be here around TIME",
                              "Stick around until about TIME, that's when the caravan rolls in",
                              "I hear the merchant stops here at TIME",
                              "TIME is a pretty interesting time of the day for this place",
                              "Need some keys? Wait until TIME",
                              "I can't wait until TIME",
                              "TIME. Remember it"};
        string returnString = _messages[Random.Range(0, _messages.Length)];
        returnString = returnString.Replace("TIME", time);
		return returnString;
	}
    private string mpDialogue(bool isHost) {
        string start = "Hey Blue! ";
        if (isHost) start = "Hey Pink! ";
        string[] _messages = {"I'm the Vybz! I'm the coolest cat around!",
                              "Isn't it fun to explore with friends?",
                              "I heard that merchant is scared of strangers..",
                              "My friend fell into the dungeon. I hope he's alright...",
                              "Have you heard the legend of the great Sir Vybihal?",
                              "Isn't it fun to explore with friends?",
                              "I heard that merchant is scared of strangers..",
                              "My friend fell into the dungeon. I hope he's alright...",
                              "Have you heard the legend of the great Sir Vybihal?",
                              "Isn't it fun to explore with friends?",
                              "I heard that merchant is scared of strangers..",
                              "My friend fell into the dungeon. I hope he's alright...",
                              "Have you heard the legend of the great Sir Vybihal?",
                              "Isn't it fun to explore with friends?",
                              "I heard that merchant is scared of strangers..",
                              "My friend fell into the dungeon. I hope he's alright...",
                              "Have you heard the legend of the great Sir Vybihal?",
                              "Isn't it fun to explore with friends?",
                              "I heard that merchant is scared of strangers..",
                              "My friend fell into the dungeon. I hope he's alright...",
                              "Have you heard the legend of the great Sir Vybihal?",
                              "Isn't it fun to explore with friends?",
                              "I heard that merchant is scared of strangers..",
                              "My friend fell into the dungeon. I hope he's alright...",
                              "Have you heard the legend of the great Sir Vybihal?",
                              "Isn't it fun to explore with friends?",
                              "I heard that merchant is scared of strangers..",
                              "My friend fell into the dungeon. I hope he's alright...",
                              "Have you heard the legend of the great Sir Vybihal?",
                              "Isn't it fun to explore with friends?",
                              "I heard that merchant is scared of strangers..",
                              "My friend fell into the dungeon. I hope he's alright...",
                              "Have you heard the legend of the great Sir Vybihal?",
                              "Isn't it fun to explore with friends?",
                              "I heard that merchant is scared of strangers..",
                              "My friend fell into the dungeon. I hope he's alright...",
                              "Have you heard the legend of the great Sir Vybihal?",
                              "Isn't it fun to explore with friends?",
                              "I heard that merchant is scared of strangers..",
                              "My friend fell into the dungeon. I hope he's alright...",
                              "Have you heard the legend of the great Sir Vybihal?",
                              "Isn't it fun to explore with friends?",
                              "I heard that merchant is scared of strangers..",
                              "My friend fell into the dungeon. I hope he's alright...",
                              "Have you heard the legend of the great Sir Vybihal?",
                              "Isn't it fun to explore with friends?",
                              "I heard that merchant is scared of strangers..",
                              "My friend fell into the dungeon. I hope he's alright...",
                              "Have you heard the legend of the great Sir Vybihal?"
                              };
        string returnString = _messages[Random.Range(0, _messages.Length)];
        return start + returnString;
    }

    private Dictionary<string, string> _conversations;
}