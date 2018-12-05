using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

using TBD.Items; 

public class OnInventorySlotClick : MonoBehaviour, IPointerClickHandler {
  public PlayerInventory pInventory;

  InventorySlot slot;
  Item item;

  void Start() {
    this.slot = this.gameObject.GetComponent<InventorySlot>();
  }

  public void OnPointerClick(PointerEventData eventData) {
    this.item = this.slot.item;
    
    if (this.item == null)
      return;

    if (eventData.button == PointerEventData.InputButton.Left) {
      pInventory.MousePickup(this.item);
    } else if (eventData.button == PointerEventData.InputButton.Right) {
      pInventory.OnEquip(this.item);
    } else if (eventData.button == PointerEventData.InputButton.Middle) {
      Debug.Log(this.item.uid);
      pInventory.OnThrow(this.item);
    }
  }
}