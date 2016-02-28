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
        Stream stream = new FileStream(".\\Assets\\Resources\\" + filename, FileMode.Open);
        _tileMapData = (TileMapData)formatter.Deserialize(stream);
        stream.Close();
    }

    /** 
    * Get tile data for a specific position on the map
    * @param x_pos the x-coordinate of the tile
    * @param y_pos the y-coordinate of the tile
    * @return the tile data (a clone of it, so it's not modifiable)
    */
    public TileMapData.Tile getTile(int x_pos, int y_pos) {
        return _tileMapData.getTile(x_pos, y_pos).clone();
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
