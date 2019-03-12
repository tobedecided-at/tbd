using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TBD.Networking;

[RequireComponent(typeof(NetworkIdentity))]
public class NetworkGameObjectDisabler : MonoBehaviour {

	public List<GameObject> ToDisable;
	NetworkIdentity ni;

	private void Start() {
		ni = GetComponent<NetworkIdentity>();

		if (ToDisable.Count <= 0) return;

		// If we are the local player, dont do anything
		if (ni.LocalPlayerAuthority) return;

		foreach (var b in ToDisable) {
			Destroy(b);
		}

		// Disable ourself
		Destroy(this);
	}

}
