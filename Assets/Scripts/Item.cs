using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class Item {

  public string slug;
  public string title;
  public string desc;
  public float pickupRange;
  public bool stackable;
  public int maxStackSize;
  public int rarity;
  public ItemStats stats;
  public Dictionary<string, int> components;

  public Item(JObject item) {
    this.slug = (string)item["slug"];
    this.title = (string)item["title"];
    this.desc = (string)item["desc"];
    this.pickupRange = (float)item["pickupRange"];
    this.stackable = (bool)item["stackable"];
    this.maxStackSize = (int)item["maxStackSize"];
    this.rarity = (int)item["rarity"];
    this.stats = new ItemStats(item["stats"]);
    this.components = new Dictionary<string, int>();
    
    foreach (var cItem in (JObject)item["components"]) {
      this.components.Add((string)cItem.Key, (int)cItem.Value);
    }
  }
}