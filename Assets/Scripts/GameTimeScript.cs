using UnityEngine;
using System.Collections;

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

    void Update() {
        var total_game_sec = Time.time * GAME_SEC_PER_SEC;
        current_hour = (int) (total_game_sec / SEC_PER_HOUR) % 24;
        current_min = (int) (total_game_sec / SEC_PER_MIN) % 60;

        // Uncomment to show game time each frame
        //Debug.Log(string.Format("{0}:{1} -- {2}", current_hour, current_min, total_game_sec));
    }

    private int current_hour = 0;
    private int current_min = 0;

    private static int GAME_SEC_PER_SEC = 5;
    private static int SEC_PER_HOUR = 60 * 60;
    private static int SEC_PER_MIN = 60;
}
