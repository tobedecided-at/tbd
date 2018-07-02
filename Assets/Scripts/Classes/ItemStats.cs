using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class ItemStats {
  public float iHealth;

  public float iDamage;
  public float mDamage;
  
  public float iArmor;
  
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
        case "iDamage":
          this.iDamage = (float)stat.Value;
        break;
        case "mDamage":
          this.mDamage = (float)stat.Value;
        break;
        case "iArmor":
          this.iArmor = (float)stat.Value;
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

  public ItemStats(ItemStats template) {

    this.iHealth = template.iHealth;
    this.iDamage = template.iDamage;
    this.mDamage = template.mDamage;
    this.iArmor = template.iArmor;
    this.iFood = template.iFood;
    this.iWater = template.iWater;
    this.iHeat = template.iHeat;
  }
}