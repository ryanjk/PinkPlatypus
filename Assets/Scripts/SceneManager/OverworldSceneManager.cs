using UnityEngine;
using System.Collections;
using System;
using Type = TileMapData.Tile.Type;

public class OverworldSceneManager : SceneManager {

    public GameObject walkable_tile_prefab;
    public GameObject nonwalkable_tile_prefab;
    public GameObject town_tile_prefab;
    public GameObject npc_prefab;

    public GameObject portal_prefab;
    public GameObject dungeon_portal_prefab;

    protected override void prepare_scene(string destination, string source) {

        _player.GetComponent<PlayerController>().stop_moving();

        // get the map data
        var map_data = GetComponent<TileMapScript>();
        map_data.loadMap(destination + "_map_data.bin");

        //map_data.print_map();

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
                    } break;
                    case Type.OVERWORLD_NONWALKABLE: {
                        new_game_object = Instantiate(nonwalkable_tile_prefab, world_pos + new Vector3(0.0f, 1.0f, 0.0f), Quaternion.identity) as GameObject;
                    } break;
                    case Type.TOWN: {
                        new_game_object = Instantiate(town_tile_prefab, world_pos, Quaternion.identity) as GameObject;
                    } break;
                    case Type.TOWN_NPC: {
                        var new_tile = Instantiate(town_tile_prefab, world_pos, Quaternion.identity) as GameObject;
                        new_tile.transform.SetParent(map_container.transform);
                        
                        new_game_object = Instantiate(npc_prefab, world_pos + new Vector3(0.0f, 1.0f, 0.0f), Quaternion.identity) as GameObject;
                        new_game_object.GetComponentInChildren<Talk>().set_overworld_npc(true);
                    } break;
                    case Type.ENTRY_PORTAL: {
                        new_game_object = Instantiate(walkable_tile_prefab, world_pos, Quaternion.identity) as GameObject;
                        var portal = Instantiate(portal_prefab, world_pos + new Vector3(0.0f, 1.0f, 0.0f), Quaternion.identity) as GameObject;
                        portal.GetComponent<PortalScript>().destination = "PortalRoom";
                        portal.GetComponent<PortalScript>().source = destination;
                        if (source.Contains("PortalRoom")) {
                            _player.transform.position = world_pos + new Vector3(0.0f, 1.0f, -1.0f);
                        }
                    } break;
                    case Type.DUNGEON_PORTAL: {
                        new_game_object = Instantiate(walkable_tile_prefab, world_pos, Quaternion.identity) as GameObject;
                        var dungeon_portal = Instantiate(dungeon_portal_prefab, world_pos + new Vector3(0.0f, 1.0f, 0.0f), Quaternion.identity) as GameObject;
                        var world_color = destination.Substring(0, destination.IndexOf('_'));
                        dungeon_portal.GetComponent<PortalScript>().destination = world_color + "_dungeon";
                        dungeon_portal.GetComponent<PortalScript>().source = destination;
                        if (source.Contains("dungeon")) {
                            _player.transform.position = world_pos + new Vector3(0.0f, 1.0f, -1.0f);
                        }
                        } break;
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
    }
}
