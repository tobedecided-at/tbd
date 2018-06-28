using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour {
  public string itemSlug;
  public GameObject itemPrefab;
  Item item;

  void Update() {
    if (item == null)
      item = ItemDb.GetItemBySlug(itemSlug);
   
    Instantiate(itemPrefab, this.transform.position, this.transform.rotation);
    Destroy(this);
  }
}