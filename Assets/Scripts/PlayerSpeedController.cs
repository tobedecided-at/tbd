using UnityEngine;

public class PlayerSpeedController : MonoBehaviour {
  public float Speed;
  public float SpeedIncrease;

  void Setup() {
    Speed = TBDBootstrap.Settings.PlayerSpeed;
    SpeedIncrease = 0f;
  }
}