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
    public TileMapData(int width, int height) {
        _width = width;
        _height = height;
        _tiles = new Tile[width, height];
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
    * Tile class
    * data structure that contains information every tile stores. For now, just store if the tile is walkable.
    */
    [Serializable]
    public class Tile {
        public bool is_walkable;
    }

    /**
    * Serialize the map data to disk
    * @param filename name of the file relative to the "Assets/Resources" folder 
    */
    public void saveToDisk(string filename) {
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(".\\Assets\\Resources\\" + filename, FileMode.Create, FileAccess.Write, FileShare.None);
        formatter.Serialize(stream, this);
        stream.Close();
    }

    public override string ToString() {
        string s = "";
        for (int i = 0; i < _width; i++) {
            s += "\n";
            for (int j = 0; j < _height; j++) {
                if (_tiles[i, j].is_walkable) {
                    s += "1 ";
                }
                else {
                    s += "0 ";
                }
            }
        }
        return s;
    }

    private int _width;
    private int _height;
    private Tile[,] _tiles;

}
