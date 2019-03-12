using System;
using System.Threading.Tasks;

using System.Collections.Generic;
using System.Security.Cryptography;

using WebSocketSharp;

using Newtonsoft.Json.Linq;

using TBD.WS;
using TBD.Users;
using TBD.Crypto;
using TBD.Networking;

using UnityEngine;

namespace TBD.Client.WSBehaviorsClient {
	public class Client_TBDGame : WSClient {

		public static Client_TBDGame instance;
		public new WebSocket Ws;

		public Client_TBDGame(string serverAddress, int port, string token) {
			// Close the old connection if there was one
			instance?.Close();

			instance = this;

			Data = new WSData();
			Handler = new MessageHandler();

			Data.jwt = token;

			Ws = new WebSocket("ws://" + serverAddress + ":" + port + "/game");
			IsOpen = false;

			Ws.OnOpen += (sender, e) => { OnOpen((WebSocket)sender); };
			Ws.OnMessage += (sender, e) => { OnMessage((WebSocket)sender, e); };
			Ws.OnError += (sender, e) => { base.OnError((WebSocket)sender, e); };
			Ws.OnClose += (sender, e) => { base.OnClose(e); };

			Handler.Register(WSMessageType.ClientWrite, TBDNetworking.instance.OnClientWrite);
			Handler.Register(WSMessageType.ClientGetID, TBDNetworking.instance.OnClientGetID);
			Handler.Register(WSMessageType.ClientConnected, TBDNetworking.instance.OnClientConnected);
			Handler.Register(WSMessageType.ClientDisconnected, TBDNetworking.instance.OnClientDisconnected);
			Handler.Register(WSMessageType.NetworkSyncClientList, TBDNetworking.instance.OnNetworkSyncClientList);
			Handler.Register(WSMessageType.NetworkLoadScene, TBDNetworking.instance.OnNetworkLoadScene);
			Handler.Register(WSMessageType.NetworkSyncUpdate, TBDNetworking.instance.OnNetworkSyncUpdate);
			Handler.Register(WSMessageType.NetworkSyncInstantiate, TBDNetworking.instance.OnNetworkSyncInstantiate);
			Handler.Register(WSMessageType.NetworkSyncDestroy, TBDNetworking.instance.OnNetworkSyncDestroy);

			// Connect last, else there may be a race condition
			Ws.Connect();
		}

		protected override void OnOpen(WebSocket sender) {
			IsOpen = true;

			var msg = new WSMessage(WSMessageType.ClientToken,
				new Dictionary<string, object> {
				{ "token", Data.jwt }
			});
			Ws.Send(msg);
		}

		protected override void OnMessage(WebSocket sender, MessageEventArgs e) {
			JObject data = e.Data.toJObject();

			WSMessageType type = (WSMessageType)(int)data["type"];
			string argv = data["data"].toJsonString();

			UnityThread.executeInFixedUpdate(() => {
				Handler.Handle(type, argv);
			});
		}
	}
}