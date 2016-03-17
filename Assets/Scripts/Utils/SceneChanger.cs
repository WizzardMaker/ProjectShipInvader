using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour {
	public void ChangeScene(int id) {
		SceneManager.LoadScene(id);
	}
	public void ChangeScene(string id) {
		SceneManager.LoadScene(id);
	}
}
