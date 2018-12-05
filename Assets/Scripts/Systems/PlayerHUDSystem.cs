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
      var tCompassDir = hud.tCompassDir;

      hud.sHealth.maxValue = cHealth.max;
      hud.sHealth.value = cHealth.value;
      hud.tHealth.text = cHealth.value.ToString();

      hud.sArmor.maxValue = cArmor.max;
      hud.sArmor.value = cArmor.value;
      hud.tArmor.text = cArmor.value.ToString();

      rImgCompass.uvRect = new Rect(trPlayer.localEulerAngles.y / 360f, 0, 1, 1);

      Vector3 forward = trPlayer.forward;
      forward.y = 0;

      float headingAngle = Quaternion.LookRotation(forward).eulerAngles.y;
      headingAngle = 5 * (Mathf.RoundToInt(headingAngle / 5.0f));

      int displayangele;
      displayangele = Mathf.RoundToInt (headingAngle);

      switch (displayangele) {
        case 0:
          tCompassDir.text = "N";
          break;
        case 360:
          tCompassDir.text = "N";
          break;
        case 45:
          tCompassDir.text = "NE";
          break;
        case 90:
          tCompassDir.text = "E";
          break;
        case 130:
          tCompassDir.text = "SE";
          break;
        case 180:
          tCompassDir.text = "S";
          break;
        case 225:
          tCompassDir.text = "SW";
          break;
        case 270:
          tCompassDir.text = "W";
          break;
        case 315:
          tCompassDir.text = "NW";
          break;
        default:
          tCompassDir.text = headingAngle.ToString ();
          break;
      }
    }
  }
}