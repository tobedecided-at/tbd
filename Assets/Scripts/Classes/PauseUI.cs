using UnityEngine;
using Unity.Entities;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour {

  public GameObject pauseUi;

  [Header("Parts")]
  public GameObject pause;
  public GameObject settings;

  public bool isPaused;
  public bool added;

	void Start () {
    pauseUi.SetActive(false);
    Disable();
	}

  void SetLockMode(bool mode) {
    Cursor.lockState = (mode) ? CursorLockMode.Locked : CursorLockMode.None;
    Cursor.visible = !mode;
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
    SetLockMode(!isPaused);
  }

  public void OnSettingsButton() {
    Disable();
    settings.SetActive(!settings.activeSelf);
  }

  public void OnQuitButton() {
    SceneManager.LoadScene("Main");
  }

  public void OnBackButton() {
    Reset();
  }
}
