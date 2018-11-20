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

  TBDSceneManager sceneManager;

  protected override void OnUpdate() {
    for (int i = 0; i != data.Length; i++) {
      var cHealth = data.trPlayer[i].GetComponent<Health>();
      var cArmor = data.trPlayer[i].GetComponent<Armor>();

      sceneManager = GameObject.FindWithTag("TBDSceneManager").GetComponent<TBDSceneManager>();

      var cDamaged = data.cDamaged[i];

      if (Input.GetKeyDown(KeyCode.F11)) {
        cDamaged.hit.Add(new DamageInfo("BECAUSE", "SELF", 10));
      }
      // TODO: Take care of healing first because of fairness

      // Take care of damage
      for (int c = 0; c < cDamaged.hit.Count; c++) {
        var damageToPlayer = cDamaged.hit[c].damageAmount - cArmor.value;
        
        // If the remaining damage is <= 0 then we have blocked 100%
        if (damageToPlayer <= 0) {
          cArmor.value -= cDamaged.hit[c].damageAmount;
          continue;
        }
        
        if (cArmor.value < 0) {
          cArmor.value = 0;
        }

        // DoDamage
        var damageAmount = (cHealth.value - cDamaged.hit[c].damageAmount <= 0) ? -1 : cDamaged.hit[c].damageAmount;
        // If damageAmount == false, we are dead
        if (damageAmount == -1) {
          sceneManager.LoadScene("Main");
        } else {
          cHealth.value -= damageAmount;
        }
      }
      cDamaged.hit.Clear();
    }
  }
}