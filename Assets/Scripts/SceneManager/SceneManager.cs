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

        string destination = _player.getSceneLoadData().destination;
        string source = _player.getSceneLoadData().source;

        prepare_scene(destination, source);
    }
	
	void Update () {
	
	}

    public void changeScene(string destination, string source) {
        prepare_to_leave_scene(destination, source);
        Application.LoadLevel(destination);
    }

    protected abstract void prepare_scene(string destination, string source);
    protected abstract void prepare_to_leave_scene(string destination, string source);

    protected PlayerMain _player;
}
