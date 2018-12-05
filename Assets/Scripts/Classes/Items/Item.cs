using UnityEngine;
using UnityEngine.UI;

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace TBD.Items {
  public class Item {

    public static string imgInventoryPath = Application.dataPath + TBDBootstrap.Settings.ItemPath + "img/";

    public string slug;
    public string title;
    public string desc;
    public float weight;
    public float pickupRange;
    public int maxStackSize;
    public int rarity;
    public Dictionary<string, float> stats = new Dictionary<string, float>();
    public Dictionary<string, int> components;
    public GameObject model;
    public float value;
    public int stackSize;
    public bool equipped = false;
    public bool usable;
    public Texture2D imgInventory;
    public dynamic specific;

    public static Item CreateFromJson(JObject json) {

      Item item = json.ToObject<Item>();

      item.stats = json["stats"].ToObject<Dictionary<string, float>>();
      item.components = json["components"].ToObject<Dictionary<string, int>>();
      item.model = Resources.Load(item.slug, typeof(GameObject)) as GameObject;

      if (File.Exists(imgInventoryPath + item.slug + ".png")) {
        var bFileData = File.ReadAllBytes(imgInventoryPath + item.slug + ".png");
        var t2dtex = new Texture2D(2, 2);
        t2dtex.LoadImage(bFileData);
        item.imgInventory = t2dtex;
      } else {
        var bFileData = File.ReadAllBytes(imgInventoryPath + "default.png");
        var t2dtex = new Texture2D(2, 2);
        t2dtex.LoadImage(bFileData);
        item.imgInventory = t2dtex;
      }
      return item;
    }

    public override bool Equals(object obj) {
      if (obj == null) return false;
      Item temp = (Item)obj;
      if (this.slug == temp.slug)
        return true;
      return false;
    }
  }
}
