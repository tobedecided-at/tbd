using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TBD.Networking;

[RequireComponent(typeof(NetworkIdentity))]
public class NetworkComponentDisabler : MonoBehaviour {

	public List<Behaviour> ComponentsToDisable;
	public List<GameObject> GameObjectsToDisable;
	NetworkIdentity ni;

	private void Start() {
		ni = GetComponent<NetworkIdentity>();

		if (ComponentsToDisable.Count <= 0) return;
		if (GameObjectsToDisable.Count <= 0) return;

		// If we are the local player, dont do anything
		if (ni.LocalPlayerAuthority) return;

		foreach (var a in GameObjectsToDisable) {
			Destroy(a);
		}
		foreach (var b in ComponentsToDisable) {
			Destroy(b);
		}

		// Disable ourself
		Destroy(this);
	}

}
