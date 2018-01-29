using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public void NewGame() {
		PlayerPrefs.SetInt ("Crystal1HP", 10);
		PlayerPrefs.SetInt ("Crystal2HP", 10);
		PlayerPrefs.SetInt ("Crystal3HP", 10);
		SceneManager.LoadScene ("GameBoard");
	}
}