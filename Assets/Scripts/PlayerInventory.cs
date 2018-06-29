using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerInventory : MonoBehaviour {
  [SerializeField]
  public static List<Item> inventory = new List<Item>();
  public float weight;
  public float speedRed;
  public bool overWeight;
  float speed;

  public static PlayerInventory instance;

  void Start() {
    if (instance != this && instance != null)
      Destroy(this);

    instance = this;
    speed = TBDBootstrap.Settings.PlayerSpeed;
    speedRed = TBDBootstrap.Settings.PlayerSpeedReduction;
  }

  public void OnPickup(GameObject itemGo) {
    var item = itemGo.GetComponent<ItemComponent>();

    Destroy(itemGo);

    if (item.pickedUp)
      return;
    
    item.pickedUp = true;
    bool added = false;

    for (int x = 0; x != inventory.Count; x++) {
      var i = inventory[x];

      if (!i.stackable) continue;
      if (i.slug == item.slug && i.stackSize < i.maxStackSize) {
        i.stackSize++;
        added = true;
        break;
      } else {
        inventory.Add(item.item);
        added = true;
      }
    }
    
    if (!added) {
      added = true;
      inventory.Add(item.item);
    }

    weight += item.weight;

  }

  public void OnThrow(GameObject item) {
    var i = item.GetComponent<ItemComponent>();
    if (!i.pickedUp)
      return;

    inventory.Remove(i.item);
    weight -= i.weight;
    // TODO
    // Instantiate Item infront of Player, add force
  }

  void Update() {
    if (weight > TBDBootstrap.Settings.CarryWeight && !overWeight) {
      TBDBootstrap.Settings.PlayerSpeed *= speedRed;
      overWeight = true;
    } else if (weight < TBDBootstrap.Settings.CarryWeight) {
      TBDBootstrap.Settings.PlayerSpeed = speed;
      overWeight = false;
    }

    if (Input.GetButtonDown("Fire2")) {
      for (int x = 0; x != inventory.Count; x++) {
        var i = inventory[x];
        Debug.Log($"{i.title} | Weight {i.weight} | Value {i.value} | Stack {i.stackSize}");
      }
    }
  }

}