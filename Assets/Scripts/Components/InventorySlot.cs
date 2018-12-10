using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;
using TBD.Items;

public class InventorySlot : MonoBehaviour {
  public int id;
  public bool isHotbarSlot = false;
  public Item item;
  public TMP_Text tmproStackSize;
  public RawImage rimgIconHolder;
  public GameObject goHotbarSlotSelectedVisual;
}
