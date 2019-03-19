using System;
using System.Collections.Generic;

using TBD.WS;
using TBD.Server;
using TBD.Server.Behavior;

using System.Threading;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace TBD.Networking {
  public static class NetworkingServer {

		static bool bServerRunning;
    static WSServer wss;

    public delegate void OnServerStateChange(bool state);
    public static OnServerStateChange onServerStateChange;


    public static bool bReady = false;
		public static bool BServerRunning {
			get { return bServerRunning; }
			set {
				if (bServerRunning != value) {
					bServerRunning = value;
					onServerStateChange?.Invoke(value);
				}
			}
		}


    public static void Init() {
      wss = new WSServer(42220, false);
      wss.GetServer().AddWebSocketService<Server_Authenticate>("/authenticate");
      wss.GetServer().AddWebSocketService<Server_TBDGame>("/game");

			// Make sure that we have a new secret key for the session
			var cryptoProvider = new RNGCryptoServiceProvider();
			cryptoProvider.GetBytes(Crypto.JWToken.secret);

			Task.Run(delegate { Server_TBDGame.Update(); });

			bReady = true;
    }

    public static void Start() {
			Console.WriteLine("Starting Server");
      if (!bReady) Init();

      wss.Start();
      BServerRunning = true;
		}

    public static void Stop() {
      if (wss == null) {
        Console.WriteLine("Cannot stop Server because he is not running");
        return;
      }

      bReady = false;
      wss.Stop();
      wss = null;
		}
  }
}