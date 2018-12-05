using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using System.Collections.Generic;

public class PlayerHealthSystem : ComponentSystem {
  public struct Data {
    public readonly int Length;
    public ComponentArray<Transform> trPlayer;
    public ComponentArray<Player> gPlayer;
  }

  [Inject] private Data data;

  TBDSceneManager sceneManager;
  bool run = false;

  protected override void OnUpdate() {
    if (sceneManager == null)
      sceneManager = TBDBootstrap.Settings.TBDsm;

    for (int i = 0; i != data.Length; i++) {
      var cDamaged = data.trPlayer[i].GetComponent<Damaged>();
      var cHealed = data.trPlayer[i].GetComponent<Healed>();

      var cHealth = data.trPlayer[i].GetComponent<Health>();
      var cArmor = data.trPlayer[i].GetComponent<Armor>();

      if (Input.GetKeyDown(KeyCode.F11)) {
        cDamaged.hit.Add(new DamageInfo("BECAUSE", "SELF", 10));
      }
      if (Input.GetKeyDown(KeyCode.F12)) {
        cHealed.hit.Add(new HealInfo("BECAUSE", "GOD", 10));
      }

      // TODO: Take care of healing first because of fairness
      for (int h = 0; h < cHealed.hit.Count; h++) {
        var healAmount = cHealed.hit[i].healAmount;
<<<<<<< HEAD
        
=======

>>>>>>> master
        // Heal without limits
        cHealth.value += healAmount;
        // Maximize health value, better against cheating
        cHealth.value = (cHealth.value > TBDBootstrap.Settings.BaseHealth) ? TBDBootstrap.Settings.BaseHealth : cHealth.value;
      }

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
      cHealed.hit.Clear();
    }
  }
}
