using UnityEngine;

using TBD.Items;

public class ItemSpawner : MonoBehaviour {
  public string itemSlug;
  public Item item;
  public int amount;
  public static System.Random rng;

  static GameObject goItemHolder;

  void Start() {
    rng = new System.Random();
    goItemHolder = (GameObject)Resources.Load("ItemHolder");
  }

  void Update() {
    
    if (ItemDb.GetItemDataBySlug(itemSlug) == null) {
      Debug.LogWarning(string.Format("Item {0} could not be found!", itemSlug));
      return;
    }
    
    for (int x = 1; x <= amount; x++) {
      item = ItemDb.GetNewItemBySlug(itemSlug);
      var pos = this.gameObject.transform.position;
      var rot = this.gameObject.transform.rotation;

      SpawnItem(item, pos, rot);
    }

    Object.Destroy(this.gameObject);
  }

  public static GameObject SpawnItem(Item toSpawn, Vector3 pos, Quaternion rot, float force = 0f) {

    GameObject iGo = Object.Instantiate(goItemHolder, pos, rot);
    GameObject model = Object.Instantiate((GameObject)toSpawn.model, iGo.transform.position, iGo.transform.rotation, iGo.transform);

    iGo.name = toSpawn.slug + " " + rng.Next();
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