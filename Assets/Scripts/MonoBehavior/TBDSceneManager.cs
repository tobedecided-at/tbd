using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.UI;

public class TBDSceneManager : MonoBehaviour {

	public delegate void OnSceneLoaded(string name);

	public static TBDSceneManager instance;
	public static OnSceneLoaded onSceneLoaded;

  public GameObject p_black;
  public GameObject frame;
  public GameObject loadingBar;
  public Animator animator;
  public Text percentage;

  public static bool created = false;

  Image i_black;
  Slider s_loading;

	public string GetActiveSceneName() {
		return SceneManager.GetActiveScene().name;
	}

  public void OnQuitButton() {
    Application.Quit(0);
  }

  void Awake() {
		DontDestroyOnLoad(this);
		// Destroy new SceneManager
		if (instance != this && instance != null)
			Destroy(this);

		instance = this; 

    if (!created) {
      created = true;
    }

    s_loading = loadingBar.GetComponent<Slider>();

    if (animator.enabled) animator.enabled = false;
    if (frame.activeSelf) frame.SetActive(false);
    if (loadingBar.activeSelf) loadingBar.SetActive(false);
  }

  void Start() {
    i_black = p_black.GetComponent<Image>();
  }

  public void LoadScene(string toLoad) {
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
    LoadScene(toLoad, LoadMode.CutFade);
  }

  public void LoadScene(string toLoad, LoadMode loadMode) {
    if (!Application.CanStreamedLevelBeLoaded(toLoad)) {
      Debug.LogError("Scene not found! " + toLoad);
      return;
    }
    
    StartCoroutine(LoadSceneCoroutine(toLoad, loadMode));
    
  }

  IEnumerator LoadSceneCoroutine(string toLoad, LoadMode loadMode = LoadMode.CutFade) {

    frame.SetActive(true);
    animator.enabled = true;
    loadingBar.SetActive(true);
    percentage.text = "0%";
    s_loading.value = 0f;

    AsyncOperation load = null;
    load = SceneManager.LoadSceneAsync(toLoad);

    float progress = 0f;
    switch (loadMode) {
      case LoadMode.CutCut:
        CutOut();
      break;
      case LoadMode.CutFade:
        CutOut();        
      break;
      case LoadMode.FadeCut:
        FadeOut();
      break;
      case LoadMode.FadeFade:
        FadeOut();
      break;
    }

    while (!load.isDone || progress < 1f) {
      progress = Mathf.Clamp01(load.progress / .9f);
      percentage.text = (progress * 100) + "%";

      s_loading.value = progress;

      if (progress >= 1f) {
        HidePercentage();
      }

      // Wait for one frame
      yield return null;
    }

    switch (loadMode) {
      case LoadMode.CutCut:
        CutIn();
      break;
      case LoadMode.CutFade:
        FadeIn();
      break;
      case LoadMode.FadeCut:
        CutOut();
      break;
      case LoadMode.FadeFade:
        FadeIn();
      break;
    }

		onSceneLoaded?.Invoke(toLoad);
  }

  void HidePercentage() {
    loadingBar.SetActive(false);
  }

  void CutOut() {
    SetAlpha(255f);
    p_black.SetActive(true);
  }

  void CutIn() {
    SetAlpha(0f);
    p_black.SetActive(false);
  }

  void FadeOut() {
    p_black.SetActive(true);
    animator.SetTrigger("FadeOut");
  }

  void FadeIn() {
    animator.SetTrigger("FadeIn");
  }

  void SetAlpha(float alpha) {
    Color c = i_black.color;
    c = new Color(c.r, c.g, c.b, alpha);
  }

  public void OnFadeComplete() {
    // Make sure everything is disabled
    animator.enabled = false;
    frame.SetActive(false);
  }

  public enum LoadMode {
    CutCut,
    CutFade,
    FadeCut,
    FadeFade, 
  }
}