using UnityEngine;
using System.Collections;

public class WorldGenerator : MonoBehaviour {

    private int map_width = 50;
    private int map_height = 50;

    void Start() {
        generate_world("1", map_width, map_height);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.G)) {
            generate_world("1", map_width, map_height);
        }
    }

    public void generate_world(string world_id, int width, int height) {
        var beginning_time = Time.time;
        
        // create noise
        var map_data = new NoiseGenerator().GenerateNoise(width, height);

        // flatten noise to 0 or 1
        var threshold = 0.6f; // walkable iff value <= threshold
        for (int i = 0; i < height; ++i) {
            for (int j = 0; j < width; ++j) {
                var value = map_data[i, j];
                map_data[i, j] = value <= threshold ? 1.0f : 0.0f;
            }
        }

        // place towns

        // town 1
        var town_1 = new Town(7,7);
        town_1.pos = new int[] { Random.Range(0, width - town_1.width), Random.Range(0, height - town_1.height) };
        place_town_in_map(town_1, ref map_data);

        // town 2
        var town_2 = new Town(4, 4);
        bool town_2_placed = false;
        while (!town_2_placed) {
            town_2.pos = new int[] { Random.Range(0, width - town_2.width), Random.Range(0, height - town_2.height) };
            town_2_placed = place_town_in_map(town_2, ref map_data);
        }

        // town 3
        var town_3 = new Town(15, 15);
        bool town_3_placed = false;
        while (!town_3_placed) {
            town_3.pos = new int[] { Random.Range(0, width - town_3.width), Random.Range(0, height - town_3.height) };
            town_3_placed = place_town_in_map(town_3, ref map_data);
        }

        // print the map and other info (changing as more of the 'generate_world' function is completed)
        Debug.Log(string.Format("Time taken: {0}", Time.time - beginning_time));
        Debug.Log(string.Format("Town 1 position is: {0}, {1}\n", town_1.pos[0], town_1.pos[1]));
        Debug.Log(string.Format("Town 2 position is: {0}, {1}\n", town_2.pos[0], town_2.pos[1]));
        string map_output = "";
        for (int i = 0; i < height; ++i) {
            map_output += "\n";
            for (int j = 0; j < width; ++j) {
                var value = map_data[i, j] == 2.0f ? "X" : map_data[i, j] == 0.0f ? "O" : "." ;
                map_output += string.Format("{0} ", value);
            }
        }
        Debug.Log(map_output);

        // place dungeon entrance

        // place portal entrance
    }

    private bool place_town_in_map(Town town, ref float[,] map_data) {
        // check to make sure not overwriting another town
        for (int i = town.pos[1]; i < town.pos[1] + town.height; ++i) {
            for (int j = town.pos[0]; j < town.pos[0] + town.width; ++j) {
                if (map_data[i, j] == 2.0f) {
                    return false;
                }
            }
        }

        // sure that it's not overwriting another town, so write to map data
        for (int i = town.pos[1]; i < town.pos[1] + town.height; ++i) {
            for (int j = town.pos[0]; j < town.pos[0] + town.width; ++j) {
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
