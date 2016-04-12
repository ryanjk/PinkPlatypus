using UnityEngine;
using System.Collections;

public class MerchantPlacer : MonoBehaviour {

    public GameObject merchant_prefab;
	private Merchant merchant;
	// Use this for initialization
	void Start () {
        time = FindObjectOfType<GameTimeScript>();
        tile_map = FindObjectOfType<TileMapScript>();
        schedule = FindObjectOfType<ScheduleScript>();
        path_finder = gameObject.AddComponent<WorldGenerator>();


        is_moving = false;
        schedule.loadSchedule("schedule_data.bin");
	}
	
	// Update is called once per frame

	void Update () {
        var merchant = GameObject.FindGameObjectWithTag("Merchant");
        bool just_added_merchant = false;
        if (merchant == null) {
            var currently_in = schedule.getLowerEntry(time.getHour(), time.getMinute());
            if (currently_in.world_id == tile_map.get_map_id()) {
                // spawn merchant, he should be here
                var going_to = schedule.getUpperEntry(time.getHour(), time.getMinute());
                var cur_min = time_to_min(time.getHour(), time.getMinute());
                var currently_in_min = time_to_min(currently_in.hour, currently_in.minute);
                var going_to_min = time_to_min(going_to.hour, going_to.minute);

                var spawn_point = new Vector3();
                if (going_to.world_id != currently_in.world_id) { // not on a path, no need to calculate percent along path
                    spawn_point = new Vector3(currently_in.x_pos, 1.0f, currently_in.y_pos);
                }
                else {
                    var path = path_finder.get_path(new int[] { currently_in.x_pos, currently_in.y_pos }, new int[] { going_to.x_pos, going_to.y_pos }, tile_map.get_raw_data(), false);
                    var percent_along_path = ((float)(cur_min - currently_in_min)) / (going_to_min - currently_in_min);
                    var path_pos = (int)(percent_along_path * path.Count);
                    path_pos = path_pos >= path.Count ? path.Count - 1 : path_pos;
                    var spawn_point_2d = path[path_pos];
                    spawn_point = new Vector3(spawn_point_2d.x, 1.0f, spawn_point_2d.y);
                }
                merchant = Instantiate(merchant_prefab, spawn_point, Quaternion.Euler(0.0f, 270.0f, 0.0f)) as GameObject;
                Debug.Log("no merchant, adding him in");
                just_added_merchant = true;
                set_merchant_collider(merchant, false);
            }
        }
        else if (!is_moving) {
            // merchant is here, should he still be?
            var should_be_in = schedule.getLowerEntry(time.getHour(), time.getMinute());
            if (should_be_in.world_id != tile_map.get_map_id()) {
                Destroy(merchant);
                close_merchant_shop();
                Debug.Log("removing merchant from scene");
            }
        }

        if (just_added_merchant || merchant == null) {
            return;
        }

        // if merchant is still here and we didn't just place him in
        if (!is_moving) {
            var coming_from = schedule.getLowerEntry(time.getHour(), time.getMinute());
            var destination = schedule.getUpperEntry(time.getHour(), time.getMinute());

            var same_entries = coming_from.world_id == destination.world_id &&
                coming_from.x_pos == destination.x_pos &&
                coming_from.y_pos == destination.y_pos;
            var different_worlds = coming_from.world_id != destination.world_id;
            if (same_entries || different_worlds) {
                // don't move, in waiting position
                Debug.Log(string.Format("merchant is waiting until {0}:{1}", destination.hour, destination.minute));
                merchant.GetComponent<Merchant>().set_sprite_from_movement(null, null);
                set_merchant_collider(merchant, true);
                return;
            }

            var merchant_grid_pos = new int[] { (int) merchant.transform.position.x, (int) merchant.transform.position.z };
            var next_move = path_finder.get_path(merchant_grid_pos, new int[] { destination.x_pos, destination.y_pos }, tile_map.get_raw_data(), false)[1];
            iTween.MoveTo(merchant, iTween.Hash(
                "position", new Vector3( next_move.x, 1.0f, next_move.y),
                "name", "merchant_move_tween",
                "time", 2.0f,
                "oncomplete", "on_tween_complete",
                "oncompletetarget", gameObject,
                "easetype", iTween.EaseType.linear
            ));
            is_moving = true;
            merchant.GetComponent<Merchant>().set_sprite_from_movement(merchant_grid_pos, new int[] { next_move.x, next_move.y });
            set_merchant_collider(merchant, false);
            close_merchant_shop();
            Debug.Log(string.Format("Moving from ({0}, {1}) to ({2}, {3})", merchant_grid_pos[0], merchant_grid_pos[1], next_move.x, next_move.y));
        }
        else {
            Debug.Log("still moving");
        }
    }

    public void on_tween_complete() {
        is_moving = false;
    }

    void OnDestroy() {
        if (is_moving) {
            iTween.StopByName("merchant_move_tween");
        }
    }

    private int time_to_min(int hour, int min) {
        return hour * 60 + min;
    }

    private void set_merchant_collider(GameObject merchant, bool on) {
        var shop = GameObject.Find("ShopCollider");
        if (shop != null) {
            shop.GetComponent<BoxCollider>().enabled = on;
        }
    }

    private void close_merchant_shop() {
        var shop = GameObject.Find("ShopCollider");
        var shop_script = shop.GetComponent<ShopMain>();
        shop_script.closeShopWindow();
    }

    private bool is_moving;
    private ScheduleScript schedule;
    private TileMapScript tile_map;
    private GameTimeScript time;
    private WorldGenerator path_finder;
}
