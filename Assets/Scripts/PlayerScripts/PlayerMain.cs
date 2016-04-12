using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Collections.Generic;

public class PlayerMain : MonoBehaviour {
    public float speed;
    void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
	void Start () {
        //TODO: fix this, can't create a script with new. should be AddComponent.
		//_inventory = new ItemInventory ();
       // _sceneLoadData = new SceneLoadData();
       if (_times_ran == 0) {
            load_inventory();
            _times_ran++;
        }
	}

    void OnApplicationQuit() {
        Debug.Log("saving player inventory");
        _inventory.saveToDisk("player");
    }

    public void load_inventory() {
        _inventory.loadFromDisk("player");
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
    * Count an item in the inventory
    * @param itemID: ID of the item to count
    * @return int the quantity of the item the player has
    */
    public int countItem(int pItemID) {
        return _inventory.countItem(pItemID);
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
	/*
	public Vector3 getPosition(){

	}*/

    public SceneLoadData getSceneLoadData() {
        return _sceneLoadData;
    }

    public void setSceneLoadData(SceneLoadData data) {
        _sceneLoadData = data;
    }

	public ItemInventory _inventory;

    //TODO: change to private, add access methods
    private SceneLoadData _sceneLoadData;

    private static int _times_ran = 0;

} 
	