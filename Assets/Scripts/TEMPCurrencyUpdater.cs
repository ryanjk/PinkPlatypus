using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TEMPCurrencyUpdater : MonoBehaviour {
	public Text currency;
	public PlayerMain player;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		currency.text = "$ " + player.countItem(0);
	}
}
