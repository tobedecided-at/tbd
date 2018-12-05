using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

using TBD.Items;

public class PlayerInventory : MonoBehaviour {
  [SerializeField]
  public List<Item> inventory = new List<Item>();
  public Item iOnMouse;
  public InventorySlot isUnderMouse;

  public GameObject goSlotHolder;
  public GameObject goSlotPrefab;
  
  public float weight;
  public float speedRed;
  
  public bool overWeight;

  public int iInventorySize {get; private set;}
  public InventoryUI invUI {get; private set;}

  public delegate void OnItemPickup(Item i);
  public delegate void OnItemEquip(Item i);
  public delegate void OnItemThrow(Item i);

  public OnItemPickup onItemPickupCB;
  public OnItemEquip onItemEquipCB;
  public OnItemThrow onItemThrowCB;

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
      goTempSlot.GetComponent<OnInventorySlotClick>().pInventory = this;

      invUI.lSlots.Add(goTempSlot.GetComponent<InventorySlot>());
      inventory.Add(null);
    }
  }

  public bool AddToInventory(Item item) {
    bool added = false;

    // If the Item is already in the inventory
    foreach (Item invItem in inventory) {
      if (!item.Equals(invItem)) continue;
      // List<T>.IndexOf returns the index of the first found instance
      // Should only ever be one because of how .Equals works
      int iIndex = inventory.IndexOf(item);

      // If the stacksize of the item in the inventory + stacksize of picked up item
      // is less than the max allowed stacksize for that Item
      if ((invItem.stackSize + item.stackSize) <= item.maxStackSize) {
        // We can stack it without problems
        invItem.stackSize += item.stackSize;
        // And with that the item was "added" to the inventory
        // Return out
        added = true;
        return true;
      }
      // Else the Item would stack too much, so we add a new Item
      // Since this is the same as adding a completely new item, we will
      // do it in one step
    }
    
    if (!added) {
      // The Item is not present in the inventory
      // So we add a new one
      int nextFreeIndex = GetNextFreeSlot();
      inventory[nextFreeIndex] = item;
      invUI.lSlots[nextFreeIndex].item = item;
    }

    if (onItemPickupCB != null)
      onItemPickupCB(item);

    return true;
  }

  int GetNextFreeSlot() {
    foreach (Item i in inventory) {
      if (i == null) {
        return inventory.IndexOf(i);
      } // Free slot found, return
      else // Slot occupied
        continue;
    }
    
    // No more free slots
    return -1;
  }

  public Item RemoveFromInventory(Item item) {
    for (int i = 0; i < inventory.Count; i++) {
      Item invItem = inventory[i];

      // Do UID-based comparison because really has to be the same Item
      // And not the same type
      if ((item == null || invItem == null) || item.uid != invItem.uid) continue;

      InventorySlot iUISlot = invUI.lSlots[i];

      // TODO: We throw away the whole stack (for now, maybe change later)

      inventory[i] = null;
      iUISlot.item = null;

      if (onItemThrowCB != null)
        onItemThrowCB(item);
      
      return item;
    }
    Debug.Log("failed");
    return null;
  }

  public void OnPickup(GameObject itemGo) {
    var itemC = itemGo.GetComponent<ItemComponent>();
    var item = itemC.item;
    var added = false;
    
    AddToInventory(item);
    Destroy(itemGo);

    item.specific.OnPickup();
  }

  public void OnEquip(Item item) {
    Debug.LogWarning("TODO!");
  }

  public void OnThrow(Item item) {
    // Throws away whole stack for now

    // If the Item couldn't be removed (for some reason)
    if (RemoveFromInventory(item) == null)
      return;

    var pos = this.gameObject.transform.position;
    var rot = this.gameObject.transform.rotation;

    var spawned = ItemSpawner.SpawnItem(item, new Vector3(pos.x, pos.y+0.5f, pos.z), rot, 150f);
  }

  bool AddToMouse(Item item) {
    if (item == null)
      return false;

    iOnMouse = item;
    RemoveFromInventory(item);

    return true;
  }

  bool RemoveFromMouse() {
    if (iOnMouse == null)
      return false;

    iOnMouse = null;
    return true;
  }

  public void MousePickup(Item item) {
    // If there is no Item currently in the "mouse" slot
    if (iOnMouse == null) {
      // If that somehow failed
      if (AddToMouse(item) == null)
        Debug.LogError("Failed, should not happen!");

      return;
    }
    // No else since we return
    // If there is an Item currently in "mouse" slot

    // Swap item with underMouse
    if (isUnderMouse != null) { // If there is a slot under the cursor
      Item i = isUnderMouse.item; // Try get it's item
      if (i == null) { // If the slot under the cursor is empty
        // Add the Item to the slot under the cursor
        int iSlot = invUI.lSlots.IndexOf(isUnderMouse);
        inventory[iSlot] = iOnMouse;
        invUI.lSlots[iSlot].item = iOnMouse;
        RemoveFromMouse();
      } else { // Else the slot under the cursor is occupied
        // So we switch the Items
        int iSlot = invUI.lSlots.IndexOf(isUnderMouse);

        Item temp = inventory[iSlot];
        inventory[iSlot] = iOnMouse;
        invUI.lSlots[iSlot].item = iOnMouse;

        iOnMouse = temp;
      }
    } else { // No slot under cursor
      // Throw Item on ground
      RemoveFromMouse();
      OnThrow(item);
    }
  }

  void Update() {
    // Check if we are Overweight

    float fCalculatedWeight = 0f;
    
    for (int i = 0; i < inventory.Count; i++) {
      if (inventory[i] == null) continue;
      fCalculatedWeight += (inventory[i].weight * inventory[i].stackSize);
    }

    weight = fCalculatedWeight;

    if (weight > TBDBootstrap.Settings.CarryWeight && !overWeight) {
      TBDBootstrap.Settings.PlayerSpeed *= speedRed;
      overWeight = true;
    } else if (weight < TBDBootstrap.Settings.CarryWeight) {
      TBDBootstrap.Settings.PlayerSpeed = speed;
      overWeight = false;
    }
  }
}