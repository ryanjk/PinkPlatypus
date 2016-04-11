using UnityEngine;
using System.Collections;

public class MerchantPlacer : MonoBehaviour {

    public GameObject merchant_prefab;

	// Use this for initialization
	void Start () {
        time = FindObjectOfType<GameTimeScript>();
        tile_map = FindObjectOfType<TileMapScript>();
        schedule = FindObjectOfType<ScheduleScript>();
        path_finder = gameObject.AddComponent<WorldGenerator>();

        schedule.loadSchedule("schedule_data.bin");
	}
	
	// Update is called once per frame
	void Update () {
        var merchant = GameObject.FindGameObjectWithTag("Merchant");
        if (merchant == null) {
            var currently_in = schedule.getLowerEntry(time.getHour(), time.getMinute());
            if (currently_in.world_id == tile_map.get_map_id()) {
                // spawn merchant, he should be here
                var going_to = schedule.getUpperEntry(time.getHour(), time.getMinute());
                var cur_min = time_to_min(time.getHour(), time.getMinute());
                var currently_in_min = time_to_min(currently_in.hour, currently_in.minute);
                var going_to_min = time_to_min(going_to.hour, going_to.minute);

                var path = path_finder.get_path(new int[] { currently_in.x_pos, currently_in.y_pos }, new int[] { going_to.x_pos, going_to.y_pos }, tile_map.get_raw_data(), false);
                var percent_along_path = ((float)(cur_min - currently_in_min)) / (going_to_min - currently_in_min);
                var path_pos = (int)(percent_along_path * path.Count);
                path_pos = path_pos >= path.Count ? path.Count - 1 : path_pos;
                var spawn_point_2d = path[path_pos];
                var spawn_point = new Vector3(spawn_point_2d.y, 1.0f, spawn_point_2d.x);
                merchant = Instantiate(merchant_prefab, spawn_point, Quaternion.identity) as GameObject;
                Debug.Log("no merchant, adding him in");
            }
        }
        else if (!is_moving) {
            Debug.Log("should merchant still be here?");
            // merchant is here, should he still be?
            var should_be_in = schedule.getLowerEntry(time.getHour(), time.getMinute());
            if (should_be_in.world_id != tile_map.get_map_id()) {
                Destroy(merchant);
                Debug.Log("no");
            }
        }

        // see if merchant is still here
        merchant = GameObject.FindGameObjectWithTag("Merchant");
        if (merchant != null && !is_moving) {
            // move it towards its path
            var coming_from = schedule.getLowerEntry(time.getHour(), time.getMinute());
            var destination = schedule.getUpperEntry(time.getHour(), time.getMinute());

            if (coming_from.world_id == destination.world_id && 
                coming_from.x_pos == destination.x_pos && 
                coming_from.y_pos == destination.y_pos) {
                // don't move, in waiting position
                Debug.Log("merchant is waiting");
                return;
            }

            var merchant_grid_pos = new int[] { (int) merchant.transform.position.x, (int) merchant.transform.position.z };
            var next_move = path_finder.get_path(merchant_grid_pos, new int[] { destination.x_pos, destination.y_pos }, tile_map.get_raw_data(), false)[1];
            iTween.MoveTo(merchant, iTween.Hash(
                "position", new Vector3( (float) next_move.y, 1.0f, (float) next_move.x),
                "name", "merchant_move_tween",
                "time", 0.1f,
                "oncomplete", "on_tween_complete",
                "easetype", iTween.EaseType.linear
            ));
            is_moving = true;
            Debug.Log(string.Format("Moving from ({0}, {1}) to ({2}, {3})", merchant_grid_pos[0], merchant_grid_pos[1], next_move.y, next_move.x));
        }
    }

    public void on_tween_complete() {
        is_moving = false;
    }

    private int time_to_min(int hour, int min) {
        return hour * 60 + min;
    }

    private bool is_moving = false;
    private ScheduleScript schedule;
    private TileMapScript tile_map;
    private GameTimeScript time;
    private WorldGenerator path_finder;
}
