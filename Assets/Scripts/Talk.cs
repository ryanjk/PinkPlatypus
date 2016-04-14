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
    private bool is_overworld_npc = false;

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
    private Dictionary<string, string> _conversations;
}