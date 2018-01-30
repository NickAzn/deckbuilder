using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public void NewGame() {
		SaveLoad.ResetCrystalHealth ();
		SaveLoad.ResetPlayerDeck ();
		SceneManager.LoadScene ("GameBoard");
	}
}