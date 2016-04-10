using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NPortalScript : NetworkBehaviour {

    public string destination;
    //public string source;

    public void switchScene() {
        //PlayerMain player = GameObject.Find("Player").GetComponent<PlayerMain>();
        //player._sceneLoadData.destination = destination;
        //player._sceneLoadData.source = source;
        (FindObjectOfType(typeof(SceneManager)) as SceneManager).changeScene(destination, Application.loadedLevelName);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" || other.tag == "FPPlayer"){
            switchScene();
        }
    }

}