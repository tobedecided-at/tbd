﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressAnyKey : MonoBehaviour {

  WindowManager wm;
  void Start() {
    wm = TBDBootstrap.Settings.UI.GetComponent<WindowManager>();
  }

  // Update is called once per frame
  void Update() {
    if (Input.anyKey) {
      wm.ShowMain();
    }
  }
}
