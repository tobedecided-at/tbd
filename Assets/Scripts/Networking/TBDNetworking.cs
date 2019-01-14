using System;
using System.Collections.Generic;

using UnityEngine;

namespace TBD.Networking {
  public class TBDNetworking : MonoBehaviour {

    public void StartService() {
      if (!NetworkingServer.bReady) NetworkingServer.Init();
      NetworkingServer.Start();
    }
    public void StopService() {
      NetworkingServer.Stop();
    }
  }
}
