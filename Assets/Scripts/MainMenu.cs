using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	public bool newGame;
	public bool saveGame;
	public bool loadGame;
	public bool scores;
	public bool addPlayer;
	public bool viewItems;
	public bool exit;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnMouseUp(){
		if (newGame) {
			Application.LoadLevel(1);
		}
		if (loadGame) {
		}
		if (scores) {
		}
		if (exit) {
			Application.Quit();
		}
	}
}
