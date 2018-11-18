using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class PlayerHUDSystem : ComponentSystem {
  public struct Data {
    public readonly int Length;
    public ComponentArray<HUD> gPlayerHUD;
    public ComponentArray<Transform> trPlayer;
  }

  [Inject] private Data data;

  protected override void OnUpdate() {
    HUD inventoryUI = TBDBootstrap.Settings.UI.GetComponent<HUD>();
    for (int i = 0; i != data.Length; i++) {
      var gPlayerHUD = data.gPlayerHUD[i];
      var trPlayer = data.trPlayer[i];

      var cHealth = trPlayer.GetComponent<Health>().value;
      var cMaxHealth = trPlayer.GetComponent<Health>().max;

      inventoryUI.sHealth.maxValue = cMaxHealth;
      inventoryUI.sHealth.value = cHealth;
      inventoryUI.tHealth.text = cHealth.ToString();
    }
  }
}