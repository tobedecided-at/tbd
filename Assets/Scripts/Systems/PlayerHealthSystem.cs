using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using System.Collections.Generic;

public class PlayerHealthSystem : ComponentSystem {
  public struct Data {
    public readonly int Length;
    public ComponentArray<Damaged> cDamaged;
    public ComponentArray<Transform> trPlayer;
    public ComponentArray<Player> gPlayer;
  }

  [Inject] private Data data;

  protected override void OnUpdate() {
    for (int i = 0; i != data.Length; i++) {
      var cHealth = data.trPlayer[i].GetComponent<Health>();
      var cPlayerInput = data.gPlayer[i].GetComponent<PlayerInput>();

      var cDamaged = data.cDamaged[i];

      if (cPlayerInput.btnDebugTakeDamage) {
        cDamaged.lHitDamage.Add(10);
        cDamaged.lHitType.Add("DEBUG");
        cDamaged.lHitName.Add("SELF");

      }
      // TODO: Take care of healing first because of fairness

      // Take care of damage
      for (int c = 0; c < cDamaged.lHitDamage.Count; c++) {
        // DoDamage
        var damageAmount = (cHealth.value - cDamaged.lHitDamage[c] <= 0) ? -1 : cDamaged.lHitDamage[c];
        // If damageAmount == false, we are dead
        if (damageAmount == -1) {
          Debug.LogError("Dead");
        } else {
          cHealth.value -= damageAmount;
        }
      }
      cDamaged.lHitDamage.Clear();
      cDamaged.lHitType.Clear();
      cDamaged.lHitName.Clear();
    }
  }
}