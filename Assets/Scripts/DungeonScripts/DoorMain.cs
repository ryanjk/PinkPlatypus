using UnityEngine;
using System.Collections;

/** 
* DoorMain
* @author Harry Schneidman
* Component for Dungeon Doors.
* 
* These doors open if the player presses 'E' while standing nearby holding the key item assigned to the door.
* 
* If the player doesn't have the item a error message is printed in the debug log.
* 
* The "keyID" variable is the itemID of the item that opens the door .
*/

public class DoorMain : MonoBehaviour {
	
	public int keyID;
	
	/**
	 * See UnityDocs: http://docs.unity3d.com/ScriptReference/Collider.OnTriggerEnter.html
	 * If the player pushes E while in the trigger area, it checks if the player has the key in inventory.
	 * If they do the door destroys itself.
	 * If they don't a message is sent to the Debug Log.
	 */
	void OnTriggerStay(Collider other){
		if(Input.GetKeyUp(KeyCode.E)){
			GameObject player = GameObject.FindWithTag("Player");
			if(player != null){
				PlayerMain playerScript = player.GetComponent<PlayerMain>();
				if(playerScript.countItem(keyID) == 1){
					Destroy(gameObject);
				}
				else{
					Debug.Log ("Key not in Player Inventory");
				}
			}
			else{
				Debug.Log ("Player Object not found");
			}
		}
	}

	/**
	 * Sets the keyID to a given itemID.
	 * If the itemID is a consumable item or an invalid ID, returns false.
	 * @param pKeyID: The itemID to set the key to.
	 * @return: True if the key was succesfully set, false otherwise.
	 */
	public bool setKey(int pKeyID){
		if(ItemDatabase.isValidItem(pKeyID) && ItemDatabase.isKeyItem(pKeyID)){
			keyID = pKeyID;
			return true;
		}
		return false;
	}
	
	/**
	 * Sets the default keyID to 101 (the first world's key)
	 */
	void Start(){
		//keyID = 101;
	}

}
