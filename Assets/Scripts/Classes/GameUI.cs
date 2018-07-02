using UnityEngine;
using Unity.Entities;

public class GameUI : MonoBehaviour {

  public GameObject pauseUi;

  [Header("Parts")]
  public GameObject pause;
  public GameObject settings;

  public static GameUI instance;

  public bool isPaused;
  public bool added;

	void Start () {
    if (instance != null && instance != this)
      Destroy(this);
    instance = this;

    pauseUi.SetActive(false);
    Disable();
	}

  void Update() {
    Cursor.lockState = (isPaused) ? CursorLockMode.None : CursorLockMode.Locked;
    Cursor.visible = (isPaused) ? true: false;
  }
	
	void Disable() {
    pause.SetActive(false);
    settings.SetActive(false);
  }

  void Reset() {
    Disable();
    pause.SetActive(pauseUi.activeSelf);
  }

  public void OnPauseButton() {
    pauseUi.SetActive(!pauseUi.activeSelf);
    pause.SetActive(pauseUi.activeSelf);
    isPaused = pauseUi.activeSelf;
  }

  public void OnSettingsButton() {
    Disable();
    settings.SetActive(!settings.activeSelf);
  }

  public void OnQuitButton() {
    Application.Quit();
  }

  public void OnBackButton() {
    Reset();
  }
}
