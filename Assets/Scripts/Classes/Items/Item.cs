using UnityEngine;
using UnityEngine.UI;

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace TBD.Items {
  public class Item {

    public static System.Random rng = new System.Random();
    public static string imgInventoryPath = Application.dataPath + TBDBootstrap.Settings.ItemPath + "img/";

    public int uid;
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
    public bool wearable;
    public EquipmentSlot wearable_slot;
    public Texture2D imgInventory;
    public dynamic specific;

    public static Item CreateFromJson(JObject json) {

      Item item = json.ToObject<Item>();

      item.stats = json["stats"].ToObject<Dictionary<string, float>>();
      item.components = json["components"].ToObject<Dictionary<string, int>>();
      item.model = Resources.Load(item.slug, typeof(GameObject)) as GameObject;
      item.stackSize = 1;
      if (item.wearable)
        item.wearable_slot = (EquipmentSlot)Enum.Parse(typeof(EquipmentSlot), (string)json["wearable_slot"], true);
      item.uid = rng.Next();

      // Load the items picture if it exists
      if (File.Exists(imgInventoryPath + item.slug + ".png")) {
        var bFileData = File.ReadAllBytes(imgInventoryPath + item.slug + ".png");
        var t2dtex = new Texture2D(2, 2);
        t2dtex.LoadImage(bFileData);
        item.imgInventory = t2dtex;
      } else { // Fallback to default.png (maybe something funny to show that we messed up)
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
