using Unity.Mathematics;
using UnityEngine;

public class PlayerInput : MonoBehaviour {
  public float2 move;
  public float2 rotate;
  public bool jump;
  public bool isOnGround;
  public bool pickupBtn;
  public bool pickedUp;
  public bool pauseBtn;
}