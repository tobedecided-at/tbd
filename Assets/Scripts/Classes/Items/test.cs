using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace TBD.Items {
  public class test : IItem {
    public Item data {get; set;}
    public PlayerInventory inventory {get; set;}

    public override void OnPickup() {
      Player player = inventory.gameObject.GetComponentInParent<Player>();
      Healed healed = player.GetComponent<Healed>();
      healed.hit.Add(new HealInfo("HEAL", "TEST ITEM", Mathf.RoundToInt(data.stats["iHealth"])));
      Debug.Log("Picked up " + data.slug + " and applied "+ Mathf.RoundToInt(data.stats["iHealth"]) +" Health");
    }
    public override void OnUse() {}
    public override void OnAction() {}
  }
}