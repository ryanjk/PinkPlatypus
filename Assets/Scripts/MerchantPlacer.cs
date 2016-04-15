using UnityEngine;
using System.Collections.Generic;
public class MerchantPlacer : MonoBehaviour {
    public GameObject merchant_prefab;
	void Start () {
        time = FindObjectOfType<GameTimeScript>();
        tileMap = FindObjectOfType<TileMapScript>();
        schedule = FindObjectOfType<ScheduleScript>();
        pathFinder = gameObject.AddComponent<WorldGenerator>();
        isMoving = false;
        schedule.loadSchedule("schedule_data.bin");
        Application.runInBackground = true;
	}
	
	// Update is called once per frame
	void Update () {
        GameObject merchant = GameObject.FindGameObjectWithTag("Merchant");
        bool justAddedMerchant = false;
        if (merchant == null) {
            ScheduleData.ScheduleEntry currentlyIn = schedule.getLowerEntry(time.getHour(), time.getMinute());
            if (currentlyIn.world_id == tileMap.get_map_id()) {
                // spawn merchant, he should be here
                ScheduleData.ScheduleEntry goingTo = schedule.getUpperEntry(time.getHour(), time.getMinute());
                int currentMin = timeToMin(time.getHour(), time.getMinute());
                int currentlyInMin = timeToMin(currentlyIn.hour, currentlyIn.minute);
                int goingToMin = timeToMin(goingTo.hour, goingTo.minute);

                Vector3 spawnPoint = new Vector3();
                if (goingTo.world_id != currentlyIn.world_id) { // not on a path, no need to calculate percent along path
                    spawnPoint = new Vector3(currentlyIn.x_pos, 1.0f, currentlyIn.y_pos);
                }
                else {
                    List<WorldGenerator.Point> path = pathFinder.get_path(new int[] { currentlyIn.x_pos, currentlyIn.y_pos }, new int[] { goingTo.x_pos, goingTo.y_pos }, tileMap.get_raw_data(), false);
                    float percentAlongPath = ((float)(currentMin - currentlyInMin)) / (goingToMin - currentlyInMin);
                    var pathPosition = (int)(percentAlongPath * path.Count);
                    pathPosition = pathPosition >= path.Count ? path.Count - 1 : pathPosition;
                    var spawn_point_2d = path[pathPosition];
                    spawnPoint = new Vector3(spawn_point_2d.x, 1.0f, spawn_point_2d.y);
                }
                merchant = Instantiate(merchant_prefab, spawnPoint, Quaternion.Euler(0.0f, 270.0f, 0.0f)) as GameObject;
                Debug.Log("no merchant, adding him in");
                justAddedMerchant = true;
                setMerchantCollider(merchant, false);
            }
        }
        else if (!isMoving) {
            // merchant is here, should he still be?
            var shouldBeIn = schedule.getLowerEntry(time.getHour(), time.getMinute());
            if (shouldBeIn.world_id != tileMap.get_map_id()) {
                Destroy(merchant);
                closeMerchantShop();
                Debug.Log("removing merchant from scene");
            }
        }

        if (justAddedMerchant || merchant == null) {
            return;
        }

        // if merchant is still here and we didn't just place him in
        if (!isMoving) {
            var comingFrom = schedule.getLowerEntry(time.getHour(), time.getMinute());
            var destination = schedule.getUpperEntry(time.getHour(), time.getMinute());

            var sameEntries = comingFrom.world_id == destination.world_id &&
                comingFrom.x_pos == destination.x_pos &&
                comingFrom.y_pos == destination.y_pos;
            var differentWorlds = comingFrom.world_id != destination.world_id;
            if (atPoint(merchant, comingFrom) && (sameEntries || differentWorlds)) {
                // don't move, in waiting position
                Debug.Log(string.Format("merchant is waiting until {0}:{1}", destination.hour, destination.minute));
                merchant.GetComponent<Merchant>().set_sprite_from_movement(null, null);
                setMerchantCollider(merchant, true);
                return;
            }

            var merchantPosition = new int[] { (int) merchant.transform.position.x, (int) merchant.transform.position.z };
            var newDestination = destination;
            var nextPath = pathFinder.get_path(merchantPosition, new int[] { newDestination.x_pos, newDestination.y_pos }, tileMap.get_raw_data(), false);
            var pathIndex = nextPath.Count == 1 ? 0 : 1;
            var nextMove = nextPath[pathIndex];
            iTween.MoveTo(merchant, iTween.Hash(
                "position", new Vector3( nextMove.x, 1.0f, nextMove.y),
                "name", "merchant_move_tween",
                "time", 1.0f,
                "oncomplete", "on_tween_complete",
                "oncompletetarget", gameObject,
                "easetype", iTween.EaseType.linear
            ));
            isMoving = true;
            merchant.GetComponent<Merchant>().set_sprite_from_movement(merchantPosition, new int[] { nextMove.x, nextMove.y });
            setMerchantCollider(merchant, false);
            closeMerchantShop();
        }
    }

    public void onTweenComplete() {
        isMoving = false;
    }

    void OnDestroy() {
        if (isMoving) {
            iTween.StopByName("merchant_move_tween");
        }
        Application.runInBackground = false;
    }

    private int timeToMin(int hour, int min) {
        return hour * 60 + min;
    }

    private void setMerchantCollider(GameObject merchant, bool on) {
        var shop = GameObject.Find("ShopCollider");
        if (shop != null) {
            shop.GetComponent<BoxCollider>().enabled = on;
        }
    }

    private bool atPoint(GameObject merchant, ScheduleData.ScheduleEntry entry_point) {
        var current_pos = merchant.transform.position;

        var sameWorld = tileMap.get_map_id() == entry_point.world_id;
        var samePosition = current_pos.x == entry_point.x_pos && current_pos.z == entry_point.y_pos;

        return samePosition && sameWorld;
    }

    private void closeMerchantShop() {
        var shop = GameObject.Find("ShopCollider");
        var shopScript = shop.GetComponent<ShopMain>();
        shopScript.closeShopWindow();
    }

    private bool isMoving;
    private Merchant merchant;
    private ScheduleScript schedule;
    private TileMapScript tileMap;
    private GameTimeScript time;
    private WorldGenerator pathFinder;
}
