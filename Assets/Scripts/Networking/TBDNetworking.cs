using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

using UnityEngine;

using TBD.WS;
using TBD.Client.WSBehaviorsClient;

using Newtonsoft.Json;
using System.Linq;

namespace TBD.Networking {
	public class TBDNetworking : MonoBehaviour {

		public static TBDNetworking instance;
		public static bool OfflineMode = true;
		public static WSData LocalPlayerData { get; private set; }

		static TBDBootstrap TBDBootstrap;
		static TBDSceneManager SceneManager;

		public GameObject PlayerPrefab;
		// Players with GameObjects in scene
		public Dictionary<string, GameObject> PlayerList = new Dictionary<string, GameObject>();
		// Connected clients with data
		public Dictionary<string, WSData> ClientList = new Dictionary<string, WSData>();
		public bool Connected = false;

		public static bool IsLocalPlayer(GameObject o) {
			NetworkIdentity ni = o.GetComponent<NetworkIdentity>();
			if (ni == null) return false;
			if (LocalPlayerData.id == ni.ID) return true;

			return false;
		}

		void Awake() {
			instance = this;
		}

		void Start() {
			Init();
		}

		void Init() {
			UnityThread.initUnityThread();

			DontDestroyOnLoad(this);
			TBDBootstrap = GetComponent<TBDBootstrap>();
			SceneManager = TBDBootstrap.Settings.SceneManager;
		}

		public void Connect() {
			NetworkingClient.Init("gaben.ddns.net");
		}

		public GameObject InstantiatePlayer(string id) {
			GameObject go = TBDBootstrap.SpawnNetworkPlayer(id, PlayerPrefab);
			PlayerList.Add(id, go);
			return go;
		}

		void OnNetworkSceneLoaded(string name) {
			var msg = new WSMessage(WSMessageType.NetworkLoadSceneComplete, new Dictionary<string, object> {
				{ "Scene", name }
			});
			Client_TBDGame.instance.Ws.Send(msg);
		}

		#region Commands

		// Destroys the GameObject with given name
		// May be abused, TODO
		public string OnNetworkSyncDestroy(int l, string v) {
			if (SceneManager.GetActiveSceneName() != "Sandbox") return "INFO_NOT_IN_SANDBOX";

			JObject u = v.toJObject();
			string id = JsonConvert.DeserializeObject<WSData>((string)u["Client"]).id;

			if (PlayerList.TryGetValue(id, out GameObject found)) {
				Destroy(found);
				PlayerList.Remove(id);

				return "SUCCESS";
			}
			return "WARN_DESTROY_TARGET_NOT_FOUND";
		}

		// Called on server message
		public string OnNetworkSyncInstantiate(int l, string v) {
			if (SceneManager.GetActiveSceneName() != "Sandbox") return "INFO_NOT_IN_SANDBOX";

			JObject u = v.toJObject();

			string id = JsonConvert.DeserializeObject<WSData>((string)u["Client"]).id;
			// If we have spawned that player already
			if (PlayerList.TryGetValue(id, out GameObject found)) return "INFO_ALREADY_SPAWNED";

			Vector3 pos = JsonConvert.DeserializeObject<Vector3>((string)u["Position"]);
			Quaternion rot = JsonConvert.DeserializeObject<Quaternion>((string)u["Rotation"]);

			InstantiatePlayer(id);
			return "SUCCESS";
		}

		// Called on server message
		public string OnNetworkGetActiveScene(int l, string v) {
			JObject u = v.toJObject();
			string scene = TBDSceneManager.instance.GetActiveSceneName();

			Client_TBDGame.instance.Ws.Send(new WSMessage(WSMessageType.NetworkGetActiveScene, new Dictionary<string, object> {
				{ "Scene", scene }
			}));

			return "SUCCESS";
		}

		// Called on server message
		public string OnNetworkLoadScene(int l, string v) {
			JObject u = v.toJObject();

			string sceneNameToLoad = (string)u["Scene"];
			int loadMode = (int)u["LoadMode"];

			TBDSceneManager.onSceneLoaded += OnNetworkSceneLoaded;
			SceneManager.LoadScene(sceneNameToLoad, (TBDSceneManager.LoadMode)loadMode);

			return "SUCCESS";
		}

		// Called on server message
		public string OnNetworkSyncUpdate(int l, string v) {

			Dictionary<string, WSData> dict = JsonConvert.DeserializeObject<Dictionary<string, WSData>>(v);
			foreach (var entry in dict) {

				// It may be that we have not spawned this player yet
				if (PlayerList.TryGetValue(entry.Value.id, out GameObject found)) {
					// TODO: Move to own system - NetworkPlayerMoveSystem.cs ?
					// If we read our own data, ignore
					if (entry.Value.id == LocalPlayerData.id) continue;

					Transform t = found.transform;
					t.position = new Vector3(entry.Value.Position.x, entry.Value.Position.y, entry.Value.Position.z);
					t.rotation = new Quaternion(entry.Value.Rotation.x, entry.Value.Rotation.y, entry.Value.Rotation.z, entry.Value.Rotation.w);
				}


			}

			return "SUCCESS";

		}

		// Called on server message
		public string OnClientWrite(int l, string v) {
			JObject u = v.toJObject();
			string msg = (string)u["msg"];
			// TODO: Make UI
			Debug.Log(msg);

			return "SUCCESS";
		}

		// Called when client gets his ID from server
		public string OnClientGetID(int l, string v) {
			JObject u = v.toJObject();
			Connected = true;

			LocalPlayerData = JsonConvert.DeserializeObject<WSData>((string)u["Client"]);
			ClientList.Add(LocalPlayerData.id, LocalPlayerData);

			return "SUCCESS";
		}

		// Called when a new player connects
		// That new player is already "InGame"
		public string OnClientConnected(int l, string v) {
			JObject u = v.toJObject();

			var data = JsonConvert.DeserializeObject<WSData>((string)u["Client"]);
			// If we registered this client already
			if (ClientList.TryGetValue(data.id, out WSData found)) {
				return "ERROR_ALREADY_REGISTERED";
			}

			ClientList.Add(data.id, data);

			return "SUCCESS";
		}

		// Called on server message
		public string OnClientDisconnected(int l, string v) {
			JObject u = v.toJObject();

			var client = JsonConvert.DeserializeObject<WSData>((string)u["Client"]);
			ClientList.Remove(client.id);

			OnNetworkSyncDestroy(l, v);
			LobbyViewManager.instance.RemoveFromView(client.id);

			return "SUCCESS";
		}

		// Receives all connected Clients to update the ListView
		public string OnNetworkSyncClientList(int l, string v) {
			JObject u = v.toJObject();

			var clients = JsonConvert.DeserializeObject<Dictionary<string, WSData>>((string)u["Clients"]);
			string loadedSceneName = TBDSceneManager.instance.GetActiveSceneName();

			ClientList.Clear();
			foreach (var client in clients) {
				ClientList.Add(client.Value.id, client.Value);
				// TODO: Cleanup
				if (loadedSceneName == "Sandbox")
					InstantiatePlayer(client.Value.id);
			}

			return "SUCCESS";
		}

		#endregion

		private void OnApplicationQuit() {
			NetworkingClient.Reset();
		}
	}
}
