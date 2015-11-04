using UnityEngine;
using System.Collections;

public class Flashlight : MonoBehaviour {
	public Light flashlight;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyUp(KeyCode.F))
			flashlight.enabled = !flashlight.enabled;
	}
}
