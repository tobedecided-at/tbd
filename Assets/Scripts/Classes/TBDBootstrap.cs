using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

public sealed class TBDBootstrap : MonoBehaviour {
  public static TBDSettings Settings;

  public static void NewGame() {
    var entityManager = World.Active.GetExistingManager<EntityManager>();
    var entity = SpawnPlayer(entityManager);

    // Settings.GameUi = GameObject.Find("GameUi").GetComponent<GameUi>();
  }

  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
  public static void InitializeWithScene() {
    var settings = GameObject.Find("Settings");
    Settings = settings?.GetComponent<TBDSettings>();
    if (!Settings) return;
  }

  static Entity SpawnPlayer(EntityManager _em) {
    var pSPL = Settings.PlayerSpawnPos;
    
    if (Settings.PlayerSpawnPos[0] == null) {
      Debug.LogWarning("No SpawnPos defined!");
      return new Entity();
    }

    var pSP = Settings.PlayerSpawnPos[0].transform;

    if (Settings.UseRandomSpawn)
      pSP = Settings.PlayerSpawnPos[Random.Range(0, Settings.PlayerSpawnPos.Count)].transform;
    
    var player = Object.Instantiate(Settings.PlayerPrefab, pSP.position, pSP.rotation);
    var entity = player.GetComponent<GameObjectEntity>().Entity;

    return entity;
  }
}