using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {

  public AudioMixer mixer;
  public Dropdown dd_res;
  
  Resolution[] resolutions;

  void Start() {
    resolutions = Screen.resolutions;

    dd_res.ClearOptions();

    List<string> options = new List<string>();
    
    int currentRes = 0;
    int i = 0;
    foreach (var entry in resolutions) {
      string option = entry.width + " x " + entry.height;
      options.Add(option);
      if (entry.width == Screen.currentResolution.width &&
          entry.height == Screen.currentResolution.height) currentRes = i;
      
      i++;
    }
    dd_res.AddOptions(options);
    dd_res.value = currentRes;
    dd_res.RefreshShownValue();
  }

  void SetResolution(int index) {
    Resolution res = resolutions[index];
    Debug.Log(res);
    Screen.SetResolution(res.width, res.height, Screen.fullScreen);
  }

  void SetVolume(float volume) {
    mixer.SetFloat("vMaster", volume);
  }

  void SetQualityByIndex(int index) {
    QualitySettings.SetQualityLevel(index);
  }

  void SetFullscreen(bool set) {
    Screen.fullScreen = set;
  }

  public void OnVolumeChanged(float amount) {
    SetVolume(amount);
  }

  public void OnQualityChanged(int index) {
    SetQualityByIndex(index);
  }

  public void OnFullscreenToggle(int index) {
    switch (index) {
      case 0:
        SetFullscreen(false);
      break;
      case 1:
        SetFullscreen(true);
      break;
    }
  }

  public void OnResolutionChange(int index) {
    SetResolution(index);
  }
}
