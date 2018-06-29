using System.Collections.Generic;
using UnityEngine;

public class ItemComponent : MonoBehaviour {
  
  public string slug;
  public string title;
  public string desc;
  public float pickupRange;
  public float weight;
  public bool stackable;
  public int maxStackSize;
  public int rarity;
  public ItemStats stats;
  public bool pickedUp;
  public Dictionary<string, int> components; 
  public float value;

  public Item item;
}