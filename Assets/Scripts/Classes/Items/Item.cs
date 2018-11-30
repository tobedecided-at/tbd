using UnityEngine;
using UnityEngine.UI;

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

public class Item : IItem {

  public static string imgInventoryPath = Application.dataPath + TBDBootstrap.Settings.ItemPath + "img/";
  public static readonly System.Random rng = new System.Random();

  public int uid;
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
  public Texture2D imgInventory;

  public static Item CreateFromJson(JObject json) {

    Item item = json.ToObject<Item>();

    item.stats = json["stats"].ToObject<ItemStats>();
    item.components = json["components"].ToObject<Dictionary<string, int>>();
    item.model = Resources.Load(item.slug, typeof(GameObject)) as GameObject;
    item.uid = rng.Next(100000, 999999);

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
}