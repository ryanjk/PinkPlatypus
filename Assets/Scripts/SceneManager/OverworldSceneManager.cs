using UnityEngine;
using System.Collections;
using System;

public class OverworldSceneManager : SceneManager {

    protected override void prepare_scene(string destination, string source) {
        //throw new NotImplementedException();
    }

    protected override void prepare_to_leave_scene(string destination, string source) {
        SceneLoadData s = new SceneLoadData();
        s.destination = destination;
        s.source = source;
        _player.setSceneLoadData(s);
        //throw new NotImplementedException();
    }

}
