using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;

/** 
* ItemDatabase
* @author Ryan Kitner
* Global static class that contains information about the items in the game.
*
* I decided to store the data as CSV. I think this makes sense for this type of data. It's editable in Excel, and the tabular
* format is what you've expect. JSON and XML would be cumbersome to modify. I found someone else's CSV reader online for the
* reading methods here: http://wiki.unity3d.com/index.php?title=CSVReader 
*
* I'm assuming that the database will not change during run-time. 
* Also I assume the names of the properties (i.e. columns) are:
* Name of the item = "Name"
* ID of the item   = "ID"
* Key item status  = "isKey"
* If these change they'll break the functioning of this class.
* 
*/
public class ItemDatabase : MonoBehaviour {

    /** 
    * Check if an item has a record in the database
    * @param itemID the global ID of the item
    * @return bool true if it has a record, false otherwise
    */
    public static bool isValidItem(int itemID) {
        return getItemIndex(itemID) != -1;
    }

    /**
    * Get the name of an item from its ID
    @param itemID the global ID of the item
    @return string the item's name
    */
    public static string getName(int itemID) {
        int item_index = getItemIndex(itemID);
        Assert.IsTrue(item_index != -1, "ItemDatabase: Can't get item name. Item is not valid.");

        return _data[getPropertyIndex("Name"), item_index];
    }

    /**
    * Check if an item is a key item
    * @param itemID the global ID of the item
    * @return bool true if item is a key item, false otherwise 
    */
    public static bool isKeyItem(int itemID) {
        int item_index = getItemIndex(itemID);
        Assert.IsTrue(item_index != -1, "ItemDatabase: Can't check if item is a key item. Item is not valid.");

        return Convert.ToBoolean(_data[getPropertyIndex("isKey"), item_index]);
    }

    void Awake() {
        TextAsset item_database_csv = (TextAsset)Resources.Load("item_database");
        _data = CSVReader.SplitCsvGrid(item_database_csv.text);

        _item_indices = new Dictionary<int, int>();
        for (int i = 1; i < _data.GetUpperBound(1); i++) {
            _item_indices.Add(int.Parse(_data[0, i]), i);
        }

        _property_indices = new Dictionary<string, int>();
        for (int i = 0; i < _data.GetUpperBound(0); i++) {
            _property_indices.Add(_data[i, 0], i);
        }

    }

    private static string[,] _data;
    private static Dictionary<int, int> _item_indices;
    private static Dictionary<string, int> _property_indices;

    /** 
    * Get the index of the item to use when checking its property in a column
    * @param itemID the global ID of the item
    * @return int the index if it exists, -1 otherwise
    */
    private static int getItemIndex(int itemID) {
        if (_item_indices.ContainsKey(itemID)) {
            return _item_indices[itemID];
        }
        else {
            return -1;
        }
    }

    /** 
    * Get the index of the property to use to index an item's property
    * @param property the name of the property (i.e. name of the column in the database)
    * @return int the index if it exists, -1 otherwise
    */
    private static int getPropertyIndex(string property) {
        if (_property_indices.ContainsKey(property)) {
            return _property_indices[property];
        }
        else {
            return -1;
        }
    }

    /**
    * This class is responsible for parsing the item database and creating an in-memory representation
    * Source: http://wiki.unity3d.com/index.php?title=CSVReader
    */
    private class CSVReader : MonoBehaviour {

        // outputs the content of a 2D array, useful for checking the importer
        static public void DebugOutputGrid(string[,] grid) {
            string textOutput = "";
            for (int y = 0; y < grid.GetUpperBound(1); y++) {
                for (int x = 0; x < grid.GetUpperBound(0); x++) {

                    textOutput += grid[x, y];
                    textOutput += "|";
                }
                textOutput += "\n";
            }
            Debug.Log(textOutput);
        }

        // splits a CSV file into a 2D string array
        static public string[,] SplitCsvGrid(string csvText) {
            string[] lines = csvText.Split("\n"[0]);

            // finds the max width of row
            int width = 0;
            for (int i = 0; i < lines.Length; i++) {
                string[] row = SplitCsvLine(lines[i]);
                width = Mathf.Max(width, row.Length);
            }

            // creates new 2D string grid to output to
            string[,] outputGrid = new string[width + 1, lines.Length + 1];
            for (int y = 0; y < lines.Length; y++) {
                string[] row = SplitCsvLine(lines[y]);
                for (int x = 0; x < row.Length; x++) {
                    outputGrid[x, y] = row[x];

                    // This line was to replace "" with " in my output. 
                    // Include or edit it as you wish.
                    outputGrid[x, y] = outputGrid[x, y].Replace("\"\"", "\"");
                }
            }

            return outputGrid;
        }

        // splits a CSV row 
        static public string[] SplitCsvLine(string line) {
            return (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(line,
            @"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)",
            System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
                    select m.Groups[1].Value).ToArray();
        }
    }

}
