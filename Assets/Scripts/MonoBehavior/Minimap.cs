using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    GameObject player;

    bool bPlayerFound = false;
   
    void Update() {
      if (!bPlayerFound)
        player = TBDBootstrap.Settings.Globals.goPlayer;
    }

    void LateUpdate() {
      if (!bPlayerFound) return;
      
      Vector3 newPosition = player.transform.position;
      newPosition.y = transform.position.y;
      transform.position = newPosition;
    }
}
