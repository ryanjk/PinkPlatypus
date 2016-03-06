using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NOverworldSceneManager : SceneManager {

	// Use this for initialization
	void Start () {
        _networkMan = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManagerMain>();
        _networkMan.enabled = true;
        //_networkMan.StartHost(); 
    }
	
	// Update is called once per frame
	void Update () {
        if(_player != null && _player.gameObject.transform.position.y != 1f) {
            _player.gameObject.transform.position = new Vector3(_player.gameObject.transform.position.x, 1f, _player.gameObject.transform.position.z);
        }

    }
    public void setPlayer(PlayerMain p) {
        _player = p;
        return;
    }

    protected override void prepare_scene(string destination, string source) {
    }
    protected override void prepare_to_leave_scene(string destination, string source) {
    }

    private NetworkManager _networkMan;

}
