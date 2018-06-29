using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour {
  public string itemSlug;
  public GameObject itemPrefab;
  Item item;

  void Update() {
    if (item == null) {
      item = ItemDb.GetItemBySlug(itemSlug);
      return;
    }
   
    GameObject iGo = Instantiate(itemPrefab, this.transform.position, this.transform.rotation);
    GameObject model = Instantiate((GameObject)item.model, iGo.transform.position, iGo.transform.rotation, iGo.transform);

    iGo.name = item.slug;
    var iComp = iGo.AddComponent<ItemComponent>();
    
    iComp.slug = item.slug;
    iComp.title = item.title;
    iComp.desc = item.desc;
    iComp.pickupRange = item.pickupRange;
    iComp.stackable = item.stackable;
    iComp.maxStackSize = item.maxStackSize;
    iComp.rarity = item.rarity;
    iComp.stats = item.stats;
    iComp.components = item.components;
    iComp.weight = item.weight;
    iComp.pickedUp = item.pickedUp;
    iComp.item = item.item;
    iComp.value = item.value;

    Destroy(this);
  }
}