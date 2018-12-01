using UnityEngine;
using System.Collections.Generic;

namespace TBD.Items {
  public class test : Item {
    public Item data;

    public void OnPickup() {
      Debug.Log("Picked up " + data.slug + " and applied 5 Health");
    }
  }
}