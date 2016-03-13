using UnityEngine;
using System.Collections;

public class PauseMenuScript : MonoBehaviour {

	private int _level;
	private bool _paused;
	// Use this for initialization
	void Start () {
		_level = Application.loadedLevel;
		_paused = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Return)) {
			if (_paused.Equals (true)) {
				
				foreach (Transform child in this.transform){
					child.gameObject.SetActive (true);
				}
				Application.LoadLevel (_level);
				_paused = false;
			}
			else{
				_level = Application.loadedLevel;
				Application.LoadLevel (5);
				//gameObject.SetActive (false);

				foreach (Transform child in this.transform){
				child.gameObject.SetActive(false);
				}


				_paused = true;
			}

		}
	}
}
