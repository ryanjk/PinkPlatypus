using UnityEngine;
using System.Collections;

public class PortalSceneManager : SceneManager {

    protected override void prepare_scene(string destination, string source) {
        if (source.Equals("menu")) {
            // assume origin is the center for now
            _player.gameObject.transform.position.Set(0.0f, 1.0f, 0.0f);
        }

        else {
            var portal = GameObject.Find(source);
            var pos = portal.transform.position;
            _player.gameObject.transform.position = new Vector3(2.0f + pos.x, pos.y, pos.z);
        }

    }

    protected override void prepare_to_leave_scene(string destination, string source) {

    }

}
