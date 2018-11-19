using UnityEngine;
using System.Collections.Generic;

public class Damaged : MonoBehaviour {
  public List<DamageInfo> hit = new List<DamageInfo>();
}

public class DamageInfo {
  public string damageType;
  public string originName;
  public int damageAmount;

  public DamageInfo(string type, string origin, int amount) {
    this.damageType = type;
    this.originName = origin;
    this.damageAmount = amount;
  }
}