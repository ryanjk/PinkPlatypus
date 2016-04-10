using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ClockMain : MonoBehaviour {

    public Text clock;
    public GameTimeScript time;
	// Update is called once per frame
	void Update () {
        string hour = "";
        string minute = "";
        string second = "";
        if (time.getHour() < 10) {
            hour = "0" + time.getHour();
        }
        else {
            hour = "" + time.getHour();
        }
        if (time.getMinute() < 10) {
            minute = "0" + time.getMinute();
        }
        else {
            minute = "" + time.getMinute();
        }
        if (time.getSecond() < 10) {
            second = "0" + time.getSecond();
        }
        else {
            second = "" + time.getSecond();
        }
        clock.text = hour + ":" + minute + ":" + second;
	}
}
