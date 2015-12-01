using UnityEngine;
using System.Collections;

/*
 * PlayerMain
 @author Alexandra Djamen
 * Component that assembles all the essential player methods
 *
 *
 */

public class PlayerMain : MonoBehaviour {
    // Use this for initialization
	void Awake () {
		_inventory = gameObject.GetComponent<ItemInventory>();

	}
	
	// Update is called once per frame
	void Update () {

    }


	/**
    * Add an item to the inventory
    * @param itemID: ID of the item to add
    */
	public void addItem(int pId){
		_inventory.addItem (pId);
	}

	/**
	* Get the quantity of a certain item from inventory
	* @param pId: ID of item to check
	* @return the count of the item
	*/ 
	public int countItem(int pId){
		return _inventory.countItem(pId);
	}

	/**
    * Add an item to the inventory
    * @param itemID: ID of the item to add
    * @param quantity: Number of items to add
    */
	public void addItem(int pId, int pQuantity){
		_inventory.addItem (pId, pQuantity);
	}

	/**
    * Remove an item from the inventory
    * @param itemID: ID of the item to add
    * @return bool true if item is successfully removed, false otherwise
    */
	public bool removeItem(int pItemID) {
		return _inventory.removeItem(pItemID, 1);
	}

	/**
    * Remove an item from the inventory
    * @param itemID: ID of the item to add
    * @param quantity: Number of items to remove
    * @return bool true if the exact quantity of items are successfully removed, false otherwise
    */
	public bool removeItem(int pItemID, int pQuantity) {
		return _inventory.removeItem(pItemID, pQuantity);
	}


	private ItemInventory _inventory;

}
	