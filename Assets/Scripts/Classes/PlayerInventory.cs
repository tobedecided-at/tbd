using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour {
  [SerializeField]
  public List<Item> inventory = new List<Item>();

  public GameObject goSlotHolder;
  public GameObject goSlotPrefab;
  
  public float weight;
  public float speedRed;
  
  public bool overWeight;

  public int iInventorySize {get; private set;}
  public InventoryUI invUI {get; private set;}

  float speed;

  void Start() {
    speed = TBDBootstrap.Settings.PlayerSpeed;
    speedRed = TBDBootstrap.Settings.PlayerSpeedReduction;
    iInventorySize = TBDBootstrap.Settings.InventorySize;

    invUI = TBDBootstrap.Settings.UI.GetComponent<InventoryUI>();

    goSlotHolder = invUI.goSlotsParent;
    goSlotPrefab = invUI.goSlotsPrefab;

    GenerateSlots();
  }

  void GenerateSlots() {
    GameObject goTempSlot;
    Color invisible = new Color(255f, 255f, 255f, 0f);
    for (int i = 0; i < iInventorySize; i++) {
      goTempSlot = Instantiate(goSlotPrefab, goSlotHolder.gameObject.transform);
      goTempSlot.name = string.Format("Slot {0}", i);
      goTempSlot.GetComponent<InventorySlot>().rimgIconHolder.color = invisible;
      invUI.lSlots.Add(goTempSlot);
    }
  }

  void AddItemToUI() {
    
  }

  public void OnPickup(GameObject itemGo) {
    var itemC = itemGo.GetComponent<ItemComponent>();
    var item = itemC.item;
    var added = false;

    // Safety net
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
      // If the item is not yet in the inventory OR the stack is full
      // If the inventory is full
      if (inventory.Count == iInventorySize) {
        // Do nothing
        Debug.LogWarning("Inventory is full!");
        return;  
      }
      // Add it to the inventory and increase the stacksize from 0 to 1;
      inventory.Add(item);
      added = true;
      inventory[inventory.Count - 1].stackSize++;

      invUI.lSlots[inventory.Count - 1].GetComponent<InventorySlot>().item = item;
    }

    Destroy(itemGo);

    if (added) {
      GiveStat(item.stats);
    }
    weight += item.weight;
  }

  public void OnThrow(GameObject item) {
    // Safety
    var i = item.GetComponent<ItemComponent>();
    if (!i.pickedUp)
      return;

    inventory.Remove(i.item);
    weight -= i.item.weight;
    // TODO:
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