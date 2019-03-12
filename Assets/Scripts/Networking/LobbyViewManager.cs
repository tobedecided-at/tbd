using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TBD.WS;

using UnityEngine;
using UnityEngine.UI;

public class LobbyViewManager : MonoBehaviour {

	public static LobbyViewManager instance;

	public Dictionary<string, GameObject> LobbyViewItems;
	public GameObject LobbyViewParent;
	public GameObject LobbyViewItemPrefab;

	// Start is called before the first frame update
	void Start() {
		instance = this;
		LobbyViewItems = new Dictionary<string, GameObject>();
	}

	public void AddToView(string id, string name) {
		GameObject temp = Instantiate(LobbyViewItemPrefab, LobbyViewParent.transform);
		Text text = temp.GetComponentInChildren<Text>();

		text.text = name + " : " + id.Substring(0, 5) + "...";
		LobbyViewItems.Add(id, temp);
	}

	public void RemoveFromView(string id) {
		GameObject found;
		LobbyViewItems.TryGetValue(id, out found);

		if (found != null) {
			LobbyViewItems.Remove(id);
			Destroy(found);
		}
	}

	public void AddDictionaryToView(Dictionary<string, WSData> dict) {
		for (int i = 0; i < dict.Count; i++) {
			var data = dict.ElementAt(i).Value;
			AddToView(data.id, data.username);
		}
	}

	public void Clear() {
		foreach (var e in LobbyViewItems) {
			Destroy(e.Value);
		}
		LobbyViewItems.Clear();
	}
}
