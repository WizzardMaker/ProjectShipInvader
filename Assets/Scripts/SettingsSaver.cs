using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SettingsSaver : MonoBehaviour {
	public void SaveSettings() {
		GameController.SaveSettings();
	}
	public void LoadSettings() {
		GameController.LoadSettings();
		//SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
