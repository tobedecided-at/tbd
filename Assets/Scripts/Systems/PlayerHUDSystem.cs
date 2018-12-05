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
      var tCompassDir = inventoryUI.tCompassDir;

      inventoryUI.sHealth.maxValue = cHealth.max;
      inventoryUI.sHealth.value = cHealth.value;
      inventoryUI.tHealth.text = cHealth.value.ToString();

      inventoryUI.sArmor.maxValue = cArmor.max;
      inventoryUI.sArmor.value = cArmor.value;
      inventoryUI.tArmor.text = cArmor.value.ToString();

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