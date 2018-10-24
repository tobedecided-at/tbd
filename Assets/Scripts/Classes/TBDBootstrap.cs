using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

public class TBDBootstrap : MonoBehaviour {
  public static TBDSettings Settings;

  public static void NewGame() {
    var entityManager = World.Active.GetExistingManager<EntityManager>();
    var entity = SpawnPlayer(entityManager);

    // Settings.GameUi = GameObject.Find("GameUi").GetComponent<GameUi>();
  }

  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
  public static void Init() {
    var settings = GameObject.Find("Settings");
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

    return entity;
  }
}