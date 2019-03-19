using System;
using System.Collections.Generic;

using UnityEngine;

using TBD.WS;
using TBD.Server;
using TBD.Server.Behavior;

namespace TBD.Networking {
  public static class NetworkingServer {

    public delegate void OnServerStateChange(bool state);
    public static OnServerStateChange onServerStateChange;

		public static bool bServerRunning;

    public static bool bReady = false;

    static WSServer wss;

    public static void Init() {
      wss = new WSServer(Settings.SERVER_PORT, false);
      wss.GetServer().AddWebSocketService<Server_Authenticate>("/authenticate");
      wss.GetServer().AddWebSocketService<Server_TBDGame>("/game");

      bReady = true;
    }

    public static void Start() {
			Debug.Log("Start");
      if (!bReady) Init();

      wss.Start();
      bServerRunning = true;
		}

    public static void Stop() {
      if (wss == null || !wss.GetServer().IsListening) {
        Debug.LogWarning("Cannot stop TBDNetworkingService because it is not running");
        return;
      }

      bReady = false;
      wss.Stop();
      wss = null;
		}
  }
}