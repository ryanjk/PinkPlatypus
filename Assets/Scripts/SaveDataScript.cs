using UnityEngine;
using System.Collections;
using System;

using RupeeSaveData = DungeonSceneManager.RupeeSaveData;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public class SaveDataScript : MonoBehaviour {

    [Serializable]
    public class SaveData {
        public TileMapData[] map_data = new TileMapData[5];
        public ScheduleData schedule = new ScheduleData();
        public Dictionary<int, int>[] item_inventory = new Dictionary<int, int>[2];
        public RupeeSaveData[][] dungeon_data = new RupeeSaveData[5][];

        public SaveData() {
            item_inventory[0] = new Dictionary<int, int>();
            item_inventory[1] = new Dictionary<int, int>();
            var _inventory = item_inventory[1];
            _inventory.Add(101, 1);
            _inventory.Add(102, 1);
            _inventory.Add(201, 1);
            _inventory.Add(202, 1);
            _inventory.Add(301, 1);
            _inventory.Add(302, 1);
            _inventory.Add(401, 1);
            _inventory.Add(402, 1);
            _inventory.Add(501, 1);
            _inventory.Add(502, 1);

            for (int i = 0; i < 5; ++i) {
                dungeon_data[i] = new RupeeSaveData[3];
                for (int j = 0; j < 3; ++j) {
                    var rupee_data = new RupeeSaveData();
                    rupee_data.id = j;
                    rupee_data.picked_up = false;
                    dungeon_data[i][j] = rupee_data;
                }
            }
        }
    }

    public static void save() {
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(".\\Assets\\Resources\\save_data.bin", FileMode.Create, FileAccess.Write, FileShare.None);
        formatter.Serialize(stream, save_data);
        stream.Close();
    }

    public static bool load() {
        try {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(".\\Assets\\Resources\\save_data.bin", FileMode.Open);
            save_data = formatter.Deserialize(stream) as SaveData;
            stream.Close();
            return true;
        } catch(Exception e) {
            Debug.Log("Can't load, no save data");
            return false;
        }
    }

    public static int map_to_slot(string color) {
        int map_save_slot = -1;
        if (color == "red") {
            map_save_slot = 0;
        }
        else if (color == "blue") {
            map_save_slot = 1;
        }
        else if (color == "green") {
            map_save_slot = 2;
        }
        else if (color == "purple") {
            map_save_slot = 3;
        }
        else if (color == "yellow") {
            map_save_slot = 4;
        }

        if (map_save_slot == -1) {
            Debug.Log("COULD NOT CONVERT WORLD NAME TO SLOT");
            return -1;
        }
        else {
            return map_save_slot;
        }
    }

    public static int inventory_owner_to_slot(string owner) {
        var slot_number = -1;
        if (owner == "player") {
            slot_number = 0;
        }
        else if (owner == "merchant") {
            slot_number = 1;
        }

        if (slot_number == -1) {
            Debug.Log("COULD NOT GET SLOT FOR INVENTORY OWNER");
            return -1;
        }

        return slot_number;
    }

    public static SaveData save_data;
}
