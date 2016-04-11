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
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(".\\Assets\\Resources\\" + filename + "_map_data.bin", FileMode.Open);
        _tileMapData = (TileMapData)formatter.Deserialize(stream);
        stream.Close();
    }

    public float[,] get_raw_data() {
        return _tileMapData.getTiles();
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

    public int getMapWidth() {
        return _tileMapData.getWidth();
    }

    public int getMapHeight() {
        return _tileMapData.getHeight();
    }

    public int[] get_key_point (TileMapData.KeyPoint key_point) {
        return _tileMapData.get_key_point(key_point);
    }

    public void print_map() {
        Debug.Log(_tileMapData);
    }

    void Start() {
        /*
        Use this to test the loading of map data. Leaving here for possible future use.
        _tileMapData = new TileMapData(10, 10);
        bool w = true;
        for (int i = 0; i < 10; i++) {
            w = !w;
            for (int j = 0; j < 10; j++) {
                TileMapData.Tile t = new TileMapData.Tile();
                t.is_walkable = w;
                _tileMapData.setTile(i, j, t);
            }
        }
        _tileMapData.saveToDisk("map.bin");
        _tileMapData = null;
        loadMap("map.bin");
        print(_tileMapData); */
    }

    private TileMapData _tileMapData;

}
