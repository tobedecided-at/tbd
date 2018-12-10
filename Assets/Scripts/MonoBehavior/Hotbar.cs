using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TBD.Items;

public class Hotbar : MonoBehaviour {

  public List<InventorySlot> lHotbarSlots = new List<InventorySlot>();
  public GameObject goSlotHolder;

  public InventorySlot isUnderMouse;

  void Start() {

  }

  void GenerateSlots() {
  }

  public void SetSlot(int index, Item toSet) {
    
  }

  public void HideHotbar() {
    goSlotHolder.SetActive(false);
  }

  public void ShowHotbar() {
    goSlotHolder.SetActive(true);
  }
}
