using System;
using System.Collections.Generic;

using UnityEngine;

using TBD.WS;
using TBD.Server;
using TBD.Server.Behavior;

namespace TBD.Networking {
  public static class Service {

    public delegate void OnServerStateChange(bool state);
    public static OnServerStateChange onServerStateChange;

    public static bool bReady = false;

    static WSServer wss;
    static bool bServerRunning = false;

    public static void Init() {
      wss = new WSServer(710, false);
      // wss.GetServer().AddWebSocketService<B_Echo>("/echo");
      wss.GetServer().AddWebSocketService<B_Auth>("/auth");
      wss.GetServer().AddWebSocketService<B_TBDGame>("/tbd_game");

      bReady = true;
    }

    public static void Start() {
      if (!bReady) return;

      wss.Start();
      bServerRunning = true;

      if (onServerStateChange != null)
        onServerStateChange(true);
    }

    public static void Stop() {
      if (wss != null)
        wss.Stop();
    }
  }
}