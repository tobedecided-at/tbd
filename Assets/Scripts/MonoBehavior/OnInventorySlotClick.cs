using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

using TBD.Items; 

public class OnInventorySlotClick : MonoBehaviour, IPointerClickHandler {
  public PlayerInventory pInventory;

  Item item;

  void Start() {
    this.item = this.gameObject.GetComponent<InventorySlot>().item;
  }

  public void OnPointerClick(PointerEventData eventData) {
    if (this.item == null)
      return;

    if (eventData.button == PointerEventData.InputButton.Left) {
      pInventory.MousePickup(this.item);
    } else if (eventData.button == PointerEventData.InputButton.Right) {
      pInventory.OnEquip(this.item);
    } else if (eventData.button == PointerEventData.InputButton.Middle) {
      pInventory.OnThrow(this.item);
    }
  }
}