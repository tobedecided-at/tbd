
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMoveSystem : ComponentSystem {
  public struct Filter {
      public Transform T;
      public Position3D Position;
      public PlayerInput Input;
  }

  public struct WeaponFilter {
    public Weapon weapon;
  }

  protected override void OnUpdate() {

    var settings = TBDHelper.Settings;

    float dt = Time.deltaTime;

    foreach (var e in GetEntities<Filter>()) {
      var pi = e.Input;
      var t = e.T.gameObject.transform;

      t.Translate(settings.playerMoveSpeed * pi.Move.x * Time.deltaTime, 0f, settings.playerMoveSpeed * pi.Move.y * Time.deltaTime);
      e.Position.Value = e.T.position;
    }
  }
}