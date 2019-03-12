using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;

using WebSocketSharp;
using WebSocketSharp.Server;

using Newtonsoft.Json.Linq;

using TBD.WS;
using TBD.Crypto;
using TBD.Networking;

using Newtonsoft.Json;
using System.Threading;
using System.Diagnostics;

namespace TBD.Server.Behavior {

	public class Server_TBDGame : WebSocketBehavior {

		const double UpdateRate = 64f;

		static Dictionary<string, WebSocketBehavior> WebSocketClients = new Dictionary<string, WebSocketBehavior>();
		static Dictionary<string, WSData> Clients = new Dictionary<string, WSData>();
		volatile static Queue<WSData> UpdateQueue = new Queue<WSData>();
		static int countOpenSocket = 0;

		WebSocket client;
		WSData Client;
		MessageHandler Handler;

		void Send(WSMessage msg) {
			if (client.ReadyState == WebSocketState.Open)
				client.Send(msg);
		}

		#region Commands

		string OnNetworkSyncUpdate(int l, string v) {
			JObject client = v.toJObject();

			if (client["Position"] != null) {
				Client.Position = JsonConvert.DeserializeObject<SVector3>((string)client["Position"]);
			}

			if (client["Rotation"] != null) {
				Client.Rotation = JsonConvert.DeserializeObject<SQuaternion>((string)client["Rotation"]);
			}

			lock (UpdateQueue) { UpdateQueue.Enqueue(Client); }

			return "SUCCESS";
		}

		// Called by response from client that he has finished loading the scene
		string OnNetworkSceneLoaded(int l, string v) {
			JObject u = v.toJObject();

			string loadedSceneName = (string)u["Scene"];

			if (loadedSceneName == "Sandbox") {
				Client.InGame = true;
				lock (Clients) Clients[Client.id] = Client;
			}

			// Sync the client list
			Send(new WSMessage(WSMessageType.NetworkSyncClientList, new Dictionary<string, object> {
				{ "Clients", JsonConvert.SerializeObject(Clients) }
			}));
			// Tell everyone that they should spawn the new player
			SendBroadcast(new WSMessage(WSMessageType.NetworkSyncInstantiate, new Dictionary<string, object> {
					{ "Client", JsonConvert.SerializeObject(Client) },
					{ "Position", JsonConvert.SerializeObject(new SVector3(0f, 0f, 0f)) },
					{ "Rotation", JsonConvert.SerializeObject(new SQuaternion(0f, 0f, 0f, 0f)) }
				}), true);
			SendBroadcast(new WSMessage(WSMessageType.ClientWrite, new Dictionary<string, object> {
					{ "msg", "New Client connected!: " + Client.username },
				}), true);

			return "SUCCESS";
		}

		// Called on new client connecting
		string OnRcvToken(int l, string v) {
			string token = (string)v.toJObject()["token"];

			// If this is a new connection
			if (Client.jwt == null) {
				bool valid = JWToken.ValidateToken(token);
				if (valid) {
					Client.jwt = token;

					JObject decodedToken = JWToken.DecodeToken(token);
					Client.id = ID;
					Client.clearance = Int32.Parse((string)decodedToken["clearance"]);
					Client.isAuth = true;
					Client.username = (string)decodedToken["sub"];

					lock(Clients) Clients.Add(ID, Client);
				 
					// Tell the client which ID is him
					Send(new WSMessage(WSMessageType.ClientGetID, new Dictionary<string, object> {
						{ "Client", JsonConvert.SerializeObject(Client) }
					}));

					// Send the new ClientList to everyone
					SendBroadcast(new WSMessage(WSMessageType.ClientConnected, new Dictionary<string, object> {
						{ "Client", JsonConvert.SerializeObject(Client) }
					}));

					// Load Scene "Game"
					Send(new WSMessage(WSMessageType.NetworkLoadScene, new Dictionary<string, object> {
						{ "Scene", "Sandbox" },
						{ "LoadMode", 1 }
					}));

					return "SUCCESS";

				}
				return "ERROR_TOKEN_INVALID";
			}
			// TODO:
			return "ERROR_TOKEN_ALREADY_GOT";
		}

