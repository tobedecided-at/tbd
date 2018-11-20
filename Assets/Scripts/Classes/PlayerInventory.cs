using UnityEngine;

using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour {
  [SerializeField]
  public List<Item> inventory = new List<Item>();
  public float weight;
  public float speedRed;
  public bool overWeight;
  float speed;

  public static PlayerInventory instance;

  void Start() {
    speed = TBDBootstrap.Settings.PlayerSpeed;
    speedRed = TBDBootstrap.Settings.PlayerSpeedReduction;
  }

  public void OnPickup(GameObject itemGo) {
    var itemC = itemGo.GetComponent<ItemComponent>();
    var item = itemC.item;
    var added = false;

    Destroy(itemGo);

    if (itemC.pickedUp)
      return;
    
    itemC.pickedUp = true;

    // Loop through Inventory to see if we can stack the item
    for (int x = 0; x != inventory.Count; x++) {
      var i = inventory[x];

      // If the Item is already in the Inventory
      if (i.slug == item.slug) {
        // If the stack size of the Item in the Inventory is smaller than MaxStackSize
        if (i.stackSize < i.maxStackSize) {
          // Increase StackSize of Saved Item
          i.stackSize++;
          added = true;
        } else continue; // Continue to search through inv
      }
    } // End For
    
    if (!added) {
      // If the item is not yet in the inventory OR the Stack is full
      // Increase it's stacksize and add it to the inventory
      inventory.Add(item);
      added = true;
      inventory[inventory.Count - 1].stackSize++;
    }

    if (added) {
      GiveStat(item.stats);
    }

    weight += item.weight;
  }

  public void OnThrow(GameObject item) {
    var i = item.GetComponent<ItemComponent>();
    if (!i.pickedUp)
      return;

    inventory.Remove(i.item);
    weight -= i.item.weight;
    // TODO
    // Instantiate Item infront of Player, add force
  }

  public void OnEquip(GameObject item) {
    GiveStat(item.GetComponent<ItemComponent>().item.stats);
  }

  void GiveOneTimeStat(ItemStats stats) {
    var go = this.GetComponentInParent<Transform>().gameObject;
    
    if (stats.iFood != 0f) {
      var stat = go.GetComponent<Food>();
      stat.value += stats.iFood;
    }
    if (stats.iWater != 0f) {
      var stat = go.GetComponent<Water>();
      stat.value += stats.iWater;
    }
    if (stats.iHealth != 0f) {
      var stat = go.GetComponent<Health>();
      stat.value += stats.iHealth;
    }
  }

  void GiveStat(ItemStats stats) {
    var go = this.GetComponentInParent<Transform>().gameObject;

    if (stats.iDamage != 0f) {
      var stat = go.GetComponent<Attack>();
      stat.value += stats.iHealth;
    }
    if (stats.mDamage != 0f) {
      var stat = go.GetComponent<Attack>();
      stat.value *= stats.mDamage;
    }
    if (stats.iArmor != 0f) {
      var stat = go.GetComponent<Armor>();
      stat.value += stats.iArmor;
    }
  }


  void Update() {
    // Check if we are Overweight
    if (weight > TBDBootstrap.Settings.CarryWeight && !overWeight) {
      TBDBootstrap.Settings.PlayerSpeed *= speedRed;
      overWeight = true;
    } else if (weight < TBDBootstrap.Settings.CarryWeight) {
      TBDBootstrap.Settings.PlayerSpeed = speed;
      overWeight = false;
    }

    // Loop through all Items with Stats
  }
}