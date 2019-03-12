using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

using TBD.Items;

public class PlayerItemEquipSystem : ComponentSystem {
  public struct Data {
    public readonly int Length;
    public ComponentArray<PlayerInventory> pi;
  }

  [Inject] private Data data;

  protected override void OnUpdate() {
    for (int i = 0; i != data.Length; i++) {
      var inventory = data.pi[i];

			// If the hotbar item != null
			if (inventory.hotbar == null) continue;
      Item selected = inventory.inventory[inventory.hotbar.selected];
      if (selected != null) {
        // TODO:
      }
    } 
  }
}