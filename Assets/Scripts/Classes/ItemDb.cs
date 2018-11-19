using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class ItemDb : MonoBehaviour {
  [SerializeField]
  private static List<JObject> db = new List<JObject>();
  private string path;

  void Start() {
    path = Application.dataPath + "/StreamingAssets" + TBDBootstrap.Settings.ItemPath;
    GetItemsFromFile();
  }

  JObject GetItemDataFromFile(string filename) {
    string content = File.ReadAllText(path + filename);
    return JObject.Parse(content);
  }

  void GetItemsFromFile() {
    DirectoryInfo d = new DirectoryInfo(path);

    foreach (var file in d.GetFiles("*.tbd")) {
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
    foreach (var item in db) {
      if ((string)item["slug"] == slug)
        return new Item(item);
    }
    return null;
  }
}