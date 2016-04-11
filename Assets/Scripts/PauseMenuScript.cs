using UnityEngine;
using System.Collections;

public class PauseMenuScript : MonoBehaviour {

	private string _level;
	private bool _paused;
	private Vector3 _pos;
	// Use this for initialization
	void Start () {
		
		_level = Application.loadedLevelName;
        if (_level == "StartingPortalRoom")
			_level = "PortalRoom"; //To avoid going back to starting portal room and creating 2 players
		_paused = false;
		_pos = gameObject.transform.position;

	}
	
	// Update is called once per frame
	void Update () {
		
		//_pos = gameObject.transform.position;
		if (Input.GetKeyDown(KeyCode.Return)) {
			
			if (_paused.Equals (true)) {
				
				foreach (Transform child in this.transform){
					child.gameObject.SetActive (true);
                }
                if (_level == "StartingPortalRoom")
                    _level = "PortalRoom"; //To avoid going back to starting portal room and creating 2 players

                SceneLoadData newSceneLoadData = new SceneLoadData();
                newSceneLoadData.destination = gameObject.GetComponent<PlayerMain>().getSceneLoadData().source;
                newSceneLoadData.source = "LoadMenu";
                gameObject.GetComponent<PlayerMain>().setSceneLoadData(newSceneLoadData);

                Application.LoadLevel (_level);
				gameObject.transform.position = _pos;

				_paused = false;
			}
			else {
                SceneLoadData newSceneLoadData = new SceneLoadData();
                newSceneLoadData.destination = "LoadMenu";
                if (gameObject.GetComponent<PlayerMain>().getSceneLoadData()!= null) {
                    newSceneLoadData.source = gameObject.GetComponent<PlayerMain>().getSceneLoadData().destination;
                }
                else newSceneLoadData.source = "StartingPortalRoom";
                //newSceneLoadData.source = Application.loadedLevelName;
                gameObject.GetComponent<PlayerMain>().setSceneLoadData(newSceneLoadData);
                _level = Application.loadedLevelName;
				_pos = gameObject.transform.position;

				Application.LoadLevel("LoadMenu");

				foreach (Transform child in this.transform){
				    child.gameObject.SetActive(false);
				}

				//Time.timeScale = 0;

				_paused = true;
			}

		}
	}
}
