using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;
using Type = TileMapData.Tile.Type;

public class NOverworldSceneManager : SceneManager {

    public GameObject walkable_tile_prefab;
    public GameObject nonwalkable_tile_prefab;
    public GameObject town_tile_prefab;
    public GameObject npc_prefab;

    public GameObject portal_prefab;
    public GameObject dungeon_portal_prefab;
    public TileMapScript map;
    public NetworkAdaptor na;

    protected override void prepare_scene(string destination, string source) {
        Debug.Log("in scene");
        _player.GetComponent<PlayerController>().stop_moving();
        if (destination.Substring(1, 1) == "S") {
            map = loadMap(destination);
            GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>().StartHost();
            na.nPlaceMap(map);
        }
    }

    private TileMapScript loadMap(string destination) {
        // get the map data
        TileMapScript map_data = new TileMapScript();
        destination = destination.Substring(2);
        map_data.loadMap(destination + "_map_data.bin");

        map_data.print_map();
        return map_data;
    }


    protected override void prepare_to_leave_scene(string destination, string source) {
        GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>().StopHost();

    }

    protected override void alt_prepare_scene() {
        na.registerPrefabs();
        GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>().StartClient();
    }
}
