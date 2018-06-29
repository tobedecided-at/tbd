using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class TBDHelper : MonoBehaviour {
  
  void Start() {
    TBDBootstrap.NewGame();
  }
}