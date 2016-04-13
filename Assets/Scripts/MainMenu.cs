using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.IO;

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

            // clear the old save data
            File.Delete(".\\Assets\\Resources\\save_data.bin");

            SaveDataScript.save_data = new SaveDataScript.SaveData();

            world_generator = gameObject.AddComponent<WorldGenerator>();
            schedule_generator = gameObject.AddComponent<ScheduleGenerator>();

            var map_width = 50;
            var map_height = 50;

            world_generator.generate_world("red_overworld", map_width, map_height);
            world_generator.generate_world("blue_overworld", map_width, map_height);
            world_generator.generate_world("green_overworld", map_width, map_height);
            world_generator.generate_world("purple_overworld", map_width, map_height);
            world_generator.generate_world("yellow_overworld", map_width, map_height);

            schedule_generator.generate_schedule();

            Application.LoadLevel("StartingPortalRoom");
        }
        if (loadGame) {
            var loaded = SaveDataScript.load();
            if (loaded) {
                Application.LoadLevel("StartingPortalRoom");
            }
        }
        if(addPlayer) {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PauseMenuScript>().hostNetwork();
        }
        if (enterAddress) {
            Application.LoadLevel("ServerConnect");
        }
        if(connect) {
            man.networkAddress = GameObject.FindGameObjectWithTag("Text").GetComponent<Text>().text;
            Application.LoadLevel("NSceneGenTest");

        }
        if (back) {
            Destroy(GameObject.FindGameObjectWithTag("Player"));
            Application.LoadLevel("MainMenu");
        }
        if (exit) {
			Application.Quit();
		}
	}

    private WorldGenerator world_generator;
    private ScheduleGenerator schedule_generator;
}
