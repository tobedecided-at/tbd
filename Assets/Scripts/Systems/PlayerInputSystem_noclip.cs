using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class PlayerInputSystem_noclip : ComponentSystem {
  public struct Data {
    public readonly int Length;
    public ComponentArray<PlayerInput> PlayerInput;
    public ComponentArray<NoClip> nc;
    public SubtractiveComponent<Dead> Dead;
  }

  [Inject] private Data data;

  protected override void OnUpdate() {
    for (int i = 0; i != data.Length; i++) {
      var pi = data.PlayerInput[i];

      pi.move = new float2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
      pi.rotate = new float2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
      pi.jump = Input.GetButtonDown("Jump");
      pi.walk = Input.GetButton("Walk");
      pi.pickupBtn = Input.GetButtonDown("Pay Respect");
      pi.pauseBtn = Input.GetButtonDown("Cancel");

      pi.noClip = true;
    } 
  }
}