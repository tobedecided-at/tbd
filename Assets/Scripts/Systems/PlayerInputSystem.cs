using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

using TBD.Networking;

public class PlayerInputSystem : ComponentSystem {
  public struct Data {
    public readonly int Length;
    public ComponentArray<PlayerInput> PlayerInput;
    public SubtractiveComponent<Dead> Dead;
		public GameObjectArray GameObject;
  }

  [Inject] private Data data;

  protected override void OnUpdate() {
    for (int i = 0; i != data.Length; i++) {
			if (!TBDNetworking.IsLocalPlayer(data.GameObject[i])) continue;

      var pi = data.PlayerInput[i];

      pi.move = new float2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
      pi.rotate = new float2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
      pi.jump = Input.GetButtonDown("Jump");
      pi.walk = Input.GetButtonDown("Walk");
      
      pi.btnPickup = Input.GetButtonDown("Pay Respect");
      pi.btnPause = Input.GetButtonDown("Cancel");
      pi.btnInventory = Input.GetButtonDown("Toggle Inventory");
      pi.iHotbarScrollDir = Input.mouseScrollDelta.y > 0f ? 1 :
                            Input.mouseScrollDelta.y < 0f ? -1 : 0;
    } 
  }
}