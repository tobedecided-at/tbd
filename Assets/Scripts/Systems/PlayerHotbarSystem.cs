using UnityEngine;
using Unity.Entities;

using TBD.Items;
public class PlayerHotbarSystem : ComponentSystem {

  struct Data {
    public readonly int Length;
    public GameObjectArray go;
    public ComponentArray<PlayerInput> pi;
    public ComponentArray<Transform> tr;
    public ComponentArray<PlayerInventory> pInventory;
  }

  [Inject] Data data;

  protected override void OnUpdate() {
    Hotbar hotbar = TBDBootstrap.Settings.UI.GetComponent<Hotbar>();
    for (int i = 0; i != data.Length; i++) {
      // true = up | false = down
      int hotbarScrollDir = data.pi[i].iHotbarScrollDir;

      PlayerInventory inventory = data.pInventory[i];

      // Initialization
      if (hotbar.selected == null) hotbar.selected = 0;
      
      // Right
      if (hotbarScrollDir == 1) {
        if (hotbar.selected + 1 >= inventory.iHotbarSize) continue;
          hotbar.selected++;

      } else if (hotbarScrollDir == -1) { // Left
        if (hotbar.selected - 1 >= 0) {
          hotbar.selected--;
        }
      } // Else nothing

      // Coloring
      var selectedSlot = inventory.invUI.lSlots[hotbar.selected];
      var highlight = selectedSlot.goHotbarSlotSelectedVisual;
      
      if (!highlight.activeSelf) {
        highlight.SetActive(true);
      }

      // Disable all hightlights except selected
      for (int j = 0; j < inventory.iHotbarSize; j++) {
        if (j != hotbar.selected)
          inventory.invUI.lSlots[j].goHotbarSlotSelectedVisual.SetActive(false);
      }
    }

  }
}