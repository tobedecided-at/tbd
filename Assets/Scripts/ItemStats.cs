using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class ItemStats {
  public float iHealth;
  public float mHealth;

  public float iDamage;
  public float mDamage;
  
  public float iArmor;
  public float mArmor;
  
  public float iFood;
  public float iWater;
  public float iHeat; // @future

  public ItemStats(JToken token) {
    JObject stats = (JObject)token;

    foreach (var stat in stats) {
      switch (stat.Key) {
        case "iHealth":
          this.iHealth = (float)stat.Value;
        break;
        case "mHealth":
          this.mHealth = (float)stat.Value;
        break;
        case "iDamage":
          this.iDamage = (float)stat.Value;
        break;
        case "mDamage":
          this.mDamage = (float)stat.Value;
        break;
        case "iArmor":
          this.iArmor = (float)stat.Value;
        break;
        case "mArmor":
          this.mArmor = (float)stat.Value;
        break;
        case "iFood":
          this.iFood = (float)stat.Value;
        break;
        case "iWater":
          this.iWater = (float)stat.Value;
        break;
        case "iHeat":
          this.iHeat = (float)stat.Value;
        break;
      }
    }
  }
}