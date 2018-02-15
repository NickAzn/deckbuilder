using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour {

	public GameObject shopButton;
	public Text stageText;
	public Text levelText;

	void Start() {
		int[] stageLoadout = SaveLoad.LoadStage ();
		if (stageLoadout == null) {
			GenerateStage (0);
		} else {
			UpdateStageUI ();
		}
	}

	void UpdateStageUI() {
		int[] stageLoadout = SaveLoad.LoadStage ();
		stageText.text = "Stage: " + (stageLoadout [0] + 1).ToString ();
		levelText.text = "Levels Cleared: " + stageLoadout [3].ToString() + "/" + stageLoadout [1].ToString ();
		if (stageLoadout [3] < stageLoadout [2]) {
			shopButton.SetActive (false);
		}
	}

	void GenerateStage(int stage) {
		int encounterCount = 0;
		int shopEncounter = 0;
		if (stage == 0) {
			encounterCount = Random.Range (5, 8);
			shopEncounter = Random.Range (3, encounterCount - 1);
		} else {
			LoadScene ("MainMenu");
		}
		SaveLoad.SaveStageLoadout (stage, encounterCount, shopEncounter);
		SaveLoad.SaveCurrentEncounter (1);
		SaveLoad.GenerateNewShop ();

		UpdateStageUI ();
	}

	public void LoadEncounter(string name) {
		int[] stageLoadout = SaveLoad.LoadStage ();
		SaveLoad.SaveCurrentEncounter (stageLoadout [3] + 1);
		SceneManager.LoadScene (name);
	}

	public void LoadScene(string name) {
		SceneManager.LoadScene (name);
	}
}
