using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class DungeonSceneManager : SceneManager {
    /*
    This method is called when the player loads into the scene.
    First it hides and disables movement for the overworld player object.
    Next it sets the destination of all portals in the scene to wherever the player came from.
    After that it can set the values of currency and keys that open doors (nothing to be done for the templatescene)
    */
    protected override void prepare_scene(string destination, string source) {
        _player.gameObject.GetComponent<PlayerController>().enabled = false;
        _player.gameObject.GetComponent<Collider>().enabled = false;
        _player.gameObject.GetComponent<PauseMenuScript>().enabled = false;
        _player.gameObject.transform.Find("Camera").gameObject.SetActive(false);
        _player.gameObject.transform.Find("spriteU").gameObject.SetActive(false);
        _player.gameObject.transform.Find("spriteD").gameObject.SetActive(false);
        _player.gameObject.transform.Find("spriteR").gameObject.SetActive(false);
        _player.gameObject.transform.Find("spriteL").gameObject.SetActive(false);

        dungeon_color = destination.Substring(0, destination.IndexOf('_'));
        
        // figure out what rupees to place

        var save_index = SaveDataScript.map_to_slot(dungeon_color);
        var rupees = SaveDataScript.save_data.dungeon_data[save_index];
        var rupee_game_objects = FindObjectsOfType(typeof(CurrencyMain)) as CurrencyMain[];
        foreach (var rupee in rupees) {
            foreach (var game_object in rupee_game_objects) {
                if (rupee.id == game_object.id) {
                    if (rupee.picked_up) {
                        game_object.GetComponent<MeshRenderer>().enabled = false;
                        game_object.picked_up = true;
                    }
                }
            }
        }
        
        foreach (PortalScript p in (FindObjectsOfType(typeof(PortalScript))) as PortalScript[]) {
            p.destination = source;
            p.source = destination;
        }

        var first_door = GameObject.Find("Door").GetComponent<DoorMain>();
        var second_door = GameObject.Find("Door2").GetComponent<DoorMain>();
        string source_color = source.Substring(0, source.IndexOf('_'));
        if (source_color == "red") {
            first_door.keyID = 101;
            second_door.keyID = 102;
            first_door.gameObject.GetComponent<MeshRenderer>().material = red;
            second_door.gameObject.GetComponent<MeshRenderer>().material = red;
            Platypus.color = new Color(1f, 77f/255f, 77f / 255f);
        }
        else if (source_color == "blue") {
            first_door.keyID = 201;
            second_door.keyID = 202;
            first_door.gameObject.GetComponent<MeshRenderer>().material = blue;
            second_door.gameObject.GetComponent<MeshRenderer>().material = blue;
            Platypus.color = new Color(77f/255, 85f / 255f, 255f / 255f);
        }
        else if (source_color == "green") {
            first_door.keyID = 301;
            second_door.keyID = 302;
            first_door.gameObject.GetComponent<MeshRenderer>().material = green;
            second_door.gameObject.GetComponent<MeshRenderer>().material = green;
            Platypus.color = new Color(16f / 255, 180f / 255f, 11f / 255f);
        }
        else if (source_color == "yellow") {
            first_door.keyID = 401;
            second_door.keyID = 402;
            first_door.gameObject.GetComponent<MeshRenderer>().material = yellow;
            second_door.gameObject.GetComponent<MeshRenderer>().material = yellow;
            Platypus.color = new Color(230f / 255f, 1f, 77f / 255f);
        }
        else if (source_color == "purple") {
            first_door.keyID = 501;
            second_door.keyID = 502;
            first_door.gameObject.GetComponent<MeshRenderer>().material = purple;
            second_door.gameObject.GetComponent<MeshRenderer>().material = purple;
            Platypus.color = new Color(146f / 255f, 20f/255f, 212f / 255f);
        }
    }

    /*
    This method is called before leaving the scene.
    It shows the player and enables movement for the overworld player object.
    After that it sets the player's sceneload data to the correct source and destination
    */
    protected override void prepare_to_leave_scene(string destination, string source) {
        // save rupees
        var save_index = SaveDataScript.map_to_slot(dungeon_color);
        var rupees = SaveDataScript.save_data.dungeon_data[save_index];
        CurrencyMain[] rupee_game_objects = (FindObjectsOfType(typeof(CurrencyMain))) as CurrencyMain[];
        for (int i = 0; i < rupee_game_objects.Length; ++i) {
            if (rupee_game_objects[i].gameObject.tag == "Rupee") {
                RupeeSaveData save_data;
                save_data.id = rupee_game_objects[i].id;
                save_data.picked_up = rupee_game_objects[i].picked_up;
                rupees[i] = save_data;
            }
        }
        SaveDataScript.save();

        // prepare player to leave
        _player.GetComponent<PlayerController>().enabled = true;
        _player.gameObject.GetComponent<Collider>().enabled = true;
        _player.gameObject.GetComponent<PauseMenuScript>().enabled = true;
        _player.gameObject.transform.Find("Camera").gameObject.SetActive(true);
        _player.gameObject.transform.Find("spriteU").gameObject.SetActive(true);
        _player.gameObject.transform.Find("spriteD").gameObject.SetActive(true);
        _player.gameObject.transform.Find("spriteR").gameObject.SetActive(true);
        _player.gameObject.transform.Find("spriteL").gameObject.SetActive(true);
        //DEBUG JUMP PLAYER SO THEY DON'T TELEPORT RIGHT AWAY
        _player.gameObject.transform.position += new Vector3(1f, 0f, 0f);
    }

    protected override void alt_prepare_scene() {
    }

    [Serializable]
    public struct RupeeSaveData {
        public int id;
        public bool picked_up;
    }

    public Material red;
    public Material green;
    public Material blue;
    public Material yellow;
    public Material purple;
    public SpriteRenderer Platypus;

    private string dungeon_color;

}
