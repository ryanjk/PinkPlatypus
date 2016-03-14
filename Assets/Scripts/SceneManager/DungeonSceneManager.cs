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
        _player.gameObject.GetComponent<PlayerController>().enabled = false;
        _player.gameObject.GetComponent<Collider>().enabled = false;
        _player.gameObject.transform.Find("Camera").gameObject.SetActive(false);
        _player.gameObject.transform.Find("spriteU").gameObject.SetActive(false);
        _player.gameObject.transform.Find("spriteD").gameObject.SetActive(false);
        _player.gameObject.transform.Find("spriteR").gameObject.SetActive(false);
        _player.gameObject.transform.Find("spriteL").gameObject.SetActive(false);

        foreach (PortalScript p in (FindObjectsOfType(typeof(PortalScript))) as PortalScript[]) {
            //p.destination = _player.getSceneLoadData().source;
            p.destination = "TemplateWorld";
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
        _player.GetComponent<PlayerController>().enabled = true;
        _player.gameObject.GetComponent<Collider>().enabled = true;
        _player.gameObject.transform.Find("Camera").gameObject.SetActive(true);
        _player.gameObject.transform.Find("spriteU").gameObject.SetActive(true);
        _player.gameObject.transform.Find("spriteD").gameObject.SetActive(true);
        _player.gameObject.transform.Find("spriteR").gameObject.SetActive(true);
        _player.gameObject.transform.Find("spriteL").gameObject.SetActive(true);
        //DEBUG JUMP PLAYER SO THEY DON'T TELEPORT RIGHT AWAY
        _player.gameObject.transform.position += new Vector3(1f, 0f, 0f);
    }

}
