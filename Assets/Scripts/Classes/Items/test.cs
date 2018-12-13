using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace TBD.Items {
  public class test : IItem {
    public Item data {get; set;}
    public PlayerInventory inventory {get; set;}

    public override void OnEquip() {
      Player player = inventory.gameObject.GetComponentInParent<Player>();
      Health health = player.GetComponent<Health>();
      health.max += (float)data.stats["iHealth"];
    }

    public override void OnUnequip() {
      Player player = inventory.gameObject.GetComponentInParent<Player>();
      Health health = player.GetComponent<Health>();
      health.max -= (float)data.stats["iHealth"];
    }

    public override void OnAction() {}
  }
}