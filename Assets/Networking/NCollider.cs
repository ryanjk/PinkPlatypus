using UnityEngine;
using System.Collections;

public class NCollider : MonoBehaviour {
    public MeshRenderer warningText;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

    }
    void OnTriggerEnter(Collider other) {
        warningText.enabled = true;
    }
    void OnTriggerExit(Collider other) {
        warningText.enabled = false;
    }
}
