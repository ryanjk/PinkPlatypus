using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WorldGenerator : MonoBehaviour {

    private int map_width = 50;
    private int map_height = 50;

    void Start() {
        /*generate_world("red_overworld", map_width, map_height);
        generate_world("blue_overworld", map_width, map_height);
        generate_world("green_overworld", map_width, map_height);
        generate_world("purple_overworld", map_width, map_height);
        generate_world("yellow_overworld", map_width, map_height); */
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.G)) {
       //     generate_world("1", map_width, map_height);
        }
    }

    public void generate_world(string world_id, int width, int height) {
        var timer = System.Diagnostics.Stopwatch.StartNew();

        // create noise
        var noise_generator = new NoiseGenerator();
        var map_data = noise_generator.GenerateNoise(height, width);

        // flatten noise to 0 or 1
        var threshold = 0.5f; // walkable iff value <= threshold
        for (int i = 0; i < height; ++i) {
            for (int j = 0; j < width; ++j) {
                var value = map_data[i, j];
                map_data[i, j] = value <= threshold ? 1.0f : 0.0f;
            }
        }

        // place portal entrance
        var portal_entrance = new int[] { Random.Range(0, height - 3), Random.Range(0, width - 3) };
        place_portal_in_map(portal_entrance, 3.0f, ref map_data);
        //map_data[portal_entrance[0], portal_entrance[1]] = 3.0f;

        // place dungeon entrance
        var dungeon_entrance = new int[2];
        {
            bool dungeon_placed = false;
            while (!dungeon_placed) { // make sure it doesn't overwrite the portal entrance
                dungeon_entrance = new int[] { Random.Range(0, height - 3), Random.Range(0, width - 3) };
                dungeon_placed = place_portal_in_map(dungeon_entrance, 4.0f, ref map_data);
            }
        }

        // place towns

        // town 1
        var town_1 = new Town(15,15);
        town_1.pos = new int[] { Random.Range(0, height - town_1.height), Random.Range(0, width - town_1.width) };
        place_town_in_map(town_1, ref map_data);

        // town 2
        var town_2 = new Town(7, 7);
        {
            bool town_2_placed = false;
            while (!town_2_placed) {
                town_2.pos = new int[] { Random.Range(0, height - town_2.height), Random.Range(0, width - town_2.width) };
                town_2_placed = place_town_in_map(town_2, ref map_data);
            }
        }

        // town 3
        var town_3 = new Town(4, 4);
        {
            bool town_3_placed = false;
            while (!town_3_placed) {
                town_3.pos = new int[] { Random.Range(0, height - town_3.height), Random.Range(0, width - town_3.width) };
                town_3_placed = place_town_in_map(town_3, ref map_data);
            }
        }

        // make sure key points are reachable

        build_path_between(portal_entrance, dungeon_entrance, map_data);
        build_path_between(town_1.pos, portal_entrance, map_data);
        build_path_between(town_2.pos, portal_entrance, map_data);
        build_path_between(town_3.pos, portal_entrance, map_data);
        build_path_between(town_3.pos, town_1.pos, map_data);
        build_path_between(town_3.pos, town_2.pos, map_data);
        build_path_between(town_2.pos, town_1.pos, map_data);

        // save map to disk
        var tilemap_data = new TileMapData(map_width, map_height);
        for (int i = 0; i < map_height; ++i) {
            for (int j = 0; j < map_width; ++j) {
                var tile = new TileMapData.Tile();
                tile.value = map_data[i, j];
                tilemap_data.setTile(i, j, tile);
            }
        }
        tilemap_data.portal_entrance = new int[] { portal_entrance[0] + 1, portal_entrance[1] + 1 };
        tilemap_data.dungeon_entrance = new int[] { dungeon_entrance[0] + 1, dungeon_entrance[1] + 1 };
        tilemap_data.town_1 = new int[] { town_1.pos[0], town_1.pos[1] };
        tilemap_data.town_2 = new int[] { town_2.pos[0], town_2.pos[1] };
        tilemap_data.town_3 = new int[] { town_3.pos[0], town_3.pos[1] };
        tilemap_data.saveToDisk(world_id + "_map_data.bin");

        // print the map and other info (changing as more of the 'generate_world' function is completed)
        Debug.Log(string.Format("Time to generate world: {0} ms", timer.ElapsedMilliseconds));
        /*string map_output = "";
        for (int i = 0; i < height; ++i) {
            map_output += "\n";
            for (int j = 0; j < width; ++j) {
                var value = tilemap_data.getTile(i, j).value;
                var value_to_char = "";

                if (value == 0.0f) {
                    value_to_char = "X"; // non-walkable
                }
                else if (value == 1.0f) {
                    value_to_char = "."; // walkable
                }
                else if (value == 2.0f) {
                    value_to_char = "T"; // town
                }
                else if (value == 3.0f) {
                    value_to_char = "P"; // portal entrance
                }
                else if (value == 3.1f) {
                    value_to_char = "p";
                }
                else if (value == 4.0f) {
                    value_to_char = "D"; // portal entrance
                }
                else if (value == 4.1f) {
                    value_to_char = "d";
                }
                map_output += string.Format("{0} ", value_to_char);
            }
        }
        Debug.Log(map_output); */
    }

    private bool place_portal_in_map(int[] portal_pos, float portal_value, ref float[,] map_data) {
        for (int i = portal_pos[0]; i < portal_pos[0] + 3; ++i) {
            for (int j = portal_pos[1]; j < portal_pos[1] + 3; ++j) {
                var tile_value = map_data[i, j];
                if (tile_value != 0.0f && tile_value != 1.0f) {
                    return false;
                }
            }
        }

        for (int i = portal_pos[0]; i < portal_pos[0] + 3; ++i) {
            for (int j = portal_pos[1]; j < portal_pos[1] + 3; ++j) {
                if (i == portal_pos[0] + 1 && j == portal_pos[1] + 1) { // if center of lot, designate portal space
                    map_data[i, j] = portal_value;
                }
                else {
                    map_data[i, j] = portal_value + 0.1f;
                }
            }
        }
        return true;
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

    public List<Point> get_path(int[] from_p, int[] to_p, float[,] map_data, bool paving) {

        var from = new Point(from_p[0], from_p[1]);
        var to = new Point(to_p[0], to_p[1]);

        var visited = new HashSet<Point>(); // already visited
        var to_visit = new HashSet<Point>(); // to visit
        var came_from = new Dictionary<Point, Point>(); // maps node to best node to it
        var g_score = new Dictionary<Point, int>(); // maps node to cost of getting to it from start
        var f_score = new Dictionary<Point, int>(); // maps node to total cost of getting from the start to goal through it

        for (int i = 0; i < map_height; ++i) {
            for (int j = 0; j < map_width; ++j) {
                var point = new Point(i, j);
                g_score[point] = int.MaxValue;
                f_score[point] = int.MaxValue;
            }
        }

        g_score[from] = 0;
        f_score[from] = heuristic_estimate(from, to);

        to_visit.Add(from);
        while (to_visit.Count != 0) {
            var current = new Point();
            int cur_min_val = int.MaxValue;
            foreach (var pair in f_score) { // get point with minimum f-score in the set
                if (pair.Value < cur_min_val && to_visit.Contains(pair.Key)) {
                    cur_min_val = pair.Value;
                    current = pair.Key;
                }
            }

            if (current.Equals(to)) {
                return reconstruct_path(came_from, current);
            }

            to_visit.Remove(current);
            visited.Add(current);

            foreach (var neighbour in get_neighbours(current, to, map_data, paving)) {

                if (visited.Contains(neighbour)) {
                    continue;
                }

                var tentative_gscore = g_score[current] + 1;
                if (!to_visit.Contains(neighbour)) {
                    to_visit.Add(neighbour);
                }
                else if (tentative_gscore >= g_score[neighbour]) {
                    continue;
                }

                came_from[neighbour] = current;
                g_score[neighbour] = tentative_gscore;
                f_score[neighbour] = g_score[neighbour] + heuristic_estimate(neighbour, to);
            }
        }

        return new List<Point>();

    }

    private void build_path_between(int[] from, int[] to, float[,] map_data) {
        var path = get_path(from, to, map_data, true);
        foreach (var point in path) {
            if (map_data[point.x, point.y] == 0.0f) {
                map_data[point.x, point.y] = 1.0f;
            }
        }
    }

    public struct Point {
        public int x;
        public int y;

        public Point(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }

            var p2 = (Point)obj;
            return ((x == p2.x) && (y == p2.y));
        }

        // override object.GetHashCode
        public override int GetHashCode() {
            int result = 17;
            result = result * 31 + x;
            result = result * 31 + y;
            return result;
        }
    }

    private List<Point> get_neighbours(Point node, Point destination, float[,] map_data, bool paving) {
        var neighbours = new List<Point>();
        if (node.x != 0) { // get upper neighbour
            var n_point = new Point(node.x - 1, node.y);
            var neighbour_value = map_data[node.x - 1, node.y];
            if ( (is_walkable(neighbour_value) || n_point.Equals(destination) || (paving && neighbour_value == 0.0f)) ) {
                neighbours.Add(n_point);
            }
        }
        if (node.x != map_height - 1) { // get bottom neighbour
            var n_point = new Point(node.x + 1, node.y);
            var neighbour_value = map_data[node.x + 1, node.x];
            if ( (is_walkable(neighbour_value) || n_point.Equals(destination) || (paving && neighbour_value == 0.0f)) ) {
                neighbours.Add(n_point);
            }
        }
        if (node.y != 0) { // get left neighbour
            var n_point = new Point(node.x, node.y - 1);
            var neighbour_value = map_data[node.x, node.y - 1];
            if ((is_walkable(neighbour_value) || n_point.Equals(destination) || (paving && neighbour_value == 0.0f))) {
                neighbours.Add(n_point);
            }
        }
        if (node.y != map_width - 1) { // get right neighbour
            var n_point = new Point(node.x, node.y + 1);
            var neighbour_value = map_data[node.x, node.y + 1];
            if ((is_walkable(neighbour_value) || n_point.Equals(destination) || (paving && neighbour_value == 0.0f))) {
                neighbours.Add(n_point);
            }
        }
        return neighbours;
    }

    private List<Point> reconstruct_path(Dictionary<Point, Point> came_from, Point current) {
        var path = new List<Point>();
        path.Add(current);
        while (came_from.Keys.Contains(current)) {
            current = came_from[current];
            path.Add(current);
        }
        return path;
    }

    private int heuristic_estimate(Point from, Point to) {
        int x_diff = from.x - to.x;
        int y_diff = from.y - to.y;
        int sqr_dist = x_diff * x_diff + y_diff * y_diff;
        return sqr_dist;
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

    private bool is_walkable(float tile_value) {
        var w = tile_value == 1.0f || tile_value == 2.0f || tile_value == 3.1f || tile_value == 4.1f;
        if (!w && tile_value != 0.0f) {
            Debug.Log(tile_value);
        }
        return w;
    }
}
