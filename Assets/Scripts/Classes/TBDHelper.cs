using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class TBDHelper : MonoBehaviour {
  
  void Start() {
    TBDBootstrap.Init();
    TBDBootstrap.NewGame();
  }
}

public static class TBDExtensions {

  public static float Map(this float f, float from1, float to1, float from2, float to2) {
    return (f - from1) / (to1 - from1) * (to2 - from2) + from2;
  } 
}