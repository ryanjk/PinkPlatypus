using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopMain : MonoBehaviour {

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

	void Awake () {
		_prices = new Dictionary<int, int>();
	}
	// Use this for initialization
	void Start () {

		_inventory = this.GetComponent<ItemInventory>();
		_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMain>();
		//This was stuff put in for testing
		//_inventory.addItem(1,10);
		//_inventory.addItem(101);
		//_player.addItem(0,100);

		foreach(int i in _inventory.getItemList().Keys){
			_prices.Add(i, 1);
		}
	}

	/*
	 * This is called when the player collides with the shop collider.
	 * The shop UI opens,and the cursor becomes visible.
	 */
	void OnTriggerEnter(Collider c){
		if(c.tag == "Player"){
			openShopWindow();
			Cursor.visible = true;
		}
	}
	
	/*
	 * This is called when the player exits the shop collider.
	 * The shop UI closes.
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
		_shopUI = Object.Instantiate(shopUIPrefab) as Canvas;

		_dropdown = _shopUI.GetComponentInChildren<Dropdown>();
		_dropdown.onValueChanged.AddListener((int i) => {
			this.updateQuantities();
		});

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
		_slider.onValueChanged.AddListener((float f) => {
			if(_dropdown.value != 0){
				_quantity.text = _slider.value + "";
				_price.text = "$ " +((int)_slider.value) * _prices[_dropdownIDs[_dropdown.value]];
			}
		});
		_slider.gameObject.SetActive(false);

		Button[] buttons = _shopUI.GetComponentsInChildren<Button>();
		buttons[0].onClick.AddListener(() => {
			this.closeShopWindow();
		});
		buttons[1].onClick.AddListener (() =>{
			this.buy();
		});
		refreshShopWindow();

	}
	
	/*
	 * This method is called when the user clicks the buy button.
	 * It checks that the user is buying more than one of soemthing and that they have enough money.
	 * It then takes the money from the player, gives them the item, removes it from the shop and then resets the UI.
	 */
	public void buy(){
		if(this._dropdown.value > 0){
			/*if(_player.countItem(0) >= ((int)_slider.value*_prices[_dropdownIDs[_dropdown.value]])){
				_player.removeItem(0,((int)_slider.value*_prices[_dropdownIDs[_dropdown.value]]));
				_player.addItem(_dropdownIDs[_dropdown.value], (int)_slider.value);
				_inventory.removeItem(_dropdownIDs[_dropdown.value], (int)_slider.value);
				_dropdown.value = 0;
				_slider.gameObject.SetActive(false);
				refreshShopWindow();
				updateQuantities();
			}
			else{*/
				Debug.Log("Not enough money to buy this!");
			//}
		}
	}
	
	/*
	 * This method used to update the dropdown list with the shop's inventory and the associated data structures.
	 */
	private void refreshShopWindow(){
		_dropdown.options.Clear();
		_dropdownIDs = new List<int>();
		_dropdown.options.Add(new Dropdown.OptionData(""));
		_dropdownIDs.Add(-1);
		foreach(int itemID in (_inventory.getItemList().Keys)) {
			_dropdown.options.Add(new Dropdown.OptionData(ItemDatabase.getName(itemID)));
			_dropdownIDs.Add(itemID);
		}
	}
	/*
	 * This method used to update the GUI quantity and price GUI elements when the user chooses a dropdown element.
	 * It is public because it is accessed by the dropdown list componenet.
	 */
	public void updateQuantities(){
		if(_dropdown.value > 0){
			_slider.value = 1;
			_quantity.text = "";
			_price.text = "";
			if(_inventory.getItemList()[_dropdownIDs[_dropdown.value]] > 1){
				_slider.gameObject.SetActive(true);
				_slider.maxValue = _inventory.getItemList()[_dropdownIDs[_dropdown.value]];
				_quantity.text = "1";
				_price.text = "$ " +((int)_slider.value) * _prices[_dropdownIDs[_dropdown.value]];
			}
			if(_inventory.getItemList()[_dropdownIDs[_dropdown.value]] == 1){
				_slider.gameObject.SetActive(false);
				_price.text = "$ " + _prices[_dropdownIDs[_dropdown.value]];
			}
		}
		else{
			_slider.value = 1;
			_quantity.text = "";
			_price.text = "";
			_slider.gameObject.SetActive(false);
		}
	}
	/*
	 * This method used to close the shop menu.
	 * It destorys the shop UI and hides the cursor.
	 */
	public void closeShopWindow(){
		Destroy(_shopUI.gameObject);
		//Cursor.visible = false; disabled for testing
	}
	
	/*
	 * A bunch of methods that allow manipulations of the inventory the shop is using.
	 * They invoke the same methods in inventory.
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


	public Canvas shopUIPrefab;
	private Canvas _shopUI;
	private ItemInventory _inventory;
	private PlayerMain _player;
	private Dictionary<int, int> _prices;
	private Dropdown _dropdown;
	private List<int> _dropdownIDs;
	private Slider _slider;
	private Text _quantity, _price;
}
