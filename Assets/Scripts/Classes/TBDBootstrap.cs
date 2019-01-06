using Unity.Entities;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

public class TBDBootstrap : MonoBehaviour {
  public static TBDSettings Settings;

  public static void NewGame() {
    var entityManager = World.Active.GetExistingManager<EntityManager>();
    var entity = SpawnPlayer(entityManager);
  }

  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
  public static void Init() {
    var settings = GameObject.Find("Settings");
    if (settings)
      Settings = settings.GetComponent<TBDSettings>();
    if (!Settings) return;
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

    var endlessTerrain = GameObject.Find("MapGenerator").GetComponent<EndlessTerrain>();
    endlessTerrain.player = player.transform;
    
    return entity;
  }
}