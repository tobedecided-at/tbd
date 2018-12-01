using UnityEngine;

using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using TBD.Items;

public class ItemDb : MonoBehaviour {
  private static List<JObject> db = new List<JObject>();
  private string path;

  public string sFileRegex = "*.tbd";

  void Start() {
    path = Application.dataPath + TBDBootstrap.Settings.ItemPath + "json/";
    LoadItems();
  }

  JObject GetItemDataFromFile(string filename) {
    string content = File.ReadAllText(path + filename);
    return JObject.Parse(content);
  }

  void LoadItems() {
    DirectoryInfo d = new DirectoryInfo(path);

    foreach (var file in d.GetFiles(sFileRegex)) {
      JObject itemData = GetItemDataFromFile(file.Name);      
      db.Add(itemData);
    }
  }

  public static JObject GetItemDataBySlug(string slug) {
    foreach (var itemData in ItemDb.db) {
      if ((string)itemData["slug"] == slug)
        return itemData;
    }
    return null;
  }

  public static Item GetNewItemBySlug(string slug) {
    foreach (var itemData in db) {
      if ((string)itemData["slug"] == slug)
        return Item.CreateFromJson(itemData);
    }
    return null;
  }
}