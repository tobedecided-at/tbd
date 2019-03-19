using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using Unity.Entities;

using TBD.WS;
using TBD.Client.WSBehaviorsClient;

namespace TBD.Networking {

	[RequireComponent(typeof(GameObjectEntity))]
	public class NetworkIdentity : MonoBehaviour {

		public bool ServerOnly;
		public bool LocalPlayerAuthority;

		// Networked object ID; is the same on all clients
		[HideInInspector]
		public string ID;
		[HideInInspector]
		public bool Sync = true;
	}
}
