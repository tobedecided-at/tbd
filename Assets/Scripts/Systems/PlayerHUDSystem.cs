using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

using TBD.Items;

public class PlayerHUDSystem : ComponentSystem {

  bool run = false;
  
  public struct Data {
    public readonly int Length;
    public ComponentArray<HUD> gPlayerHUD;
    public ComponentArray<Transform> trPlayer;
  }

  [Inject] private Data data;

  protected override void OnUpdate() {
    HUD hud = TBDBootstrap.Settings.UI.GetComponent<HUD>();
    
    for (int i = 0; i != data.Length; i++) {
      var gPlayerHUD = data.gPlayerHUD[i];
      var trPlayer = data.trPlayer[i];

      var cHealth = trPlayer.GetComponent<Health>();
      var cArmor = trPlayer.GetComponent<Armor>();
      var rImgCompass = hud.rImgCompass;

      hud.sHealth.maxValue = cHealth.max;
      hud.sHealth.value = cHealth.value;
      hud.tHealth.text = cHealth.value.ToString();

      hud.sArmor.maxValue = cArmor.max;
      hud.sArmor.value = cArmor.value;
      hud.tArmor.text = cArmor.value.ToString();

      rImgCompass.uvRect = new Rect(trPlayer.localEulerAngles.y / 360f, 0, 1, 1);
    }
  }
}