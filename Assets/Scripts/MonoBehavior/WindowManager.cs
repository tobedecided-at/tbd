using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WindowManager : MonoBehaviour {
  
  [SerializeField]
  public List<GameObject> windows = new List<GameObject>();

  void Start() {
    // Make sure that we start on the main screen
    ShowPressAny();
  }
  
  void SetWindowActive(int index) {
    DisableAll();
    windows[index].SetActive(true);
  }

  void DisableAll() {
    foreach (var window in windows) {
      window.SetActive(false);
    }
  }

  public void ShowSettings() {
    SetWindowActive(2);
  }

  public void ShowMain() {
    SetWindowActive(1);
  }

  public void ShowPressAny() {
    SetWindowActive(0);
  }

  public void ShowLobby() {
    SetWindowActive(3);
  }

}
