using UnityEngine;
using Unity.Entities;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour {

  public GameObject pauseUi;

  [Header("Parts")]
  public GameObject pause;
  public GameObject settings;
  public GameObject shadeBlur;

  public bool isPaused;

	void Start () {
    pauseUi.SetActive(false);
    DisableAll();
	}

  void SetLockMode(bool mode) {
    Cursor.lockState = (mode) ? CursorLockMode.Locked : CursorLockMode.None;
    Cursor.visible = !mode;
  }
	
	void DisableAll() {
    pause.SetActive(false);
    settings.SetActive(false);
    shadeBlur.SetActive(false);
  }

  void Reset() {
    pause.SetActive(false);
    settings.SetActive(false);
  }

  void TogglePause() {
    Reset();

    var bPauseUIActive = pauseUi.activeSelf;
    pauseUi.SetActive(!bPauseUIActive);
    pause.SetActive(!bPauseUIActive);
    shadeBlur.SetActive(!bPauseUIActive);
    isPaused = !bPauseUIActive;
    
    SetLockMode(!isPaused);
  }

  public void OnPauseButton() {
    TogglePause();
  }

  public void OnSettingsButton() {
    Reset();
    settings.SetActive(true);
  }

  public void OnQuitButton() {
    SceneManager.LoadScene("Main");
  }

  public void OnBackButton() {
    Reset();
    pause.SetActive(true);
  }
}
