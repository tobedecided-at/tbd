using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using System.Collections.Generic;

public class PlayerDealDamageSystem : ComponentSystem {
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
      var cDamaged = data.cDamaged[i];

      if (cPlayerInput.btnDebugTakeDamage) {
        cDamaged.hit.Add(new DamageInfo("DEBUG", "SELF", 10));
      }
      // TODO: Take care of healing first because of fairness

      // Take care of damage
      for (int c = 0; c < cDamaged.hit.Count; c++) {
        // DoDamage
        Debug.Log(string.Format("Took {0} damage from {1}. It felt like {2}", cDamaged.hit[c].damageAmount, cDamaged.hit[c].originName, cDamaged.hit[c].damageAmount));
        // var damageAmount = (cHealth.value - cDamaged.lHitDamage[c] <= 0) ? -1 : cDamaged.hit[c][];
        // If damageAmount == false, we are dead
        if (damageAmount == -1) {
          Debug.LogError("Dead");
        } else {
          cHealth.value -= damageAmount;
        }
      }
      cDamaged.hit.Clear();
    }
  }
}