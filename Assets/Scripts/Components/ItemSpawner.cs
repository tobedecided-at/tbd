using UnityEngine;

using TBD.Items;

public class ItemSpawner : MonoBehaviour {
  public string itemSlug;
  public Item item;
  public int amount;

  static GameObject goItemHolder;

  void Start() {
    goItemHolder = (GameObject)Resources.Load("ItemHolder");
  }

  void Update() {
    
    if (ItemDb.GetItemDataBySlug(itemSlug) == null) {
      Debug.LogWarning(string.Format("Item {0} could not be found!", itemSlug));
      return;
    }
    
    for (int x = 1; x <= amount; x++) {
      item = ItemDb.GetNewItemBySlug(itemSlug);
      var pos = gameObject.transform.position;
      var rot = gameObject.transform.rotation;

      SpawnItem(item, pos, rot);
    }

    Destroy(gameObject);
  }

  public static GameObject SpawnItem(Item toSpawn, Vector3 pos, Quaternion rot, float force = 0f) {

    GameObject iGo = Instantiate(goItemHolder, pos, rot);
    GameObject model = Instantiate(toSpawn.model, iGo.transform.position, iGo.transform.rotation, iGo.transform);

    iGo.name = "I/" + toSpawn.uid;
    var iComp = iGo.AddComponent<ItemComponent>();

    toSpawn.specific = System.Activator.CreateInstance(System.Type.GetType("TBD.Items."+toSpawn.slug));
    toSpawn.specific.data = toSpawn;

    iComp.pickedUp = false;
    iComp.item = toSpawn;

    if (force > 0f) {
      iGo.GetComponent<Rigidbody>().AddForce(rot * Vector3.forward * force, ForceMode.Impulse);
    }

    return iGo;
  }
}