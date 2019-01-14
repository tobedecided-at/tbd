using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using System.Net.Sockets;

using UnityEngine;

using TBD.WS;
using TBD.Client.Behavior;

namespace TBD.Networking {
  public static class NetworkingClient {

    public delegate void OnClientStateChange(bool state);
    public static OnClientStateChange onClientStateChange;

    public static bool bServerRunning = false;
    public static bool bReady = false;

    public static void Init(string _ip) {
      if (!IsHostUp(_ip)) {
        Debug.LogWarning("Cannot connect to Server!");
        return;
      }

      var authSocket = new CBAuth("localhost", Settings.SERVER_PORT);
      
      // "Block" until we get a token
      var token = Task.Run( async () =>
        await authSocket.GetToken().ConfigureAwait(false)).Result;

      Debug.Log("Got token, connecting to TBD...");

      // Got a token, use it to connect to /tbd_game endpoint
      var gameSocket = new CTBDGame("localhost", Settings.SERVER_PORT, token);
      Debug.Log("Connected!");
    }

    public static bool IsHostUp(string ip) {
      using(TcpClient tcpClient = new TcpClient()) {
        try {
          tcpClient.Connect(ip, Settings.SERVER_PORT);
        } catch (Exception) {
          return false;
        }
      }
      return true;
    }
  }
}