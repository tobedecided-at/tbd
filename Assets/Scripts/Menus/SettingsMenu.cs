using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour {

  public AudioMixer mixer;
  public Dropdown dd_res;
  public TMP_Text tVolumePercent;
  
  Resolution[] resolutions;
  List<string> options = new List<string>();

  void Start() {

		if (dd_res == null) return;
		resolutions = Screen.resolutions;

    dd_res.ClearOptions();
		options.Clear();
    
    int currentRes = 0;
    int i = 0;

		foreach (var entry in resolutions) {
      string option = entry.width + "x" + entry.height;
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
    Screen.SetResolution(res.width, res.height, Screen.fullScreen);
  }

  void SetVolume(float volume) {
    float iAdjusted = Mathf.RoundToInt(volume + 80);
    float sPercentage = (iAdjusted * 100) / 80;

		if (tVolumePercent != null)
			tVolumePercent.text = Mathf.RoundToInt(sPercentage).ToString() + "%";

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
