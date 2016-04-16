using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

/**
* TileMapScript
* @author Ryan Kitner
*
* Main script for Tile Map
* Attach to an object to allow it to load, store and refer to a map
*/

public class TileMapScript : MonoBehaviour {

    /**
    * Load a map from a file
    * @param filename name of the file relative to the "Assets/Resources" folder
    */
    public void loadMap(string filename) {

        var map_color = filename.Substring(0, filename.IndexOf('_'));
        var map_index = SaveDataScript.map_to_slot(map_color);
        _tileMapData = SaveDataScript.save_data.map_data[map_index];
    }

    /**
    Get the map data in its raw form as a 2D array of float values
    */
    public float[,] get_raw_data() {
        return _tileMapData.getTiles();
    }

    /**
    Get the map's name (i.e. a color)
    */
    public string get_map_id() {
        return _tileMapData.getID();
    }

    /** 
    * Get tile data for a specific position on the map
    * @param x_pos the x-coordinate of the tile
    * @param y_pos the y-coordinate of the tile
    * @return the tile data (a clone of it, so it's not modifiable)
    */
    public TileMapData.Tile getTile(int x_pos, int y_pos) {
        if (_tileMapData == null) {
            return null;
        }
        return _tileMapData.getTile(x_pos, y_pos).clone();
    }

    // get the map's width
    public int getMapWidth() {
        return _tileMapData.getWidth();
    }

    // get the map's height
    public int getMapHeight() {
        return _tileMapData.getHeight();
    }

    // Get the position of a key point
    public int[] get_key_point (TileMapData.KeyPoint key_point) {
        return _tileMapData.get_key_point(key_point);
    }

    public void print_map() {
        Debug.Log(_tileMapData);
    }

    private TileMapData _tileMapData;

}
