using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

/**
* ItemInventory
* @author Ryan Kitner
* Add this component to a GameObject to give it an inventory of items. 
* 
* Notable design decisions:
* 
* - Adding an item cannot fail (i.e. doesn't return a boolean). The responsibility of ensuring something like a key item is only
*   added once should fall upon the caller. It would be wasteful making checks everytime an item is added, and I'm also trying 
*   to minimize the responsibility of this class.
*
* - An amount of an item will only be removed if it has enough to remove exactly the quantity requested. So if the inventory
*   contains 50 gold, only calls removing 50 gold or less will return true. 
*
* Finally, I used assertions for last-second error detection. Assertions are only compiled into the development build, and the
* idea with them is to catch programmers contradicting any assumptions the API makes. For example, all the methods assert that 
* the item ID passed to it corresponds to an item in the global database. If this isn't the case, the assertion will fail and
* if Assert.raiseExceptions is set to true, the execution will stop. Otherwise it will print a message and continue.
*/
public class ItemInventory : MonoBehaviour {

    /**
    * Add an item to the inventory
    * @param itemID: ID of the item to add
    */
    public void addItem(int itemID) {
        addItem(itemID, 1);
    }

    /**
    * Add an item to the inventory
    * @param itemID: ID of the item to add
    * @param quantity: Number of items to add
    */
    public void addItem(int itemID, int quantity) {
        Assert.IsTrue(ItemDatabase.isValidItem(itemID), "ItemInventory: Cannot add item. Not valid ID.");
        Assert.IsTrue(quantity >= 0, "ItemInventory: Cannot add item. Must add quantity >= 0");

        if (_items.ContainsKey(itemID)) {
            _items[itemID] += quantity;
        }
        else {
            _items.Add(itemID, quantity);
        }
    }

    /**
    * Remove an item from the inventory
    * @param itemID: ID of the item to add
    * @return bool true if item is successfully removed, false otherwise
    */
    public bool removeItem(int itemID) {
        return removeItem(itemID, 1);
    }

    /**
    * Remove an item from the inventory
    * @param itemID: ID of the item to add
    * @param quantity: Number of items to remove
    * @return bool true if the exact quantity of items are successfully removed, false otherwise
    */
    public bool removeItem(int itemID, int quantity) {
        Assert.IsTrue(ItemDatabase.isValidItem(itemID), "ItemInventory: Cannot remove item. Not valid ID.");
        Assert.IsTrue(quantity >= 0, "ItemInventory: Cannot remove item. Must remove quantity >= 0");

        int current_count = countItem(itemID);
        Assert.IsTrue(current_count > 0, "ItemInventory: Cannot remove item. Does not exist in inventory.");
        if (quantity > current_count) {
            return false;
        }
        else if (quantity == current_count) {
            _items.Remove(itemID);
            return true;
        }
        else {
            _items[itemID] -= quantity;
            return true;
        }
    }

    /**
    * Count the amount of a given item in the inventory
    * @param itemID: ID of the item to count
    * @return int the total amount of the item in the inventory 
    */
    public int countItem(int itemID) {
        Assert.IsTrue(ItemDatabase.isValidItem(itemID), "ItemInventory: Cannot count item. Not valid ID.");

        if (!_items.ContainsKey(itemID)) {
            return 0;
        }

        else {
            return _items[itemID];
        }

    }

    /**
    * @override
    * String representation of the inventory
    */
    override public string ToString() {
        string s = "";
        foreach (var itemID in _items.Keys) {
            var line = string.Format("{0, -10}: {1, -3}\n", ItemDatabase.getName(itemID), _items[itemID]);
            s += line;
        }
        return s;
    }

    void Start() {
        _items = new Dictionary<int, int>();
    }

    private Dictionary<int, int> _items;
}
