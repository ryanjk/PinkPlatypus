using UnityEngine;
using System.Collections;
using System;

public class OverworldSceneManager : SceneManager {

    protected override void prepare_scene(string destination, string source) {
        if (source == "PortalRoom" || source == "StartingPortalRoom") {
            _player.gameObject.transform.position = new Vector3(2.0f, 1.0f, 0.0f); //Player is placed to the left of the portal
        }
        if (source == "TemplateDungeon") {
            _player.gameObject.transform.position = new Vector3(-1.5f, 1.0f, 2.0f); // Player is placed to the right of the pillar
        }
    }

    protected override void prepare_to_leave_scene(string destination, string source) {
        if(destination == "PortalRoom") {
            source = "world1";
            SceneLoadData newSceneLoadData = new SceneLoadData();
            newSceneLoadData.destination = destination;
            newSceneLoadData.source = source;
            _player.setSceneLoadData(newSceneLoadData);
        }
    }

}
