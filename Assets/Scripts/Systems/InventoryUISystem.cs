using UnityEngine;
using Unity.Entities;

using TBD.Items;
public class InventoryUISystem : ComponentSystem {

  struct Data {
    public readonly int Length;
    public GameObjectArray go;
    public ComponentArray<PlayerInput> pi;
    public ComponentArray<Transform> tr;
    public ComponentArray<PlayerInventory> pInv;
  }

  [Inject] Data data;

  protected override void OnUpdate() {
    InventoryUI inventoryUI = TBDBootstrap.Settings.UI.GetComponent<InventoryUI>();
    for (int i = 0; i != data.Length; i++) {
      bool hasToggled = data.pi[i].btnInventory;
      PlayerInventory inv = data.pInv[i];

      // If pauseBtn is down
      if (hasToggled) {
        inventoryUI.OnInventoryButton();
      }

      if (inventoryUI.isOpen) {
        var invOpen = data.go[i].GetComponent<InventoryOpen>();
        invOpen.flag = true;
      } else {
        var invOpen = data.go[i].GetComponent<InventoryOpen>();
        invOpen.flag = false;
      }

      // Loads the item's icon into the slot if available
      // Loop through all Inventory slots
      Color fullAlpha = new Color(255f, 255f, 255f, 255f);

      for (int b = 0; b < inventoryUI.lSlots.Count; b++) {
        InventorySlot slot = inventoryUI.lSlots[b].GetComponent<InventorySlot>();

        // If slot is empty (aka no item in this slot)
        if (slot.item == null) {
          if (slot.tmproStackSize.gameObject.activeSelf)
            slot.tmproStackSize.gameObject.SetActive(false);
          continue;
        }

        if (!slot.tmproStackSize.gameObject.activeSelf) {
          slot.tmproStackSize.gameObject.SetActive(true);
        }

        // There is an Item in the slot
        Item item = slot.item;
        // If the icon is not set
        if (slot.rimgIconHolder.texture == null) {
          // Set the alpha back to 255f
          slot.rimgIconHolder.color = fullAlpha;
          // Set the icon to the items image
          slot.rimgIconHolder.texture = slot.item.imgInventory;
        }

        // Adjust the displayed stacksize
        slot.tmproStackSize.text = item.stackSize.ToString();
      }
    }
  }
}