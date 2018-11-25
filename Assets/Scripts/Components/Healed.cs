using UnityEngine;
using System.Collections.Generic;

public class Healed : MonoBehaviour {
  public List<HealInfo> hit = new List<HealInfo>();
}

public class HealInfo {
  public string healType;
  public string originName;
  public int healAmount;

  public HealInfo(string type, string origin, int amount) {
    this.healType = type;
    this.originName = origin;
    this.healAmount = amount;
  }
}