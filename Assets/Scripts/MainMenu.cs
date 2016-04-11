using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

	public bool newGame;
	public bool saveGame;
	public bool loadGame;
	public bool enterAddress;
    public bool addPlayer;
    public bool connect;
    public bool back;
    public bool exit;
    public NetworkManager man;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnMouseUp(){
		if (newGame) {
			//Application.LoadLevel(1);
			Application.LoadLevel("WorldGenDemo");//temporary, for test
		}
		if (loadGame) {
            Application.LoadLevel("StartingPortalRoom");
        }
        if(addPlayer) {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PauseMenuScript>().hostNetwork();
        }
        if (enterAddress) {
            Application.LoadLevel("ServerConnect");
        }
        if(connect) {
            man.networkAddress = GameObject.FindGameObjectWithTag("Text").GetComponent<Text>().text;
            Application.LoadLevel("SceneGenTest");
            man.StartClient();

        }
        if (back) {
            Application.LoadLevel("MainMenu");
        }
        if (exit) {
			Application.Quit();
		}
	}
}
