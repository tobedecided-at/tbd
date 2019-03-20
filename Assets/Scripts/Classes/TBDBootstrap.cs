using Unity.Entities;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

using TBD.Networking;

public class TBDBootstrap : MonoBehaviour {
  public static TBDSettings Settings;
	public static EntityManager EntityManager;

  void Start() {
    Settings = GetComponent<TBDSettings>();
		EntityManager = World.Active.GetExistingManager<EntityManager>();

		if (Settings == null)
      Debug.LogError("No TBDSettings component found on "+gameObject.name+" !");
  }

	public static void NewGame(GameObject prefab) {
		Transform sp = Settings.PlayerSpawnPos[0];

		var player = Instantiate(prefab, sp.transform.position, sp.transform.rotation);
		var entity = player.GetComponent<GameObjectEntity>().Entity;

		player.GetComponent<Health>().max = Settings.BaseHealth;
		player.GetComponent<Health>().value = Settings.BaseHealth;
		player.GetComponent<Armor>().max = Settings.MaxArmor;

		player.name = "P: LOCAL";

		NetworkIdentity ni = player.GetComponent<NetworkIdentity>();
		ni.ID = "LOCAL";
		ni.LocalPlayerAuthority = true;

		try {
			var endlessTerrain = GameObject.Find("MapGenerator").GetComponent<EndlessTerrain>();
			if (endlessTerrain != null)
				endlessTerrain.player = player.transform;
		} catch { };

		Settings.LocalPlayer = player;
	}

	public static GameObject SpawnNetworkPlayer(string index, GameObject prefab) {
		List<Transform> pSPL = Settings.PlayerSpawnPos;
		Transform pSP = pSPL[0].transform;

		if (Settings.UseRandomSpawn)
			pSP = pSPL[Random.Range(0, pSPL.Count)].transform;

		var player = Instantiate(prefab, pSP.position, pSP.rotation);
		var entity = player.GetComponent<GameObjectEntity>().Entity;
		player.GetComponent<Health>().max = Settings.BaseHealth;
		player.GetComponent<Health>().value = Settings.BaseHealth;
		player.GetComponent<Armor>().max = Settings.MaxArmor;

		NetworkIdentity ni = player.GetComponent<NetworkIdentity>();
		NetworkTransform nt = player.GetComponent<NetworkTransform>();
		ni.ID = index;

		// If the player that we spawned has the same ID as LocalPlayerData
		// We are the local player
		if (index == TBDNetworking.LocalPlayerData.id) {
			ni.LocalPlayerAuthority = true;
			nt.TransformSyncMethod = TransformSyncMethod.SyncRigidbody3D;
		} else ni.ServerOnly = true;

		player.name = "P: " + index;

		try {
			var endlessTerrain = GameObject.Find("MapGenerator").GetComponent<EndlessTerrain>();
			if (endlessTerrain != null)
				endlessTerrain.player = player.transform;
		} catch {};

		Settings.LocalPlayer = player;
		return player;
	}
}