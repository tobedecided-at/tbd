using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

using TBD.Items;

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

  public delegate void OnItemPickup(Item item);
  public delegate void OnItemEquip(Item item);
  public delegate void OnItemThrow(Item item);

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

      invUI.lSlots.Add(goTempSlot);
    }
  }

  public bool AddToInventory(Item item) {
    bool added = false;

    // If the Item is already in the inventory
    if (inventory.Contains(item)) {
      Debug.Log("Item in inv");
      // List<T>.IndexOf returns the index of the first found instance
      // Should only ever be one because of how .Equals works
      int iIndex = inventory.IndexOf(item);
      Item invItem = inventory[iIndex];

      // If the stacksize of the item in the inventory + stacksize of picked up item
      // is less than the max allowed stacksize for that Item
      if ((invItem.stackSize + item.stackSize) < item.maxStackSize) {
        Debug.Log("stackable");
        // We can stack it without problems
        invItem.stackSize += item.stackSize;
        // And with that the item was "added" to the inventory
        // Return out
        return true;
      } // Else the Item would stack too much, so we add a new Item
      // Since this is the same as adding a completely new item, we will
      // do it in one step

    } else {
      Debug.Log("New Item");
      // The Item is not present in the inventory
      // So we add a new one
      inventory.Add(item);
      inventory[inventory.Count - 1].stackSize += item.stackSize;
      invUI.lSlots[inventory.Count - 1].GetComponent<InventorySlot>().item = item;
    }

    if (onItemPickupCB != null)
      onItemEquipCB(item);

    return true;
  }

  public Item RemoveFromInventory(Item item) {
    if (inventory.Contains(item)) {
      int iIndex = inventory.IndexOf(item);
      GameObject iUISlot = invUI.lSlots[iIndex];

      // Take care of stacksize, else we leak inventory space
      // TODO: We throw away the whole stack (for now, maybe change later)
        /*if (item.stackSize > 1) {
          item.stackSize -= 1;
        }*/

      inventory.RemoveAt(iIndex);
      iUISlot.GetComponent<InventorySlot>().item = null;

      if (onItemThrowCB != null)
        onItemThrowCB(item);
      
      return item;
    }
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
    if (RemoveFromInventory(item) == null)
      return;

    var pos = this.gameObject.transform.position;
    var rot = this.gameObject.transform.rotation;

    var spawned = ItemSpawner.SpawnItem(item, new Vector3(pos.x, pos.y+0.5f, pos.z), rot, 150f);
  }

  public void MousePickup(Item item) {
    Debug.LogWarning("TODO!");
  }

  void Update() {
    // Check if we are Overweight

    float fCalculatedWeight = 0f;
    
    foreach (Item item in inventory) {
      fCalculatedWeight += (item.weight * item.stackSize);
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