using System.Collections.Generic;
using UnityEngine;

using TBD.Items;

public class Equipment : MonoBehaviour {
  public Dictionary<EquipmentSlot, Item> EquipQueue = new Dictionary<EquipmentSlot, Item>();
  public Dictionary<EquipmentSlot, Item> equipment = new Dictionary<EquipmentSlot, Item>();

  public void AddToEquipQueue(EquipmentSlot slot, Item toEquip) {
    EquipQueue.Add(slot, toEquip);
  }

  public void Unequip() {
    // TODO
  }
}