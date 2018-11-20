using UnityEngine;
using UnityEngine.UI;

using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

public class Item {

  public static string imgInventoryPath = Application.dataPath + "/StreamingAssets" + TBDBootstrap.Settings.ItemPath;

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
  public Image imgInventory;

  public static Item CreateFromJson(JObject json) {

    Item item = json.ToObject<Item>();

    item.stats = new ItemStats(item.stats);
    item.components = new Dictionary<string, int>();
    item.model = Resources.Load(item.slug, typeof(GameObject)) as GameObject;

    if (File.Exists(imgInventoryPath + item.slug + ".png")) {
      var bFileData = File.ReadAllBytes(imgInventoryPath + item.slug + ".png");
      var t2dtex = new Texture2D(2, 2);
    } else return null;
    
    foreach (var cItem in item.components) {
      item.components.Add((string)cItem.Key, (int)cItem.Value);
    }

    return item;
  }
}