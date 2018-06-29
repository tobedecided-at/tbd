using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

public class PlayerMovementSystem : ComponentSystem {
  public struct Data {
    public int Length;
    public GameObjectArray GameObject;
    public ComponentArray<Player> player;
    public ComponentArray<PlayerInput> pi;

    public SubtractiveComponent<Dead> Dead;
  }

  [Inject] private Data data;

  protected override void OnUpdate() {
    for (int i = 0; i != data.Length; i++) {

      var dt = Time.deltaTime;
      var rb = data.GameObject[i].GetComponentInChildren<Rigidbody>();
      var cam = data.GameObject[i].GetComponentInChildren<Camera>();

      var move = data.pi[i].move;
      var jump = data.pi[i].jump;
      var rot = data.pi[i].rotate;
      var rbRot = rb.rotation;

      var pSpd = TBDBootstrap.Settings.PlayerSpeed;
      var xRot = rot.y;
      var yRot = rot.x;

      var transform = data.GameObject[i].transform;

      if (jump)
        Jump(rb);

      var mHor = transform.right * move.x;
      var mVert = transform.forward * move.y;
      var velocity = (mHor + mVert).normalized * pSpd;

      if (velocity != Vector3.zero) {
        rb.MovePosition(rb.position + velocity * dt);
      }
      if (cam != null) {
        var cLimit = TBDBootstrap.Settings.CameraRotateLimit;

        Vector3 camRot = new Vector3(-xRot, 0f, 0f) * TBDBootstrap.Settings.CameraSensitivity;
        // float clamp = Mathf.Clamp(camRot.x, -cLimit, cLimit);

        // Debug.Log($"CamRot: {cam.transform.rotation.x}");
        // Debug.Log($"ClampRot: {clamp}");

        cam.transform.Rotate(camRot);

      }

      Vector3 rotation = new Vector3(0f, yRot, 0f) * TBDBootstrap.Settings.CameraSensitivity;
      rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));

    }
  }

  void Jump(Rigidbody _rb) {
    _rb.AddForce(new Vector3(0, TBDBootstrap.Settings.PlayerJumpPower, 0), ForceMode.Impulse);
  }
}