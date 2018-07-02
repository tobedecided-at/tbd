using UnityEngine;
using Unity.Entities;

public class GameUI : MonoBehaviour {

  public GameObject pauseUi;
  public static GameUI instance;

  public bool isPaused;
  public bool added;

	// Use this for initialization
	void Start () {
    if (instance != null && instance != this)
      Destroy(this);
    instance = this;

	}
	
	// Update is called once per frame
	void Update () {
		
	}

  public void OnPauseButton() {
    pauseUi.SetActive(!pauseUi.activeSelf);
    isPaused = pauseUi.activeSelf;
  }
}
