using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class Item {

  public string slug;
  public string title;
  public string desc;
  public float weight;
  public float pickupRange;
  public int maxStackSize;
  public int rarity;
  public ItemStats stats;
  public Dictionary<string, int> components;
  public GameObject model;
  public Item item;
  public float value;
  public int stackSize;
  public bool equipped = false;
  public bool usable;

  public Item(JObject item) {
    this.slug = (string)item["slug"];
    this.title = (string)item["title"];
    this.desc = (string)item["desc"];
    this.weight = (float)item["weight"];
    this.pickupRange = (float)item["pickupRange"];
    this.maxStackSize = (int)item["maxStackSize"];
    this.rarity = (int)item["rarity"];
    this.stats = new ItemStats(item["stats"]);
    this.components = new Dictionary<string, int>();
    this.model = Resources.Load(slug, typeof(GameObject)) as GameObject;
    this.item = this;
    this.usable = (bool)item["useable"];
    this.value = (float)item["value"];
    
    foreach (var cItem in (JObject)item["components"]) {
      this.components.Add((string)cItem.Key, (int)cItem.Value);
    }
  }
}