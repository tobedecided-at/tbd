using System;
using System.Collections.Generic;

using UnityEngine;

namespace TBD.Networking {
  public class TBDNetworkingClient : MonoBehaviour {
    public void Connect() {
      NetworkingClient.Init("127.0.0.1");
    }
  }
}
