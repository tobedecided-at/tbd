using Unity.Entities;
using UnityEngine;

public class ItemSystem : ComponentSystem {
  struct Data {
    public ItemComponent item;
    public Transform tr;
  }

  protected override void OnUpdate() {
    foreach (var e in GetEntities<Data>()) {
      var i = e.item;
      if (e.tr.gameObject.tag == TBDBootstrap.Settings.ItemPickupTag)
        continue;
      e.tr.gameObject.tag = TBDBootstrap.Settings.ItemPickupTag;
    }
  }
}