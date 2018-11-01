using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class PlayerInputSystem : ComponentSystem {
  public struct Data {
    public int Length;
    public ComponentArray<PlayerInput> PlayerInput;
    public SubtractiveComponent<Dead> Dead;
  }

  [Inject] private Data data;

  protected override void OnUpdate() {
    for (int i = 0; i != data.Length; i++) {
      var pi = data.PlayerInput[i];

      pi.move = new float2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
      pi.rotate = new float2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
      pi.jump = Input.GetButtonDown("Jump");
      pi.walk = Input.GetButtonDown("Walk");
      pi.pickupBtn = Input.GetButtonDown("Pay Respect");
      pi.pauseBtn = Input.GetButtonDown("Cancel");
    } 
  }
}