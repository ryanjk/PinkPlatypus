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
        

    public void placeMap(TileMapScript map_data) {
        // place each tile
        var map_container = new GameObject("Map");
        for (int i = 0; i < map_data.getMapHeight(); ++i) {
            for (int j = 0; j < map_data.getMapWidth(); ++j) {
                var tile = map_data.getTile(i, j);
                var world_pos = new Vector3(i, 0, j);
                GameObject new_game_object = null;
                switch (tile.get_type()) {
                    case Type.OVERWORLD_WALKABLE:
                    case Type.DUNGEON_PORTAL_BORDER:
                    case Type.ENTRY_PORTAL_BORDER: {
                            new_game_object = Instantiate(walkable_tile_prefab, world_pos, Quaternion.identity) as GameObject;
                        }
                        break;
                    case Type.OVERWORLD_NONWALKABLE: {
                            new_game_object = Instantiate(nonwalkable_tile_prefab, world_pos + new Vector3(0.0f, 1.0f, 0.0f), Quaternion.identity) as GameObject;
                        }
                        break;
                    case Type.TOWN: {
                            new_game_object = Instantiate(town_tile_prefab, world_pos, Quaternion.identity) as GameObject;
                        }
                        break;
                    case Type.TOWN_NPC: {
                            var new_tile = Instantiate(town_tile_prefab, world_pos, Quaternion.identity) as GameObject;
                            new_tile.transform.SetParent(map_container.transform);

                            new_game_object = Instantiate(npc_prefab, world_pos + new Vector3(0.0f, 1.0f, 0.0f), Quaternion.identity) as GameObject;
                            new_game_object.GetComponentInChildren<Talk>().set_overworld_npc(true);
                        }
                        break;
                    case Type.ENTRY_PORTAL: {
                            new_game_object = Instantiate(walkable_tile_prefab, world_pos, Quaternion.identity) as GameObject;
                            var portal = Instantiate(portal_prefab, world_pos + new Vector3(0.0f, 1.0f, 0.0f), Quaternion.identity) as GameObject;
                        }
                        break;
                    case Type.DUNGEON_PORTAL: {
                            new_game_object = Instantiate(walkable_tile_prefab, world_pos, Quaternion.identity) as GameObject;
                            var dungeon_portal = Instantiate(dungeon_portal_prefab, world_pos + new Vector3(0.0f, 1.0f, 0.0f), Quaternion.identity) as GameObject;
                        }
                        break;
                }
                if (new_game_object != null) {
                    new_game_object.transform.SetParent(map_container.transform);
                }
            }
        }
    }


    protected override void prepare_to_leave_scene(string destination, string source) {
        
    }

    protected override void alt_prepare_scene() {
        na.registerPrefabs();
        GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>().StartClient();
    }
}
