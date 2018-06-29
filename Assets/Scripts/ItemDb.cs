using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class ItemDb : MonoBehaviour {
  [SerializeField]
  private static List<Item> db = new List<Item>();
  private string path;

  void Start() {
    path = Application.dataPath + TBDBootstrap.Settings.ItemPath;
    GetItemsFromFile();
  }

  Item GetItemFromFile(string filename) {
    string content = File.ReadAllText(Application.dataPath + TBDBootstrap.Settings.ItemPath + "/" + filename);
    return new Item(JObject.Parse(content));
  }

  void GetItemsFromFile() {
    DirectoryInfo d = new DirectoryInfo(path);

    foreach (var file in d.GetFiles("*.tbd")) {
      Item item = GetItemFromFile(file.Name);      
      ItemDb.db.Add(item);
    }
  }

  public static Item GetItemBySlug(string slug) {
    foreach (Item item in ItemDb.db) {
      if (item.slug == slug)
        return item;
    }
    return null;
  }
}