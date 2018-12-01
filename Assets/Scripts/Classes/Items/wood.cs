using UnityEngine;
using System.Collections.Generic;

namespace TBD.Items {
  public class wood : IItem {
    public Item data {get; set;}
    public PlayerInventory inventory {get; set;}

    public override void OnPickup() {
      Debug.Log("Picked up " + data.slug);
    }

    public override void OnUse() {}
    public override void OnAction() {}
  }
}