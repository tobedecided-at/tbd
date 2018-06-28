using UnityEngine;
using Unity.Entities;

public class PlayerItemPickupSystem : ComponentSystem {
  public struct Data {
    public Player player;
    public PlayerInput pi;
    public Transform t;
  }

  protected override void OnUpdate() {
    RaycastHit hit;
    float distance = TBDBootstrap.Settings.BasePickupRange * TBDBootstrap.Settings.MultPickupRange;

    foreach (var e in GetEntities<Data>()) {

      
      // Item has been picked up and button is held
      if (e.pi.pickedUp && e.pi.pickupBtn) continue;

      // Item has been picked up, button released
      if (e.pi.pickedUp && !e.pi.pickupBtn) {
        e.pi.pickedUp = false;
        continue;
      }

      if (!e.pi.pickupBtn) continue;

      var go = e.t.gameObject;
      var player = e.player;
      var pi = e.pi;
      var cam = go.GetComponentInChildren<Camera>().gameObject;

      if (cam == null)
        continue;

      pi.pickedUp = true;
      
      Debug.DrawRay(cam.transform.position, cam.transform.forward * distance, Color.green);

      if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, distance)) {
        if (hit.transform.gameObject.tag == TBDBootstrap.Settings.ItemPickupTag) {
          OnPickup(pi, hit.transform.gameObject);
        }
      }
    }
  }

  void OnPickup(PlayerInput pi, GameObject item) {
    Debug.Log($"Picked Up: {item.name}");
  }
}