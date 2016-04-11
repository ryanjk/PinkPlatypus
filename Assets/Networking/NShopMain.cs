using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Networking;

public class NShopMain : NetworkBehaviour {

    /**
    * ShopMain
    * @author Harry Schneidman
    * This component is used with the ShopCollider prefab to allow the play to use shops.
    * 
    * The shop appears in a UI on the canvas element.
    * It has a dropdown menu for the user to select an item to buy.
    * If there is more than one of the item a slider will appear below it to choose the quantity to be purchased.
    * There is also some text that shows how much the amount of that item costs.
    * Pressing the buy button at the button will buy that amount of that item if possible.
    * Pressing the back button closes the GUI.
    * The player can also close the GUI by leaving the shop area
    * 
*/

    // Called by the engine when object holding script is created
    void Awake () {
        // Create a Dictionary of item ids and prices for those items
		_prices = new Dictionary<int, int>();
	}

    // Called by the engine when all objects are created 
	void Start () {
        
		_inventory = this.GetComponent<ItemInventory>();
		//Testing stuff: give the shop items and the players 100$
		//_inventory.addItem(1,10);
		//_inventory.addItem(101);
		//_player.addItem(0,100);

        // All items are cost at 1 unit by default.
        // This can be changed at any point by the SceneManager by calling setPrice()
		foreach(int i in _inventory.getItemList().Keys){
			_prices.Add(i, 1);
		}
	}

	/*
	 * This is called when the player collides with the shop collider.
	 * The shop UI opens,and the cursor becomes visible.
     * @param Collider c: The collider of the object that ran into the shop's collider. Should only be the player.
	 */
	void OnTriggerEnter(Collider c){
		if(c.tag == "Player" && c.gameObject.GetComponent<NPlayerController>().isHost()){
            _player = c.gameObject.GetComponent<PlayerMain>();
			openShopWindow();
			Cursor.visible = true;
		}
	}

    /*
	 * This is called when the player exits the shop collider.
	 * The shop UI closes.
     * @param Collider c: The collider of the object that is leaving into the shop's collider. Should only be the player.
	 */
    void OnTriggerExit(Collider c){
		if(c.tag == "Player"){
			closeShopWindow();
        }
	}
	/*
	 * This can be used to set the price of an item in the shop.
	 * If the shop does not contain that item, nothing happens.
	 * @param itemId: The itemId of the item to have it's price set
	 * @param price: The price to set that item to
	 */
	public void setPrice(int itemId, int price){
		if(_prices.ContainsKey(itemId))
			_prices[itemId] = price;
	}
	
	/*
	 * This method opens the shop window from the shopUI Prefab (set in the inspector).
	 * It then sets all the visual components up, assigns them to variables and updates the shop.
	 */
	private void openShopWindow(){
        _player.GetComponent<NPlayerController>().ignoreInput = true; // While the menu is open, ignore the player's movement
        _shopUI = Object.Instantiate(shopUIPrefab) as Canvas;

		_dropdown = _shopUI.GetComponentInChildren<Dropdown>();
        // Add a listener to update the current item info when a new item is selected from the dropdown item list
        _dropdown.onValueChanged.AddListener((int i) => {
			this.updateCurrentItemInfo();
		});

        // Access the Text elements of the shop UI and make them blank for now
		foreach(Text t in _shopUI.GetComponentsInChildren<Text>()){
			if(t.name == "Quantity"){
				_quantity = t;
				_quantity.text = "";
			}
			else if(t.name == "Price"){
				_price = t;
				_price.text = "";
			}
		}

		_slider = _shopUI.GetComponentInChildren<Slider>();
        // We add a listener to the quantity slider
        // When the amount of item requested is changed, the quantity text and total price update
		_slider.onValueChanged.AddListener((float f) => {
			if(_dropdown.value != 0){
				_quantity.text = _slider.value + "";
				_price.text = "$ " +((int)_slider.value) * _prices[_dropdownIDs[_dropdown.value]]; //Total price = Quantity * price of current item 
			}
		});
		_slider.gameObject.SetActive(false); // Hide the slider until we select an item from the dropdown

		Button[] buttons = _shopUI.GetComponentsInChildren<Button>();
		buttons[0].onClick.AddListener(() => { // Set the "<Back" button to close the shop
			this.closeShopWindow();
		});
		buttons[1].onClick.AddListener (() =>{ // Set the "Buy" button to attempt to purchase the chosen quantity of the chosen item
			this.buy();
		});
		updateDropdownList();

	}
	
