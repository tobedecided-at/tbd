using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

using TBD.Items;

public class PlayerItemEquipSystem : ComponentSystem {
  public struct Data {
    public readonly int Length;
    public ComponentArray<Equipment> Equipment;
    public ComponentArray<Player> Player;
  }

  [Inject] private Data data;

  protected override void OnUpdate() {
    for (int i = 0; i != data.Length; i++) {
      var equip = data.Equipment[i];
      var player = data.Player[i];

      // Cache the current count
      int length = equip.EquipQueue.Count;
      if (length == 0) continue;

      for (int j = 0; j < length; j++) {
        // For each item in the EquipQueue
        // Do some logic
        KeyValuePair<EquipmentSlot, Item> current = equip.EquipQueue.ElementAt(j);
        EquipmentSlot slot = current.Key;
        Item item = current.Value;

        Debug.Log("Placing " + item.slug + " onto Slot: " + slot);
      }
    } 
  }
}