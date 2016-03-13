using UnityEngine;
using System.Collections;

public class ActivatePlayer : MonoBehaviour {

	public GameObject _gObj;
	// Use this for initialization
	void Start () {
		_gObj = GameObject.FindGameObjectWithTag ("Player");
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Return)) {
			
			//_gObj.SetActive (true);
		}
	}
}
