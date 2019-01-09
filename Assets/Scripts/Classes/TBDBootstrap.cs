using Unity.Entities;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

public class TBDBootstrap : MonoBehaviour {
  public static TBDSettings Settings;

  void Start() {
    TBDBootstrap.Settings = GetComponent<TBDSettings>();
    if (TBDBootstrap.Settings == null)
      Debug.LogError("No TBDSettings component found on "+gameObject.name+" !");
  }

  public static void NewGame() {
    var entityManager = World.Active.GetExistingManager<EntityManager>();
    var entity = SpawnPlayer(entityManager);
  }

  static Entity SpawnPlayer(EntityManager _em) {
    List<Transform> pSPL = Settings.PlayerSpawnPos;
    Transform pSP = pSPL[0].transform;

    if (Settings.UseRandomSpawn)
      pSP = pSPL[Random.Range(0, pSPL.Count)].transform;
    
    var player = Object.Instantiate(Settings.PlayerPrefab, pSP.position, pSP.rotation);
    var entity = player.GetComponent<GameObjectEntity>().Entity;
    player.GetComponent<Health>().max = Settings.BaseHealth;
    player.GetComponent<Health>().value = Settings.BaseHealth;
    player.GetComponent<Armor>().max = Settings.MaxArmor;

    try {
      var endlessTerrain = GameObject.Find("MapGenerator").GetComponent<EndlessTerrain>();
      if (endlessTerrain != null)
        endlessTerrain.player = player.transform;
    } catch {
      Debug.LogWarning("No MapGenerator found");
    };
    
    Settings.Globals.goPlayer = player;
    return entity;
  }
}