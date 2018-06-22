using UnityEngine;

public sealed class TBDHelper {

  public static TBDSettings Settings;

  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
  public static void InitWithScene() {
    var settingsGO = GameObject.Find("Settings");
    Settings = settingsGO?.GetComponent<TBDSettings>();
    
    if (!Settings)
      return;

    GameObject.Instantiate(Settings.PlayerPrefab, GameObject.Find("PlayerSpawn").transform);
  }
}
