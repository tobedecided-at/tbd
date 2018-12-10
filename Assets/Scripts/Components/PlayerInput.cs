using Unity.Mathematics;
using UnityEngine;

public class PlayerInput : MonoBehaviour {
  public float2 move;
  public float2 rotate;
  public bool jump;
  public bool isOnGround;
  public bool pickedUp;

  public bool btnPickup;
  public bool btnInventory;
  public bool btnPause;
  public int iHotbarScrollDir;
  public bool noClip;
  public bool walk;
}