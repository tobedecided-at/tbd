using UnityEngine;
using Unity.Entities;
using UnityEngine.SceneManagement;

using System.Collections.Generic;

public class InventoryUI : MonoBehaviour {

  public GameObject gInventoryUI;
  PauseUI pauseUI;

  [Header("Parts")]
  public GameObject gInventoryPanel;
  public GameObject gBlur;

  [SerializeField]
  public List<InventorySlot> lSlots = new List<InventorySlot>();

  public GameObject goSlotsParent;
  public GameObject goSlotsPrefab;
  
  public bool isOpen;
  public bool added;

	void Start () {
    Disable();
    pauseUI = this.gameObject.GetComponent<PauseUI>();
	}

  void SetLockMode(bool mode) {
    Cursor.lockState = (mode) ? CursorLockMode.Locked : CursorLockMode.None;
    Cursor.visible = !mode;
  }
	
	public void Disable() {
    gInventoryPanel.SetActive(false);
    gInventoryUI.SetActive(false);
    gBlur.SetActive(false);
    isOpen = false;
    SetLockMode(true);
  }

  public void OnInventoryButton() {
    // We shouldn't open the inventory while the game is paused
    // Weird things happen
    if (pauseUI.isPaused) return;
    gInventoryPanel.SetActive(!gInventoryPanel.activeSelf);
    gInventoryUI.SetActive(gInventoryPanel.activeSelf);
    gBlur.SetActive(gInventoryPanel.activeSelf);
    isOpen = gInventoryPanel.activeSelf;
    SetLockMode(!isOpen);
  }
}
