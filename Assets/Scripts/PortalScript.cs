using UnityEngine;
using System.Collections;

public class PortalScript : MonoBehaviour {

    public string destination;
    //public string source;

    public void switchScene() {
        //PlayerMain player = GameObject.Find("Player").GetComponent<PlayerMain>();
        //player._sceneLoadData.destination = destination;
        //player._sceneLoadData.source = source;
        GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneManager>().changeScene(Application.loadedLevelName, destination);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" || other.tag == "FPPlayer"){
            switchScene();
        }
    }

}