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

      var cHealth = trPlayer.GetComponent<Health>();
      var cArmor = trPlayer.GetComponent<Armor>();
      var rImgCompass = inventoryUI.rImgCompass;

      inventoryUI.sHealth.maxValue = cHealth.max;
      inventoryUI.sHealth.value = cHealth.value;
      inventoryUI.tHealth.text = cHealth.value.ToString();

      inventoryUI.sArmor.maxValue = cArmor.max;
      inventoryUI.sArmor.value = cArmor.value;
      inventoryUI.tArmor.text = cArmor.value.ToString();

      rImgCompass.uvRect = new Rect(trPlayer.localEulerAngles.y / 360f, 0, 1, 1);
    }
  }
}