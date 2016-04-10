using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NOverworldSceneManager : SceneManager {

	// Use this for initialization
	void Start () {
        //_networkMan.enabled = true;
        _networkAct = gameObject.GetComponent<NetworkActor>();
        //_networkMan.StartHost(); 
    }
	
	// Update is called once per frame
	void Update () {

    }

    public void setPlayer(PlayerMain p) {
        Debug.Log("it worked!");
        _player = p;
        return;
    }

    protected override void prepare_scene(string destination, string source) {
    }
    protected override void prepare_to_leave_scene(string destination, string source) {
    }
    
    private NetworkActor _networkAct;

}