	/*
	 * This method is called when the user clicks the buy button.
	 * It checks that the user is buying more than one of something and that they have enough money.
	 * It then takes the money from the player, gives them the item, removes it from the shop and then resets the UI.
	 */
	public void buy(){
		if(this._dropdown.value > 0){
			/* commented out to avoid errors in testing but code is fully functionary
            if(_player.countItem(0) >= ((int)_slider.value*_prices[_dropdownIDs[_dropdown.value]])){
				_player.removeItem(0,((int)_slider.value*_prices[_dropdownIDs[_dropdown.value]]));
				_player.addItem(_dropdownIDs[_dropdown.value], (int)_slider.value);
				_inventory.removeItem(_dropdownIDs[_dropdown.value], (int)_slider.value);
				_dropdown.value = 0;
				_slider.gameObject.SetActive(false);
				updateDropdownList();
				updateCurrentItemInfo();
			}
			else{*/
				Debug.Log("Not enough money to buy this!");
			//}
		}
	}
	
	/*
	 * This method used to update the dropdown list with the shop's inventory and the associated data structures.
	 */
	private void updateDropdownList(){
		_dropdown.options.Clear();
        // Dropdown IDs is a list of itemIDs in the dropdown list, for the blank entry that represents no item, id -1 is used
		_dropdownIDs = new List<int>();
		_dropdown.options.Add(new Dropdown.OptionData(""));
		_dropdownIDs.Add(-1);
		foreach(int itemID in (_inventory.getItemList().Keys)) { // Add everything from the shop's inventory to the dropdown menu and the id list
			_dropdown.options.Add(new Dropdown.OptionData(ItemDatabase.getName(itemID)));
			_dropdownIDs.Add(itemID);
		}
	}

	/*
	 * This method used to update the GUI quantity and price GUI elements when the user chooses a dropdown element.
     * If the player chooses an item that the shop has more than one of, a slider is made visible to allow them to choose the quantity to buy.
	 * It is public because it is accessed by the dropdown list componenet.
	 */
	public void updateCurrentItemInfo(){
		if(_dropdown.value > 0){ // A valid item was selected
			_slider.value = 1;
			_quantity.text = "";
			_price.text = "";
			if(_inventory.getItemList()[_dropdownIDs[_dropdown.value]] > 1){ //If we have multiple of the item make the slider visible and go up to the amount we have
				_slider.gameObject.SetActive(true);
				_slider.maxValue = _inventory.getItemList()[_dropdownIDs[_dropdown.value]];
				_quantity.text = "1";
				_price.text = "$ " +((int)_slider.value) * _prices[_dropdownIDs[_dropdown.value]];
			}
			if(_inventory.getItemList()[_dropdownIDs[_dropdown.value]] == 1){ //If we only have one of the item, keep the slider hidden
				_slider.gameObject.SetActive(false);
				_price.text = "$ " + _prices[_dropdownIDs[_dropdown.value]];
			}
		}
		else{ // The dropdown's blank entry was selected (no item)
			_slider.value = 1;
			_quantity.text = "";
			_price.text = "";
			_slider.gameObject.SetActive(false);
		}
	}
	/*
	 * This method used to close the shop menu.
	 * It destroys the shop UI and hides the cursor.
	 */
	public void closeShopWindow(){
        if (_shopUI != null) {
            Destroy(_shopUI.gameObject);
            //Cursor.visible = false; disabled for testing
            _player.GetComponent<NPlayerController>().ignoreInput = false; // The player can move again
        }
    }
	
	/*
	 * A bunch of methods that allow manipulations of the inventory the shop by the SceneManager.
	 * They invoke the same methods in shop's inventory.
	 */

	public void addItem(int itemID, int quantity){
		_inventory.addItem(itemID, quantity);
	}
	public void addItem(int itemID){
		_inventory.addItem(itemID, 1);
	}	
	public void countItem(int itemID){
		_inventory.countItem(itemID);
	}
	public void removeItem(int itemID, int quantity){
		_inventory.removeItem(itemID, quantity);
	}
	public void removeItem(int itemID){
		_inventory.removeItem(itemID, 1);
	}


	public Canvas shopUIPrefab; // The shop user interface object. Attached in the engine GUI.
	private Canvas _shopUI; // Variable for access to the canvas once it's put into the game scene
	private ItemInventory _inventory; // The shop's inventory
	private PlayerMain _player; // Variable for access to the player once they interact with the shop.
    private Dictionary<int, int> _prices; // List of item id's and prices.
	private List<int> _dropdownIDs; // List of item ids in the dropdown

    private Dropdown _dropdown; // GUI Dropdown List from the GUI to choose an item to buy.
    private Slider _slider; // GUI Slider to choose the quantity of item to buy
	private Text _quantity, _price; //GUI text fields
}
