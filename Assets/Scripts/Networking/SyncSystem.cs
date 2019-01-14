using UnityEngine;
using Unity.Entities;

using TBD.Networking;

public class SyncSystem : ComponentSystem {
  public struct Data {
    public readonly int Length;
    public GameObjectArray GameObject;
    public ComponentArray<Transform> Transform;
    public ComponentArray<SyncComponent> SyncComponent;
  }

  [Inject] private Data data;

  protected override void OnUpdate() {
    
    // Check if the Server is running
    if (!TBD.Networking.NetworkingServer.bServerRunning) return;

    for (int i = 0; i != data.Length; i++) {
      var sync = data.SyncComponent[i];
      if (!sync.bShouldSync) continue;

      var transform = data.Transform[i];
      var gameObject = data.GameObject[i];
      
      Debug.Log("Syncing: " + transform.name);
    } 
  }
}