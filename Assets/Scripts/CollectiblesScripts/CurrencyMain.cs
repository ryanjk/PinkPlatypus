using UnityEngine;
using System.Collections;
using System;

/** 
* CurrencyMain
* @author Harry Schneidman
* Component with all methods for currency-giving items.
* 
* The object this component is attached to should have a Collider with IsTrigger enabled.
* 
* The "value" variable is used for the amount of currency the object should be worth.
* It should be changed with setValue() after the object is instantiated. The default value is 1.
*/

public class CurrencyMain : MonoBehaviour {

	public int value;

    public int id;
    public bool picked_up;

	/**
	 * Gives the player currency of amount equal to this component's "value" variable.
	 * After that, destroys the object.
	 */
	void giveCurrency(){
		GameObject player = GameObject.FindWithTag("Player");
		if(player != null) {
			PlayerMain playerScript = player.GetComponent<PlayerMain>();
			playerScript.addItem(0,value);
			Debug.Log ("Giving " + value + " currency to Player.");
		}
		else {
			Debug.Log ("Player Object not found");
		}
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        picked_up = true;
	}

	/**
	 * See UnityDocs: http://docs.unity3d.com/ScriptReference/Collider.OnTriggerEnter.html
	 * Calls giveCurrency() if the object colliding is the First Person Player.
	 */
	void OnTriggerEnter(Collider other){
		if(other.tag == "FPPlayer" && !picked_up){
			giveCurrency();
		}
	}

	/**
	 * Sets the value of currency object.
	 * @param pValue: The amount the currency object should be worth.
	 */
	public void setValue(int pValue){
		if(pValue > 0){
			value = pValue;
		}
	}
	/**
	 * Sets the default value of the currency object to 1 unit of currency.
	 */
	void Start(){
        picked_up = false;
	}
}
