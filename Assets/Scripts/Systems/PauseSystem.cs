using UnityEngine;
using Unity.Entities;

public class PauseSystem : ComponentSystem {

  struct Data {
    public readonly int Length;
    public GameObjectArray go;
    public ComponentArray<PlayerInput> pi;
    public ComponentArray<Transform> tr;
  }

  [Inject] Data data;

  protected override void OnUpdate() {
    PauseUI pauseUI = TBDBootstrap.Settings.UI.GetComponent<PauseUI>();
    for (int i = 0; i != data.Length; i++) {
      bool pauseBtn = data.pi[i].btnPause;

      // If pauseBtn is down
      if (pauseBtn) {
        Debug.Log("Run");
        pauseUI.OnPauseButton();
      }

      if (pauseUI.isPaused) {
        var paused = data.go[i].GetComponent<Paused>();
        paused.flag = true;
      } else {
        var paused = data.go[i].GetComponent<Paused>();
        paused.flag = false;
      }
    }
  }

}