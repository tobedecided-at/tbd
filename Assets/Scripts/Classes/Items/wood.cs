using UnityEngine;
using System.Collections.Generic;

namespace TBD.Items {
  public class wood : Item {
    public Item data;

    public void OnPickup() {
      Debug.Log("Picked up " + data.slug);
    }
  }
}