		string OnNetworkGetActiveScene(int l, string v) {
			JObject u = v.toJObject();
			Client.ActiveSceneName = (string)u["Scene"];
			return "SUCCESS";
		}

		#endregion

		// Initialize settings
		protected override void OnOpen() {
			client = Context.WebSocket;
			Handler = new MessageHandler();

			Client = new WSData();
			Client.id = ID;

			Handler.Register(WSMessageType.ClientToken, OnRcvToken);
			Handler.Register(WSMessageType.NetworkLoadSceneComplete, OnNetworkSceneLoaded);
			Handler.Register(WSMessageType.NetworkGetActiveScene, OnNetworkGetActiveScene);
			Handler.Register(WSMessageType.NetworkSyncUpdate, OnNetworkSyncUpdate);
			WebSocketClients.Add(ID, this);
			countOpenSocket++;
		}

		protected override void OnMessage(MessageEventArgs e) {

			JObject data = e.Data.toJObject();

			WSMessageType type = (WSMessageType)(int)data["type"];
			string argv = data["data"].toJsonString();

			string result = Handler.Handle(type, argv);

			// If we get an error, write to client
			if (result.Split('_')[0] == "ERROR") {
				var msg = new WSMessage(WSMessageType.ClientWrite,
					new Dictionary<string, object> {
						{ "msg", result }
					});
				Send(msg);
			}
		}

		protected override void OnError(ErrorEventArgs e) {
			OnClose(null);
			base.OnError(e);
		}

		protected override void OnClose(CloseEventArgs e) {
			WebSocketClients.Remove(ID);
			lock (Clients) Clients.Remove(ID);
			countOpenSocket--;

			SendBroadcast(new WSMessage(WSMessageType.ClientDisconnected, new Dictionary<string, object> {
					{ "Client", JsonConvert.SerializeObject(Client) }
					}),
				true);
			SendBroadcast(new WSMessage(WSMessageType.ClientWrite, new Dictionary<string, object> {
					{ "msg", "Client disconnected: " + Client.username },
				}), true);
		}


		public static void SendBroadcast(WSMessage msg, bool onlyToInGame) {
			if (!onlyToInGame) {
				SendBroadcast(msg);
				return;
			}
			lock (Clients) {
				foreach (var entry in Clients) {
					if (entry.Value.InGame) {
						if (WebSocketClients[entry.Key].Context.WebSocket.ReadyState == WebSocketState.Open)
							WebSocketClients[entry.Key].Context.WebSocket.Send(msg);
					}
				}
			}
		}

		public static void SendBroadcast(WSMessage msg) {
			lock(Clients) {
				foreach (var entry in WebSocketClients) {
					entry.Value.Context.WebSocket.Send(msg);
				}
			}
		}

		public void SendExclusiveBroadcast(WSMessage msg) {
			lock (Clients) {
				foreach (var entry in WebSocketClients) {
					if (entry.Value != this) {
						if (WebSocketClients[entry.Key].Context.WebSocket.ReadyState == WebSocketState.Open)
							entry.Value.Context.WebSocket.Send(msg);
					}
				}
			}
		}


		public static void Update() {
			Stopwatch watch = Stopwatch.StartNew();
			double start = watch.ElapsedTicks; double end = watch.ElapsedTicks;
			double diff = end - start;
			double acc_start = watch.ElapsedTicks;

			while (true) {
				start = watch.ElapsedTicks;
				diff = end - start;

				if (watch.ElapsedTicks - acc_start >= UpdateRate * 1000d) {
					OnUpdate();
					acc_start = watch.ElapsedTicks;
				}

				end = watch.ElapsedTicks;
			}
		}

		static void OnUpdate() {
			Dictionary<string, object> list = new Dictionary<string, object>();
			lock (UpdateQueue) {
				if (UpdateQueue.Count == 0) return;

				foreach (var entry in UpdateQueue) {
					if (list.TryGetValue(entry.id, out object found)) continue;
					// entry.Clean removes personal data from client
					list.Add(entry.id, entry.Clean());
				}

				UpdateQueue.Clear();
			}

			if (list.Count == 0) return;
			SendBroadcast(new WSMessage(WSMessageType.NetworkSyncUpdate, list), true);
		}
	}
}