using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class minimap : MonoBehaviour
{
    public GameObject player;
   
    void Start ()
    {
        player = TBDBootstrap.Settings.UI.GetComponentInParent<Player>().gameObject;
    }

    void LateUpdate ()
    {
        Vector3 newPosition = player.transform.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;
    }
}
