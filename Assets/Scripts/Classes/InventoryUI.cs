using UnityEngine;
using Unity.Entities;
using UnityEngine.SceneManagement;

using System.Collections.Generic;

public class InventoryUI : MonoBehaviour {

  public GameObject gInventoryUI;

  [Header("Parts")]
  public GameObject gInventoryPanel;
  public GameObject gBlur;

  [SerializeField]
  public List<GameObject> lSlots = new List<GameObject>();

  public GameObject goSlotsParent;
  
  public bool isOpen;
  public bool added;

	void Start () {
    Disable();
	}

  void SetLockMode(bool mode) {
    Cursor.lockState = (mode) ? CursorLockMode.Locked : CursorLockMode.None;
    Cursor.visible = !mode;
  }
	
	void Disable() {
    gInventoryPanel.SetActive(false);
    gInventoryUI.SetActive(false);
    gBlur.SetActive(false);
    SetLockMode(true);
  }

  public void OnInventoryButton() {
    gInventoryPanel.SetActive(!gInventoryPanel.activeSelf);
    gInventoryUI.SetActive(gInventoryPanel.activeSelf);
    gBlur.SetActive(gInventoryPanel.activeSelf);
    isOpen = gInventoryPanel.activeSelf;
    SetLockMode(!isOpen);
  }
}
