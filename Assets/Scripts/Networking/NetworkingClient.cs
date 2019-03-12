using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using System.Net.Sockets;
using System.IO;

using UnityEngine;

using TBD.WS;
using TBD.IO;
using TBD.Client.WSBehaviorsClient;

namespace TBD.Networking {
  public static class NetworkingClient {

    public delegate void OnClientStateChange(bool state);
    public static OnClientStateChange onClientStateChange;

    public static bool bReady = false;
		public static bool BClientConnected {
			get { return bClientConnected; }
			set {
				if (bClientConnected != value) {
					bClientConnected = value;
					onClientStateChange?.Invoke(value);
				}
			}
		}

		static Client_Authenticate authSocket;
		static Client_TBDGame gameSocket;

		static string token;
    static bool bClientConnected = false;

    public static async Task<string> Init(string _ip) {

			TBDNetworking.OfflineMode = false;

			if (bClientConnected) {
				authSocket?.Close();
				gameSocket?.Close();
			}

      if (!IsHostUp(_ip)) {
        Debug.LogWarning("Cannot connect to Server!");
        return "ERROR_SERVER_UNREACHABLE";
      }

			authSocket = new Client_Authenticate(_ip, Settings.SERVER_PORT);
			BClientConnected = true;

			token = await authSocket.AuthToken.Task.ConfigureAwait(false);

      // Connect to /game endpoint
			gameSocket = new Client_TBDGame(_ip, Settings.SERVER_PORT, token);
			return "SUCCESS";
		}

		public static bool IsHostUp(string _ip) {
      using(TcpClient tcpClient = new TcpClient()) {
        try {
          tcpClient.Connect(_ip, Settings.SERVER_PORT);
        } catch (Exception) {
          return false;
        }
      }
      return true;
    }

		public static void Reset() {
			authSocket?.Close();
			gameSocket?.Close();

			BClientConnected = false;
			TBDNetworking.OfflineMode = true;
		}
  }
}