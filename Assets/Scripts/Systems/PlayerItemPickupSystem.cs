using UnityEngine;
using Unity.Entities;

public class PlayerItemPickupSystem : ComponentSystem {
  struct Data {
    public Player player;
    public PlayerInventory piInventory;
    public PlayerInput pi;
    public Transform t;
  }

  protected override void OnUpdate() {
    RaycastHit hit;
    float distance = TBDBootstrap.Settings.BasePickupRange * TBDBootstrap.Settings.MultPickupRange;

    foreach (var e in GetEntities<Data>()) {
      // Item has been picked up and button is held
      if (e.pi.pickedUp && e.pi.btnPickup) continue;

      // Item has been picked up, button released
      if (e.pi.pickedUp && !e.pi.btnPickup) {
        e.pi.pickedUp = false;
        continue;
      }

      if (!e.pi.btnPickup) continue;

      var go = e.t.gameObject;
      var player = e.player;
      var pi = e.pi;
      var cam = go.GetComponentInChildren<Camera>().gameObject;
      var gPlayerInv = e.piInventory;

      if (cam == null)
        continue;

      pi.pickedUp = true;
      
      if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, distance)) {
        if (hit.transform.gameObject.tag == TBDBootstrap.Settings.ItemPickupTag) {
          var i = hit.transform.GetComponent<ItemComponent>();
          if (i == null)
            return;
          i.item.specific.inventory = e.piInventory;
          if (Vector3.Distance(hit.transform.position, go.transform.position) <= i.item.pickupRange) {
            OnPickup(hit.transform.gameObject, gPlayerInv);
          }
        }
      }
    }
  }

  void OnPickup(GameObject item, PlayerInventory inv) {
    if (item.tag == TBDBootstrap.Settings.ItemPickupTag) {
      inv.OnPickup(item);
    }
  }
}