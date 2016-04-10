using UnityEngine;
using System.Collections;

public class PortalSceneManager : SceneManager {

    protected override void prepare_scene(string destination, string source) {
        if (source.Equals("menu")) {
            // assume origin is the center for now
            _player.gameObject.transform.position.Set(0.0f, 1.0f, 0.0f);
        }

        else if (!source.Equals("LoadMenu")) {
            // assume that the name of the source is the same as the name of the portal gameobject (e.g. world1, world2, etc)
            GameObject portal = null;
            if (GameObject.Find(source) != null) {
                portal = GameObject.Find(source);
            }
            else {
                portal = defaultPortal;
            }
            var pos = portal.transform.position;
            _player.gameObject.transform.position = new Vector3(2.0f + pos.x, pos.y, pos.z);
        }
    }

    protected override void prepare_to_leave_scene(string destination, string source) {

    }
    public GameObject defaultPortal;

}
