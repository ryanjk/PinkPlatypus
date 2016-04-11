using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkActor : NetworkBehaviour {

    public void Start() {
        Debug.Log(GameObject.FindGameObjectsWithTag("NetworkPlayer").Length);
        if (GameObject.FindGameObjectsWithTag("NetworkPlayer").Length >= 2) {
            if (this.isLocalPlayer || this.isServer)
                player.GetComponent<NPlayerController>().switchToPlayer2();
            if (!this.isLocalPlayer)
                player.GetComponent<NPlayerController>().otherPlayer();
        } 
    }
    public GameObject player;
}
