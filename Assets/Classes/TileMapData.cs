using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

/**
* TileMapData class
* @author Ryan Kitner
*
* Stores the TileMapData in memory
* Map generator creates an instances of this class then saves it to disk, and then a TileMapScript loads it for 
* a GameObject to use
*/

[Serializable]
public class TileMapData {

    /**
    * TileMapData constructor
    * @param width the width of the map to create
    * @param height the height of the map to create
    */
    public TileMapData(int width, int height, string id) {
        _width = width;
        _height = height;
        _id = id;
        _tiles = new Tile[height, width];
    }

    /**
    * Set a tile in the map
    * @param x_pos x position of the tile to set
    * @param y_pos y position of the tile to set
    * @param tile reference to the tile to set in the given position
    */
    public void setTile(int x_pos, int y_pos, Tile tile) {
        _tiles[x_pos, y_pos] = tile;
    }

    /** 
    * Get a tile in the map
    * @param x_pos x position of the tile to get
    * @param y_pos y position of the tile to get
    * @return Tile a copy of the desired tile
    */
    public Tile getTile(int x_pos, int y_pos) {
        return _tiles[x_pos, y_pos];
    }

    public int getWidth() {
        return _width;
    }

    public int getHeight() {
        return _height;
    }

    /**
    * Tile class
    * Data structure that contains information every tile stores.
    * It stores a value that corresponds to some map element
    */
    [Serializable]
    public class Tile {
        public float value;

        public Tile clone() {
            Tile copy = new Tile();
            copy.value = value;
            return copy;
        }
		public bool isWalkable(){
			if(get_type()==Type.OVERWORLD_NONWALKABLE)
				return false;
			return true;
		}

        // get the type of map element the value corresponds to
        public Type get_type() {
            Type value_to_char = Type.NONE;
            if (value == 0.0f) {
                value_to_char = Type.OVERWORLD_NONWALKABLE; // non-walkable
            }
            else if (value == 1.0f) {
                value_to_char = Type.OVERWORLD_WALKABLE; // walkable
            }
            else if (value == 1.1f) {
                value_to_char = Type.ALT_OVERWORLD_WALKABLE; // walkable
            }
            else if (value == 2.0f) {
                value_to_char = Type.TOWN; // town
            }
            else if (value == 2.2f) {
                value_to_char = Type.TOWN_NPC; // town npc
            }
            else if (value == 3.0f) {
                value_to_char = Type.ENTRY_PORTAL; // portal entrance
            }
            else if (value == 3.1f) {
                value_to_char = Type.ENTRY_PORTAL_BORDER;
            }
            else if (value == 4.0f) {
                value_to_char = Type.DUNGEON_PORTAL; // portal entrance
            }
            else if (value == 4.1f) {
                value_to_char = Type.DUNGEON_PORTAL_BORDER;
            }
            return value_to_char;
        }

        public enum Type {
            OVERWORLD_WALKABLE,
            ALT_OVERWORLD_WALKABLE,
            OVERWORLD_NONWALKABLE,
            ENTRY_PORTAL_BORDER,
            ENTRY_PORTAL,
            DUNGEON_PORTAL_BORDER,
            DUNGEON_PORTAL,
            TOWN,
            TOWN_NPC,

            NONE
        }

        public override string ToString() {
            switch(get_type()) {
                case Type.OVERWORLD_NONWALKABLE: return "X";
                case Type.OVERWORLD_WALKABLE: return ".";
                case Type.ALT_OVERWORLD_WALKABLE: return ".";
                case Type.TOWN: return "T";
                case Type.ENTRY_PORTAL: return "P";
                case Type.ENTRY_PORTAL_BORDER: return "p";
                case Type.DUNGEON_PORTAL: return "D";
                case Type.DUNGEON_PORTAL_BORDER: return "d";
                default: return "";
            }
        }
    }

    /**
    * Serialize the map data to disk
    * @param filename name of the file relative to the "Assets/Resources" folder 
    */
    public void saveToDisk(string filename) {

        var map_color = filename.Substring(0, filename.IndexOf('_'));
        var map_index = SaveDataScript.map_to_slot(map_color);
        SaveDataScript.save_data.map_data[map_index] = this;
        SaveDataScript.save();
        
        /*IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(".\\Assets\\Resources\\" + filename, FileMode.Create, FileAccess.Write, FileShare.None);
        formatter.Serialize(stream, this);
        stream.Close(); */
    }

    public override string ToString() {
        string s = "";
        for (int i = 0; i < _height; i++) {
            s += "\n";
            for (int j = 0; j < _width; j++) {
                s += _tiles[i, j].ToString() + " ";
            }
        }
        return s;
    }

    public enum KeyPoint {
        PORTAL_ENTRANCE, DUNGEON_ENTRANCE, TOWN_1, TOWN_2, TOWN_3
    }

    // get a key point's coordinates from the given enum
    public int[] get_key_point(KeyPoint key_point) {
        switch(key_point) {
            case KeyPoint.DUNGEON_ENTRANCE:
                return dungeon_entrance;
            case KeyPoint.PORTAL_ENTRANCE:
                return portal_entrance;
            case KeyPoint.TOWN_1:
                return town_1;
            case KeyPoint.TOWN_2:
                return town_2;
            case KeyPoint.TOWN_3:
                return town_3;
            default:
                return null;
        }
    }

    private int _width;
    private int _height;
    private string _id;

    public string getID() {
        return _id;
    }

    public float[,] getTiles() {
        float[,] tiles = new float[_height, _width];
        for (int i = 0; i < _height; ++i) {
            for (int j = 0; j < _width; ++j) {
                tiles[i, j] = _tiles[i, j].value;
            }
        }
        return tiles;
    }

    private Tile[,] _tiles;

    // the key point corrdinates
    public int[] portal_entrance = new int[2];
    public int[] dungeon_entrance = new int[2];
    public int[] town_1 = new int[2];
    public int[] town_2 = new int[2];
    public int[] town_3 = new int[2];

}
