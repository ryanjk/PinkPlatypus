using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NColliderScript : NetworkBehaviour {

    //public string destination;
    //public string source;

    void OnTriggerEnter(Collider other) {
        if (other.tag == "NetworkPlayer") {
            Text _text = GameObject.FindGameObjectWithTag("Messages").GetComponent<Text>();
            GameObject _container = GameObject.FindGameObjectWithTag("MessageBox");
            _text.text = "You cannot use this object while in a network instance";
        }
    }
    void OnTriggerExit(Collider other) {
        
    }

}