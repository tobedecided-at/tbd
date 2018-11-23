using UnityEngine;
using Unity.Entities;

public class InventoryUISystem : ComponentSystem {

  struct Data {
    public readonly int Length;
    public GameObjectArray go;
    public ComponentArray<PlayerInput> pi;
    public ComponentArray<Transform> tr;
  }

  [Inject] Data data;

  protected override void OnUpdate() {
    InventoryUI inventoryUI = TBDBootstrap.Settings.UI.GetComponent<InventoryUI>();
    for (int i = 0; i != data.Length; i++) {
      bool hasToggled = data.pi[i].btnInventory;

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
      
    }
  }
}