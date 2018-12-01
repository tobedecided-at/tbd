using UnityEngine;

namespace TBD.Items {
    public abstract class IItem {
        Item data {get;}
        PlayerInventory inventory {get;}

        public abstract void OnPickup();
        public abstract void OnUse();
        public abstract void OnAction();
    }

}