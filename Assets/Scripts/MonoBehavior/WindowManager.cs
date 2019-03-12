using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WindowManager : MonoBehaviour {
  
  [SerializeField]
  public List<GameObject> windows = new List<GameObject>();
	public int CurrentIndex { get; private set; }

  void Start() {
		CurrentIndex = 0;

		// Make sure that we start on the main screen
		SwitchWindow(CurrentIndex);
  }
  
  public void SetWindowActive(int index) {
    windows[index].SetActive(true);
		CurrentIndex = index;
  }

	public void SwitchWindow(int index) {
    DisableAll();
		windows[index].SetActive(true);
		CurrentIndex = index;
	}

  void DisableAll() {
    foreach (var window in windows) {
      window.SetActive(false);
    }
  }
}
