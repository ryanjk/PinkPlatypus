using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Type = TileMapData.Tile.Type;

public class NetworkAdaptor : NetworkBehaviour {

    public GameObject walkable_tile_prefab;
    public GameObject nonwalkable_tile_prefab;
    public GameObject town_tile_prefab;
    public GameObject npc_prefab;

    public GameObject portal_prefab;
    public GameObject dungeon_portal_prefab;

    [SyncVar]
    public TileMapScript map;    

	void Start () {
    }
    
    public void registerPrefabs() {
        ClientScene.RegisterPrefab(walkable_tile_prefab);
        ClientScene.RegisterPrefab(nonwalkable_tile_prefab);
        ClientScene.RegisterPrefab(town_tile_prefab);
        ClientScene.RegisterPrefab(npc_prefab);
        ClientScene.RegisterPrefab(portal_prefab);
        ClientScene.RegisterPrefab(dungeon_portal_prefab);

    }

    // Update is called once per frame
    void placeMap() { }

    public void nPlaceMap(TileMapScript map_data) {
        // place each tile
        var map_container = new GameObject("Map");
        for (int i = 0; i < map_data.getMapHeight(); ++i) {
            for (int j = 0; j < map_data.getMapWidth(); ++j) {
                var tile = map_data.getTile(i, j);
                var world_pos = new Vector3(i, 0, j);
                //GameObject new_game_object = null;
                switch (tile.get_type()) {
                    case Type.OVERWORLD_WALKABLE:
                    case Type.DUNGEON_PORTAL_BORDER:
                    case Type.ENTRY_PORTAL_BORDER: {
                            GameObject new_game_object = Instantiate(walkable_tile_prefab, world_pos, Quaternion.identity) as GameObject;
                            NetworkServer.Spawn(new_game_object);
                        }
                        break;
                    case Type.OVERWORLD_NONWALKABLE: {
                            GameObject new_game_object = Instantiate(nonwalkable_tile_prefab, world_pos + new Vector3(0.0f, 1.0f, 0.0f), Quaternion.identity) as GameObject;
                            NetworkServer.Spawn(new_game_object);
                        }
                        break;
                    case Type.TOWN: {
                            GameObject new_game_object = Instantiate(town_tile_prefab, world_pos, Quaternion.identity) as GameObject;
                            NetworkServer.Spawn(new_game_object);
                        }
                        break;
                    case Type.TOWN_NPC: {
                            var new_tile = Instantiate(town_tile_prefab, world_pos, Quaternion.identity) as GameObject;
                            new_tile.transform.SetParent(map_container.transform);

                            GameObject new_game_object = Instantiate(npc_prefab, world_pos + new Vector3(0.0f, 1.0f, 0.0f), Quaternion.identity) as GameObject;
                            NetworkServer.Spawn(new_game_object);
                        }
                        break;
                    case Type.ENTRY_PORTAL: {
                            GameObject new_game_object = Instantiate(walkable_tile_prefab, world_pos, Quaternion.identity) as GameObject;
                            NetworkServer.Spawn(new_game_object);
                            GameObject portal = Instantiate(portal_prefab, world_pos + new Vector3(0.0f, 1.0f, 0.0f), Quaternion.identity) as GameObject;
                            NetworkServer.Spawn(portal);
                        }
                        break;
                    case Type.DUNGEON_PORTAL: {
                            GameObject new_game_object = Instantiate(walkable_tile_prefab, world_pos, Quaternion.identity) as GameObject;
                            NetworkServer.Spawn(new_game_object);
                            var dungeon_portal = Instantiate(dungeon_portal_prefab, world_pos + new Vector3(0.0f, 1.0f, 0.0f), Quaternion.identity) as GameObject;
                            NetworkServer.Spawn(dungeon_portal);
                        }
                        break;
                }
                //if (new_game_object != null) {
                //   new_game_object.transform.SetParent(map_container.transform);
                //}
            }
        }
    }
}