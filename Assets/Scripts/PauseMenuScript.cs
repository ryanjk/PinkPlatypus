using UnityEngine;
using System.Collections;

public class PauseMenuScript : MonoBehaviour {

	private int _level;
	private bool _paused;
	// Use this for initialization
	void Start () {
		
		_level = Application.loadedLevel;
		if (_level.Equals (1))
			_level = 2; //To avoid gooing back to starting portal room and creating 2 players
		_paused = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Return)) {
			if (_paused.Equals (true)) {
				
				foreach (Transform child in this.transform){
					child.gameObject.SetActive (true);
				}
				if (_level.Equals (1))
					_level = 2; 
				Application.LoadLevel (_level);
				_paused = false;
			}
			else{
				_level = Application.loadedLevel;

				Application.LoadLevel (5);

				foreach (Transform child in this.transform){
				child.gameObject.SetActive(false);
				}


				_paused = true;
			}

		}
	}
}
