using UnityEngine;
using Unity.Entities;

[RequireComponent(typeof(GameObjectEntity))]
public class SyncComponent : MonoBehaviour {
    public bool bShouldSync;
}