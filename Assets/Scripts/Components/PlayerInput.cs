using Unity.Mathematics;
using UnityEngine;

public class PlayerInput : MonoBehaviour {
  public float2 move;
  public float2 rotate;
  public bool jump;
  public bool isOnGround;
  public bool btnPickup;
  public bool btnInventory;
  public bool pickedUp;
  public bool btnPause;
  // HACK: Will change to gamemode later
  // TODO
  public bool noClip;
  public bool walk;
}