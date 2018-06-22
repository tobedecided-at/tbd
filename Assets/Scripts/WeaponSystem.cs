using Unity.Entities;
using UnityEngine;

public class WeaponSystem : ComponentSystem {

  struct Filter {
    public PlayerInput Input;
    public Transform T;
    public Weapon W;
  }
  
  protected override void OnUpdate() {

    foreach (var e in GetEntities<Filter>()) {
      var pi = e.Input;
      if (Input.GetKeyDown(KeyCode.E)) {
        pi.Equip = 1;
        
      }
      Debug.Log(pi.Equip);
      Debug.Log(e.W.Firerate);
    }

  }
}