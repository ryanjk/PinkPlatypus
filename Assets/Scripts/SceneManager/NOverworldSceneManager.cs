using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;
using Type = TileMapData.Tile.Type;

public class NOverworldSceneManager : SceneManager {

    public GameObject walkable_tile_prefab;
    public GameObject nonwalkable_tile_prefab;

    public GameObject portal_prefab;
    public GameObject dungeon_portal_prefab;
    
    

    protected override void prepare_scene(string destination, string source) {

        _player.GetComponent<PlayerController>().stop_moving();

        // get the map data
        var map_data = GetComponent<TileMapScript>();
        map_data.loadMap(destination + "_map_data.bin");

        map_data.print_map();

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
                            new_game_object = GameObject.Instantiate(walkable_tile_prefab, world_pos, Quaternion.identity) as GameObject;
                        }
                        break;
                    case Type.OVERWORLD_NONWALKABLE: {
                            new_game_object = GameObject.Instantiate(nonwalkable_tile_prefab, world_pos + new Vector3(0.0f, 1.0f, 0.0f), Quaternion.identity) as GameObject;
                        }
                        break;
                    case Type.TOWN: {
                            new_game_object = GameObject.Instantiate(walkable_tile_prefab, world_pos, Quaternion.identity) as GameObject;
                        }
                        break;
                    case Type.ENTRY_PORTAL: {
                            new_game_object = GameObject.Instantiate(walkable_tile_prefab, world_pos, Quaternion.identity) as GameObject;
                            var portal = GameObject.Instantiate(portal_prefab, world_pos + new Vector3(0.0f, 1.0f, 0.0f), Quaternion.identity) as GameObject;
                        }
                        break;
                    case Type.DUNGEON_PORTAL: {
                            new_game_object = GameObject.Instantiate(walkable_tile_prefab, world_pos, Quaternion.identity) as GameObject;
                            var dungeon_portal = GameObject.Instantiate(dungeon_portal_prefab, world_pos + new Vector3(0.0f, 1.0f, 0.0f), Quaternion.identity) as GameObject;
                        }
                        break;
                }
                if (new_game_object != null) {
                    new_game_object.transform.SetParent(map_container.transform);
                }
            }
        }
        GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>().StartHost();
    }

    protected override void prepare_to_leave_scene(string destination, string source) {

    }

}
