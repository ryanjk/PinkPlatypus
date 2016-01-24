using UnityEngine;
using System.Collections;
using System;

public class DungeonSceneManager : SceneManager {
    /*
    This method is called when the player loads into the scene.
    First it hides and disables movement for the overworld player object.
    Next it sets the destination of all portals in the scene to wherever the player came from.
    After that it can set the values of currency and keys that open doors (nothing to be done for the templatescene)
    */
    protected override void prepare_scene(string destination, string source) {
        _player.GetComponent<Renderer>().enabled = false;
        _player.GetComponent<PlayerController>().enabled = false;

        foreach (PortalScript p in (FindObjectsOfType(typeof(PortalScript))) as PortalScript[]) {
            p.destination = _player.getSceneLoadData().source;
        }

        CurrencyMain[] c = (FindObjectsOfType(typeof(CurrencyMain))) as CurrencyMain[];
        //Insert stuff to set currency values here

       DoorMain[] d = (FindObjectsOfType(typeof(DoorMain))) as DoorMain[];
        //Set keys for doors here
    }

    /*
    This method is called before leaving the scene.
    It shows the player and enables movement for the overworld player object.
    After that it sets the player's sceneload data to the correct source and destination
    */
    protected override void prepare_to_leave_scene(string destination, string source) {
        _player.GetComponent<Renderer>().enabled = true;
        _player.GetComponent<PlayerController>().enabled = true;
        SceneLoadData leaving = new SceneLoadData();
        leaving.destination = destination;
        leaving.source = source;
        _player.setSceneLoadData(leaving);
    }

}
