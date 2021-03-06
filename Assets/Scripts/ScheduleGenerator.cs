﻿using UnityEngine;

using KeyPoint = TileMapData.KeyPoint;
using System.IO;

/**
Class used to create a schedule and store it on disk
    @author Ryan Kitner
*/
public class ScheduleGenerator : MonoBehaviour {

	void Awake () {
        world_generator = FindObjectOfType<WorldGenerator>();
	}

    /**
    Method creates the schedule and stores it on disk
    */
    public void generate_schedule() {
        var timer = System.Diagnostics.Stopwatch.StartNew();

        // get the map data to use to build the schedule

        var red_map = gameObject.AddComponent<TileMapScript>();
        red_map.loadMap("red_overworld_map_data.bin");

        var blue_map = gameObject.AddComponent<TileMapScript>();
        blue_map.loadMap("blue_overworld_map_data.bin");

        var green_map = gameObject.AddComponent<TileMapScript>();
        green_map.loadMap("green_overworld_map_data.bin");

        var purple_map = gameObject.AddComponent<TileMapScript>();
        purple_map.loadMap("purple_overworld_map_data.bin");

        var yellow_map = gameObject.AddComponent<TileMapScript>();
        yellow_map.loadMap("yellow_overworld_map_data.bin");

        var schedule_data = new ScheduleData();

        // build the schedule by creating its parts. the variable next_time stores the time for the next part to begin at.

        var next_time = build_schedule_part(0, KeyPoint.PORTAL_ENTRANCE, KeyPoint.TOWN_1, ref schedule_data, "red", red_map);
        next_time = build_schedule_part(next_time, KeyPoint.TOWN_1, KeyPoint.TOWN_2, ref schedule_data, "red", red_map);
        next_time = build_schedule_part(next_time, KeyPoint.TOWN_2, KeyPoint.TOWN_3, ref schedule_data, "red", red_map);

        next_time = build_schedule_part(next_time, KeyPoint.PORTAL_ENTRANCE, KeyPoint.TOWN_1, ref schedule_data, "blue", blue_map);
        next_time = build_schedule_part(next_time, KeyPoint.TOWN_1, KeyPoint.TOWN_2, ref schedule_data, "blue", blue_map);
        next_time = build_schedule_part(next_time, KeyPoint.TOWN_2, KeyPoint.TOWN_3, ref schedule_data, "blue", blue_map);

        next_time = build_schedule_part(next_time, KeyPoint.PORTAL_ENTRANCE, KeyPoint.TOWN_1, ref schedule_data, "green", green_map);
        next_time = build_schedule_part(next_time, KeyPoint.TOWN_1, KeyPoint.TOWN_2, ref schedule_data, "green", green_map);
        next_time = build_schedule_part(next_time, KeyPoint.TOWN_2, KeyPoint.TOWN_3, ref schedule_data, "green", green_map);

        next_time = build_schedule_part(next_time, KeyPoint.PORTAL_ENTRANCE, KeyPoint.TOWN_1, ref schedule_data, "purple", purple_map);
        next_time = build_schedule_part(next_time, KeyPoint.TOWN_1, KeyPoint.TOWN_2, ref schedule_data, "purple", purple_map);
        next_time = build_schedule_part(next_time, KeyPoint.TOWN_2, KeyPoint.TOWN_3, ref schedule_data, "purple", purple_map);

        next_time = build_schedule_part(next_time, KeyPoint.PORTAL_ENTRANCE, KeyPoint.TOWN_1, ref schedule_data, "yellow", yellow_map);
        next_time = build_schedule_part(next_time, KeyPoint.TOWN_1, KeyPoint.TOWN_2, ref schedule_data, "yellow", yellow_map);
        next_time = build_schedule_part(next_time, KeyPoint.TOWN_2, KeyPoint.TOWN_3, ref schedule_data, "yellow", yellow_map);

        var end_point = yellow_map.get_key_point(KeyPoint.TOWN_3);
        schedule_data.insertEntry(create_schedule_entry(next_time, "yellow", end_point[0], end_point[1]));

        // add padding to make it get to 24 hours

        var padding_amount = ((24 * 60) - (next_time)) / 15;
        schedule_data.addPadding(padding_amount);

        schedule_data.saveToDisk("schedule_data.bin");

#if UNITY_EDITOR
        File.WriteAllText(".\\Assets\\Resources\\schedule_data.txt", schedule_data.ToString());
#endif

        Debug.Log(string.Format("Time to generate schedule: {0} ms", timer.ElapsedMilliseconds));
        Debug.Log(schedule_data.ToString());
    }

    // create an entry that begins at the given time between two key points. return the time the next entry should begin at.
    private int build_schedule_part(int start_min, KeyPoint from, KeyPoint to, ref ScheduleData schedule_data, string map_id, TileMapScript map_data) {

        var from_point = map_data.get_key_point(from);
        var to_point = map_data.get_key_point(to);

        // get the path between the two points
        var path = world_generator.get_path(from_point, to_point, map_data.get_raw_data(), false); 

        // make sure that a path exists first (if not there's a bug somewhere. this shouldn't happen because world gen ensures a path exists.)
        if (path.Count == 0) {
            Debug.Log(string.Format("Can't make entry in {4} from ({0}, {1}) to ({2},{3})", 
                from_point[0], from_point[1], to_point[0], to_point[1], map_id));
            map_data.print_map();
            return -1;
        }

        var path_length = path.Count - 1;
        var MERCHANT_MIN_PER_TILE = 1.0f;

        // the time to be at the destination is proportional to the length of the path times the merchant's speed
        // it's set so that it gives the perfect amount of time for the merchant to get there on time
        int finish_time = start_min + (int) (MERCHANT_MIN_PER_TILE * path_length);

        schedule_data.insertEntry(create_schedule_entry(start_min, map_id, from_point[0], from_point[1]));
        schedule_data.insertEntry(create_schedule_entry(finish_time, map_id, to_point[0], to_point[1]));

        // return the next time to create an entry at, which is WAITING_TIME more than the finish time, because the merchant waits at the spot for WAITING_TIME
        var WAITING_TIME_IN_MIN = 25;
        finish_time += WAITING_TIME_IN_MIN;
        return finish_time;
    }

    // Private method that assists filling a schedule entry
    private ScheduleData.ScheduleEntry create_schedule_entry(int total_min, string map_id, int x_pos, int y_pos) {
        var schedule_entry = new ScheduleData.ScheduleEntry();
        schedule_entry.hour = total_min / 60;
        schedule_entry.minute = total_min % 60;
        schedule_entry.world_id = map_id;
        schedule_entry.x_pos = x_pos;
        schedule_entry.y_pos = y_pos;
        return schedule_entry;
    }

    private WorldGenerator world_generator;
}
