using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	// Starts a fresh game
	public void NewGame() {
		SaveLoad.ResetCrystalHealth ();
		SaveLoad.ResetPlayerDeck ();
		SaveLoad.ResetPlayerLibrary ();
		SaveLoad.SavePlayerTokens (0);
		SaveLoad.ResetStage ();
		SceneManager.LoadScene ("GameBoard");
	}
}