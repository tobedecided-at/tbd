using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

using TBD.Items; 

public class OnInventorySlotClick : MonoBehaviour,
  IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
  public PlayerInventory pInventory;

  InventorySlot slot;
  Item item;

  void Start() {
    this.slot = this.gameObject.GetComponent<InventorySlot>();
  }

  public void OnPointerClick(PointerEventData eventData) {
    this.item = this.slot.item;

    if (eventData.button == PointerEventData.InputButton.Left) {
      pInventory.MousePickup(item);
    } else if (eventData.button == PointerEventData.InputButton.Right) {
      pInventory.OnEquip(item);
    } else if (eventData.button == PointerEventData.InputButton.Middle) {
      pInventory.OnThrow(item);
    }
  }

  public void OnPointerEnter(PointerEventData eventData) {
    pInventory.isUnderMouse = slot;
  }

  public void OnPointerExit(PointerEventData eventData) {
    pInventory.isUnderMouse = null;
  }

  void Update() {

  }
}