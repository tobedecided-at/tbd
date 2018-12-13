using UnityEngine;
using System.Collections.Generic;

namespace TBD.Items {
  public class rebar_rod : IItem {

    #region Properties
    
    public Item data {get; set;}
    public PlayerInventory inventory {get; set;}
    
    #endregion
    
    // On Pickup, duh..
    public override void OnPickup() {}

    // Hold right click
    public override void OnUse() {}

    // Left click
    public override void OnAction() {}

    // Equip
    public override void OnEquip() {}

    /*
     * Player player = inventory.gameObject.GetComponentInParent<Player>();
     * Healed healed = player.GetComponent<Healed>();
     * healed.hit.Add(new HealInfo("HEAL", "TEMPLATE_ITEM", Mathf.RoundToInt(data.stats["iHealth"])));
     * Damaged damaged = player.GetComponent<Damaged>();
     * damaged.hit.Add(new DamageInfo("DAMAGE", "TEMPLATE_ITEM", 420f));
     */
  }
}