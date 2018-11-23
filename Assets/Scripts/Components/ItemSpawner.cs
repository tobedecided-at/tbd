using UnityEngine;

public class ItemSpawner : MonoBehaviour {
  public string itemSlug;
  public GameObject itemPrefab;
  public Item item;
  public int amount;

  void Start() {

  }

  void Update() {

    item = ItemDb.GetNewItemBySlug(itemSlug);
    
    if (item == null) {
      Debug.LogWarning(string.Format("Item {0} could not be found!", itemSlug));
      return;
    }
    
    for (int x = 1; x <= amount; x++) {
      var pos = this.gameObject.transform.position;
      var rot = this.gameObject.transform.rotation;
    
      GameObject iGo = Object.Instantiate(itemPrefab, pos, rot);
      GameObject model = Object.Instantiate((GameObject)item.model, iGo.transform.position, iGo.transform.rotation, iGo.transform);

      iGo.name = item.slug + " " + x;
      var iComp = iGo.AddComponent<ItemComponent>();
      
      iComp.pickedUp = false;
      iComp.item = item;
    }

    Object.Destroy(this.gameObject);
  }
}