using UnityEngine;
using System.Collections;
using System;

/**
* GameTimeScript
* @author Ryan
*
* Get the current game time
*/

public class GameTimeScript : MonoBehaviour {

    /**
    * Get the current hour of the game time (HH:mm)
    * @return int hour
    */
	public int getHour() {
        return current_hour;

    }

    /**
    * Get the current minute of the game time (HH:mm)
    * @return int minute
    */
    public int getMinute() {
        return current_min;
    }
    
    public int getSecond() {
        return current_sec;
    }
    void Update() {
        //var total_game_sec = Time.time * GAME_SEC_PER_SEC;
        int total_game_sec = ((((DateTime.Now.Hour*60)+DateTime.Now.Minute)*60)+DateTime.Now.Second) * GAME_SEC_PER_SEC;
        current_hour = (total_game_sec / SEC_PER_HOUR) % 24;
        current_min = (total_game_sec / SEC_PER_MIN) % 60;
        current_sec = (total_game_sec) % 60;

        // Uncomment to show game time each frame
        //Debug.Log(string.Format("{0}:{1} -- {2}", current_hour, current_min, total_game_sec));
    }

    private int current_hour = 0;
    private int current_min = 0;
    private int current_sec = 0;

    private static int GAME_SEC_PER_SEC = 5;
    private static int SEC_PER_HOUR = 60 * 60;
    private static int SEC_PER_MIN = 60;
}
