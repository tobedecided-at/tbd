using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TBD.WS;
using TBD.Server;
using TBD.Server.Behavior;

namespace TBD.Networking {
  public class TBDNetworking : MonoBehaviour {

    public bool bUseNetworking = false;
    bool bServerRunning;
    
    // Called automatically by MonoBehavior
    void Start() {
      // Subscribe
      Service.onServerStateChange += OnServerStateChange;

      if (bServerRunning || !bUseNetworking) return;
      Service.Start();
    }

    public void Stop() {
      Service.Stop();
    }

    // Every frame
    void Update() {
      // TODO: Add a real switch
      if (!bUseNetworking) return;

    }

    // Gets called when the state of the server changes
    void OnServerStateChange(bool state) {
      Debug.Log("Server changed state!");
    }
  }
}
