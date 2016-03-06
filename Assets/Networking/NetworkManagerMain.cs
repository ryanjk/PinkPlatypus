using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NetworkManagerMain : NetworkManager{

	// Use this for initialization
	void Start () {
        _sceneMan = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<NOverworldSceneManager>();


    }

    void Update () {
	
	}
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
        GameObject player = (GameObject)Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        _sceneMan.setPlayer(player.GetComponent<PlayerMain>());
    }

    private NOverworldSceneManager _sceneMan;
}
