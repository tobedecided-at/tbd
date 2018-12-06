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
      Color noAlpha = new Color(255f, 255f, 255f, 0f);

      for (int b = 0; b < inventoryUI.lSlots.Count; b++) {
        InventorySlot slot = inventoryUI.lSlots[b];

        // If slot is empty (aka no item in this slot)
        if (slot.item == null) {
          if (slot.tmproStackSize.gameObject.activeSelf)
            slot.tmproStackSize.gameObject.SetActive(false);
            slot.rimgIconHolder.texture = null;
            slot.rimgIconHolder.color = noAlpha;
          continue;
        }

        if (!slot.tmproStackSize.gameObject.activeSelf) {
          slot.tmproStackSize.gameObject.SetActive(true);
        }

        // There is an Item in the slot
        Item item = slot.item;
        // Set the alpha back to 255f
        slot.rimgIconHolder.color = fullAlpha;
        // Set the icon to the items image
        slot.rimgIconHolder.texture = slot.item.imgInventory;

        // Adjust the displayed stacksize
        slot.tmproStackSize.text = item.stackSize.ToString();
      }

      Player player = data.go[i].GetComponent<Player>();
      Camera cam = data.go[i].GetComponentInChildren<Camera>();

      // If the parent is active
      if (inventoryUI.goMouseSlot.activeSelf) {
        // If there is no item in the slot, skip
        if (inventoryUI.mouseInventorySlot.item == null) continue;

        // Set the item texture
        inventoryUI.mouseInventorySlot.rimgIconHolder.color = fullAlpha;
        inventoryUI.mouseInventorySlot.rimgIconHolder.texture = inventoryUI.mouseInventorySlot.item.imgInventory;

        // Transform the mouseInventory along the mouse cursor
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        inventoryUI.goMouseSlot.transform.position = mousePos;
      } else {
        
      }
    }
  }
}