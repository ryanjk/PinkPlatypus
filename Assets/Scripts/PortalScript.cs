using UnityEngine;
using System.Collections;

public class PortalScript : MonoBehaviour {

    public string destination;
    //public string source;

    public void switchScene() {
        //PlayerMain player = GameObject.Find("Player").GetComponent<PlayerMain>();
        //player._sceneLoadData.destination = destination;
        //player._sceneLoadData.source = source;
        Application.LoadLevel(destination);
    }

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

	}
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" || other.tag == "FPPlayer"){
            switchScene();
        }
    }

}