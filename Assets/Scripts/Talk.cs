using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

/*
 * Talk
 @author Taha Ghassemi & Thomas Fix
 * 
 * Class handling dialogue with the player for NPCs
 *
 */
public class Talk : MonoBehaviour
{
    private Text _text;
    private Image _container;
    private bool is_overworld_npc = false;
    public bool is_platypus;

    public string message;
    void Start()
    {
        _text = GameObject.FindGameObjectWithTag("Messages").GetComponent<Text>();
        _container = GameObject.FindGameObjectWithTag("MessageBox").GetComponent<Image>();
        _container.enabled = false;
        _text.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Player"||other.tag=="FPPlayer") {
            _container.enabled = true;
            _text.enabled = true;

            if (is_overworld_npc) {
                var schedule_data = FindObjectOfType<ScheduleScript>();
                var world_info = FindObjectOfType<TileMapScript>();

                var npc_object = gameObject.transform.parent.gameObject;
                var npc_position = new int[] { (int) npc_object.transform.position.x, (int) npc_object.transform.position.z };
                var nearest_entry = schedule_data.getClosestEntry(world_info.get_map_id(), npc_position);
                var minute = nearest_entry.minute;
                var minute_string = minute <= 9 ? string.Format("0{0}", minute) : string.Format("{0}", minute);
                var time_string = string.Format("{0,2}:{1,2}", nearest_entry.hour, minute_string);
                _text.text = merchantSchedule(time_string);
            }
            else if(is_platypus) {
                _text.text = gameProgress();
            }
            else {
                _text.text = message;
            }
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

    public void set_overworld_npc(bool value) {
        is_overworld_npc = value;
    }

    /**
    * This method provides a way to give NPCs random, helpful dialogue about the merchant's schedule
    * given a string representation of the time he's supposed to arrive
    *
    * @param time the time to insert into the string
    * @return a random string from the hard-coded list, which includes time data
    */
	private string merchantSchedule(string time) {
        // didn't need to be super robust, so it's just a local, hard-coded array
        // of strings - TIME is to be replaced by the argument string (see below)
        string[] _messages = {"Who? The mysterious creep? I think he will be here around TIME",
                              "Stick around until about TIME, that's when the caravan rolls in",
                              "I hear the merchant stops here at TIME",
                              "TIME is a pretty interesting time of the day for this place",
                              "Need some keys? Wait until TIME",
                              "I can't wait until TIME",
                              "TIME. Remember it"};
        // pick a random string
        string returnString = _messages[Random.Range(0, _messages.Length)];
        // insert the time
        returnString = returnString.Replace("TIME", time);
		return returnString;
	}

    private string gameProgress() {
        switch ((5 - GameObject.FindGameObjectWithTag("Player").gameObject.GetComponent<PlayerMain>().countItem(1))) {
            case 5:
            case 4:
                return "Thanks for saving me! There are " + (5 - GameObject.FindGameObjectWithTag("Player").gameObject.GetComponent<PlayerMain>().countItem(1)) + " other platypi left to save!";
            case 3:
                return "Thank the heavens you're here! My 3 other friends are out there. I hope they're alright...";
            case 2:
                return "Wow. I thought I was a goner. You're my hero. Only two to go!";
            case 1:
                return "Thank you Pink Platypus! But our last platypus is in another castle";
            case 0:
                return "CONGRATULATIONS! YOU SAVED ALL THE PLATYPI! YOUR QUEST IS NOW COMPLETE.";
            default:
                return "Ummm you're not supposed to see this";
        }

    }

    private Dictionary<string, string> _conversations;
}