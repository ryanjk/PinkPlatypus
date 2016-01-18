using UnityEngine;
using System.Collections;
public class PlayerMain : MonoBehaviour {
    private Rigidbody rb;
    public float speed;
    void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        //TODO: fix this, can't create a script with new. should be AddComponent.
		//_inventory = new ItemInventory ();
       // _sceneLoadData = new SceneLoadData();

	}
    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        //Updated upstream
        rb.AddForce(movement * speed);
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
	/*
	public Vector3 getPosition(){

	}*/


	private ItemInventory _inventory;

    //TODO: change to private, add access methods
   // public SceneLoadData _sceneLoadData;

} 
	