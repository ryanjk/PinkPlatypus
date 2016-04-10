using UnityEngine;
using System.Collections;

public class WorldGenerator : MonoBehaviour {

    private int map_width = 50;
    private int map_height = 25;

    void Start() {
        generate_world("1", map_width, map_height);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.G)) {
            generate_world("1", map_width, map_height);
        }
    }

    public void generate_world(string world_id, int width, int height) {
        var beginning_time = System.DateTime.Now.Millisecond;
        
        // create noise
        var map_data = new NoiseGenerator().GenerateNoise(height, width);

        // flatten noise to 0 or 1
        var threshold = 0.6f; // walkable iff value <= threshold
        for (int i = 0; i < height; ++i) {
            for (int j = 0; j < width; ++j) {
                var value = map_data[i, j];
                map_data[i, j] = value <= threshold ? 1.0f : 0.0f;
            }
        }

        // place portal entrance
        var portal_entrance = new int[] { Random.Range(0, height), Random.Range(0, width) };
        map_data[portal_entrance[0], portal_entrance[1]] = 3.0f;

        // place dungeon entrance
        var dungeon_entrance = new int[2];
        {
            bool dungeon_placed = false;
            while (!dungeon_placed) { // make sure it doesn't overwrite the portal entrance
                dungeon_entrance = new int[] { Random.Range(0, height), Random.Range(0, width) };
                if (dungeon_entrance[0] != portal_entrance[0] && dungeon_entrance[1] != portal_entrance[1]) {
                    map_data[dungeon_entrance[0], dungeon_entrance[1]] = 4.0f;
                    dungeon_placed = true;
                }
            }
        }

        // place towns

        // town 1
        var town_1 = new Town(7,7);
        town_1.pos = new int[] { Random.Range(0, height - town_1.height), Random.Range(0, width - town_1.width) };
        place_town_in_map(town_1, ref map_data);

        // town 2
        var town_2 = new Town(4, 4);
        {
            bool town_2_placed = false;
            while (!town_2_placed) {
                town_2.pos = new int[] { Random.Range(0, height - town_2.height), Random.Range(0, width - town_2.width) };
                town_2_placed = place_town_in_map(town_2, ref map_data);
            }
        }

        // town 3
        var town_3 = new Town(15, 15);
        {
            bool town_3_placed = false;
            while (!town_3_placed) {
                town_3.pos = new int[] { Random.Range(0, height - town_3.height), Random.Range(0, width - town_3.width) };
                town_3_placed = place_town_in_map(town_3, ref map_data);
            }
        }

        // make sure key points are reachable

        // print the map and other info (changing as more of the 'generate_world' function is completed)
        Debug.Log(string.Format("Time taken: {0} ms", System.DateTime.Now.Millisecond - beginning_time));
        Debug.Log(string.Format("Portal position is: {1}, {0}\n", portal_entrance[0], portal_entrance[1]));
        Debug.Log(string.Format("Town 1 position is: {1}, {0}\n", town_1.pos[0], town_1.pos[1]));
        Debug.Log(string.Format("Town 2 position is: {1}, {0}\n", town_2.pos[0], town_2.pos[1]));
        Debug.Log(string.Format("Town 3 position is: {1}, {0}\n", town_3.pos[0], town_3.pos[1]));
        string map_output = "";
        for (int i = 0; i < height; ++i) {
            map_output += "\n";
            for (int j = 0; j < width; ++j) {
                var value = map_data[i, j];
                var value_to_char = "";
                if (value == 0.0f) {
                    value_to_char = "X"; // non-walkable
                }
                else if (value == 1.0f) {
                    value_to_char = "."; // walkable
                }
                else if (value == 2.0f) {
                    value_to_char = "o"; // town
                }
                else if (value == 3.0f) {
                    value_to_char = "P"; // portal entrance
                }
                else if (value == 4.0f) {
                    value_to_char = "D"; // portal entrance
                }
                map_output += string.Format("{0} ", value_to_char);
            }
        }
        Debug.Log(map_output);
    }

    private bool place_town_in_map(Town town, ref float[,] map_data) {
        // check to make sure not overwriting another town or portal entrance
        for (int i = town.pos[0]; i < town.pos[0] + town.height; ++i) {
            for (int j = town.pos[1]; j < town.pos[1] + town.width; ++j) {
                var tile_value = map_data[i, j];
                if (tile_value != 0.0f && tile_value != 1.0f) { // a town can only overwrite a walkable or non-walkable tile
                    return false;
                }
            }
        }

        // sure that it's not overwriting another town, so write to map data
        for (int i = town.pos[0]; i < town.pos[0] + town.height; ++i) {
            for (int j = town.pos[1]; j < town.pos[1] + town.width; ++j) {
                map_data[i, j] = 2.0f;
            }
        }

        // success
        return true;
    }

    private class Town {
        public int[] pos;
        public int width;
        public int height;

        public Town(int width, int height) {
            this.width = width;
            this.height = height;
        }
    }
}
