using System.Collections.Generic;
using UnityEngine;

public class TBDSettings : MonoBehaviour {
  public GameObject LocalPlayer;
  public GameObject PlayerPrefab;
  public List<Transform> PlayerSpawnPos;

  public bool UseRandomSpawn = true;
  public string ItemPath = "/json/Items/";

  [Header("Base Options")]
  public float BaseHealth = 100f;
  public float BaseDamage = 1f;
  public float BaseFirerate = 1f;
  public float BasePickupRange = 10f;
  public float MaxArmor = 10000;
  public GameObject UI;
  public TBDSceneManager SceneManager;

  [Header("Multiplier Options")]
  public float MultDamage = 1f;
  public float MultFirerate = 1f;
  public float MultShotRange = 1f;
  public float MultPickupRange = 1f;

  [Header("Player Options")]
  public float PlayerSpeed = 6f;
  public float PlayerJumpPower = 850f;
  public float CarryWeight = 150f;
  public float PlayerSpeedReduction = .6f;
  public string ItemPickupTag = "Item";
  public int InventorySize = 30;
  public int HotbarSize = 9;
  
  [Header("Camera Options")]
  public float CameraSensitivity = 2.5f;
  public float CameraRotateLimit = 90f;

  [Header("Enemy Options")]
  public float ShootCooldown = 0.05f;
  public float ShotRange = 100f;

  public int ScorePenalty = 50;

  public float CameraSmoothing = 5f;

}
