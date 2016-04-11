using UnityEngine;
using System.Collections;

/*
SceneManager class

Used by scenes to load appropriate data

TemplateDungeon has DungeonSceneManager
TemplateOverworld has OverworldSceneManager

On start, it figures out the data that needs to be placed in the scene by looking 
    at where the player is coming from and going

It finds the Tile Data on the harddrive and builds the map
Using the player's source it figures out where to place them in the map
*/
public abstract class SceneManager : MonoBehaviour {

	void Start () {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMain>();
        if (_player.getSceneLoadData() != null) {
            Debug.Log("Prepping scene");
            string destination = _player.getSceneLoadData().destination;
            string source = _player.getSceneLoadData().source;
            prepare_scene(destination, source);
        }
    }
	
	void Update () {
	
	}

    public void changeScene(string destination, string source) {
        SceneLoadData s = new SceneLoadData();
        s.destination = destination;
        s.source = source;
        _player.setSceneLoadData(s);

        prepare_to_leave_scene(destination, source);

        var scene_to_load = "";
        if (destination.Contains("overworld")) {
            scene_to_load = "SceneGenTest";
        }
        else if (destination.Contains("PortalRoom")) {
            scene_to_load = "PortalRoom";
        }
        else if (destination.Contains("dungeon")) {
            scene_to_load = "TemplateDungeon";
        }

        Application.LoadLevel(scene_to_load);
    }

    protected abstract void prepare_scene(string destination, string source);
    protected abstract void prepare_to_leave_scene(string destination, string source);

    protected PlayerMain _player;
}
