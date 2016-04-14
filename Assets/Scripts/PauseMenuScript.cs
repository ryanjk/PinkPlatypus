using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PauseMenuScript : NetworkBehaviour {

	private string _level;
	private bool _paused;
    private bool _networking;
	private Vector3 _pos;
	// Use this for initialization
	void Start () {
		
		_level = Application.loadedLevelName;
        if (_level == "StartingPortalRoom")
			_level = "PortalRoom"; //To avoid going back to starting portal room and creating 2 players
		_paused = false;
		_pos = gameObject.transform.position;
        _networking = false;

	}

    // Update is called once per frame
    void Update() {
        
        //_pos = gameObject.transform.position;
        if (Input.GetKeyDown(KeyCode.Return)) {
            if (!_networking) {
                if (_paused.Equals(true)) {

                    foreach (Transform child in this.transform) {
                        child.gameObject.SetActive(true);
                    }
                    gameObject.GetComponent<PlayerController>().enabled = true;
                    gameObject.GetComponent<Collider>().enabled = true;
                    if (_level == "StartingPortalRoom")
                        _level = "PortalRoom"; //To avoid going back to starting portal room and creating 2 players

                    SceneLoadData newSceneLoadData = new SceneLoadData();
                    newSceneLoadData.destination = gameObject.GetComponent<PlayerMain>().getSceneLoadData().source;
                    newSceneLoadData.source = "LoadMenu";
                    gameObject.GetComponent<PlayerMain>().setSceneLoadData(newSceneLoadData);

                    Application.LoadLevel(_level);
                    //gameObject.transform.position = _pos;

                    _paused = false;
                }
                else if (gameObject.GetComponent<PlayerController>().isStopped()) {
                    SceneLoadData newSceneLoadData = new SceneLoadData();
                    newSceneLoadData.destination = "LoadMenu";
                    if (gameObject.GetComponent<PlayerMain>().getSceneLoadData() != null) {
                        newSceneLoadData.source = gameObject.GetComponent<PlayerMain>().getSceneLoadData().destination;
                    }
                    else newSceneLoadData.source = "StartingPortalRoom";
                    //newSceneLoadData.source = Application.loadedLevelName;
                    gameObject.GetComponent<PlayerMain>().setSceneLoadData(newSceneLoadData);
                    _level = Application.loadedLevelName;
                    //_pos = gameObject.transform.position;

                    Application.LoadLevel("LoadMenu");

                    foreach (Transform child in this.transform) {
                        child.gameObject.SetActive(false);
                    }
                    gameObject.GetComponent<PlayerController>().enabled = false;
                    gameObject.GetComponent<Collider>().enabled = false;

                    //Time.timeScale = 0;

                    _paused = true;
                }
            }
            else {
                Debug.Log(GameObject.FindGameObjectsWithTag("NetworkPlayer").Length);
                if (GameObject.FindGameObjectsWithTag("NetworkPlayer").Length == 1) {
                    GameObject g = GameObject.FindGameObjectWithTag("NetworkPlayer");
                    gameObject.transform.position = g.transform.position;
                    GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManagerHUD>().enabled = false;
                    GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>().enabled = false;

                    foreach (Transform child in this.transform) {
                        child.gameObject.SetActive(true);
                    }
                    gameObject.GetComponent<PlayerController>().enabled = true;
                    gameObject.GetComponent<Collider>().enabled = true;
                    SceneLoadData newSceneLoadData = gameObject.GetComponent<PlayerMain>().getSceneLoadData();
                    newSceneLoadData.destination = newSceneLoadData.destination.Substring(2);
                    gameObject.GetComponent<PlayerMain>().setSceneLoadData(newSceneLoadData);
                    _networking = false;

                    Application.LoadLevel("SceneGenTest");
                }
            }
        }
    }
    public void hostNetwork() {
        if (gameObject.GetComponent<PlayerMain>().getSceneLoadData().source.Contains("overworld")) {
            _networking = true;
            SceneLoadData newSceneLoadData = new SceneLoadData();
            newSceneLoadData.source = "LoadMenu";
            newSceneLoadData.destination = "NS" + gameObject.GetComponent<PlayerMain>().getSceneLoadData().source;
            gameObject.GetComponent<PlayerMain>().setSceneLoadData(newSceneLoadData);
            Debug.Log("Loading: " + newSceneLoadData.destination);

            _paused = false;
            Debug.Log("Spawning Host Player");
            Application.LoadLevel("NSceneGenTest");

            }
    }
}
