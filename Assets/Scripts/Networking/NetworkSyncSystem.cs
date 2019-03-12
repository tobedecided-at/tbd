using System.Collections.Generic;

using UnityEngine;
using Unity.Entities;

using TBD.WS;
using TBD.Networking;
using TBD.Client.WSBehaviorsClient;
using Newtonsoft.Json;

public class NetworkSyncSystem : ComponentSystem {
	public struct Data {
		public readonly int Length;
		public GameObjectArray GameObject;
		public ComponentArray<NetworkIdentity> NetworkIdentity;
		public ComponentArray<NetworkTransform> NetworkTransform;
	}

	[Inject] private Data data;

	protected override void OnUpdate() {

		Dictionary<string, object> list = new Dictionary<string, object>();
		for (int i = 0; i != data.Length; i++) {
			var id = data.NetworkIdentity[i];
			var transform = data.NetworkTransform[i];

			// Only send info about LocalPlayer
			if (id.ID != TBDNetworking.LocalPlayerData.id) continue;

			// If we should not sync this object, skip
			if (transform.TransformSyncMethod == TransformSyncMethod.SyncNone) continue;

			var toSync = new Dictionary<string, object>();

			switch (transform.TransformSyncMethod) {
				case TransformSyncMethod.SyncTransform:
					SVector3 sv3 = new SVector3(id.transform.position.x, id.transform.position.y, id.transform.position.z);
					toSync.Add("Position", JsonConvert.SerializeObject(sv3));
					break;
				case TransformSyncMethod.SyncRigidbody3D:
					var rb = id.GetComponent<Rigidbody>();
					SVector3 sv = new SVector3(id.transform.position.x, id.transform.position.y, id.transform.position.z);
					toSync.Add("Position", JsonConvert.SerializeObject(sv));
					toSync.Add("Rigidbody3D", JsonConvert.SerializeObject(rb));
					break;
			}
			switch (transform.RotationAxisSyncMethod) {
				case RotationAxisSyncMethod.None: break;
				case RotationAxisSyncMethod.XYZ:
					SQuaternion sq = new SQuaternion(id.transform.rotation.x, id.transform.rotation.y, id.transform.rotation.z, id.transform.rotation.w);
					toSync.Add("Rotation", JsonConvert.SerializeObject(sq));
					break;
			}

			var msg = new WSMessage(WSMessageType.NetworkSyncUpdate, toSync);
			Client_TBDGame.instance.Ws.Send(msg);
		}
	}
}