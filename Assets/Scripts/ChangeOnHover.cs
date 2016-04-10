using UnityEngine;
using System.Collections;

public class ChangeOnHover : MonoBehaviour {


	// Use this for initialization
	void Start () {
		GetComponent<Renderer>().material.color=Color.cyan;
	}
	
	// Update is called once per frame
	void Update () {
        
    }
	void OnMouseEnter(){
		GetComponent<Renderer>().material.color=Color.green;
	}
	void OnMouseExit(){
		GetComponent<Renderer>().material.color=Color.cyan;
    }
} 
