using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour {

    public GameObject obj;

    void Awake() {

        PlayerMain player = GameObject.Find("Player").GetComponent<PlayerMain>();
        string destination = player._sceneLoadData.destination;
        string source = player._sceneLoadData.source;

        // Create the map
        Instantiate(obj);
    }
	
}
