using UnityEngine;

namespace TBD.Items {
  public class IItem {
    Item data {get;}
    PlayerInventory inventory {get;}

    public virtual void OnPickup()  {}
    
    public virtual void OnUse()     {}
    public virtual void OnAction()  {}

    public virtual void OnEquip()   {}
    public virtual void OnUnequip() {}
  }
}