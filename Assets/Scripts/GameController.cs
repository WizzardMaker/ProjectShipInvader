using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Utils.Modifier;
using System.Collections.Generic;
using System.IO;

public class GameController : MonoBehaviour {

	public static GameController active = null;

	public bool tempController;

	public bool updated;
	bool updatePassed;

	public float fov;
	public float pixelationMultiplier;
	public float mouseSensX;
	public float mouseSensY;
	public Resolution res;
	public bool invertX;
	public bool invertY;

	// Use this for initialization
	void Awake() {
		if (active != null) {
			Destroy(gameObject);
			return;
		}
		active = this;



		INIWorker.path = Path.Combine(Application.persistentDataPath, "Settings.ini");

		if (!File.Exists(INIWorker.path)) {
			SaveSettings();
        }

		LoadModifier();

		LoadSettings();

		DontDestroyOnLoad(gameObject);


		Debug.Log("Change Level");
		if (tempController != true)
			SceneManager.LoadScene("mainmenu",LoadSceneMode.Single);
	}

	public void LoadModifier() {
		GlobalModifierList.AddModifier("FOV", () => fov, (value) => fov = (float)value);

		GlobalModifierList.AddModifier("MouseSensX", () => mouseSensX, (value) => mouseSensX = (float)value);
		GlobalModifierList.AddModifier("MouseSensY", () => mouseSensY, (value) => mouseSensY = (float)value);

		GlobalModifierList.AddModifier("InvertY", () => invertY, (value) => invertY = (bool)value);
		GlobalModifierList.AddModifier("InvertX", () => invertX, (value) => invertX = (bool)value);

		GlobalModifierList.AddModifier("PixelationMultiplier", () => pixelationMultiplier, (value) => pixelationMultiplier = (float)value);

		GlobalModifierList.AddModifier("Resolutions", () => new List<Resolution>(Screen.resolutions), (value) => Debug.LogError("You are not allowed to manipulate the possible Resolutions"));
		GlobalModifierList.AddModifier("Resolution", () => FindIdInArray(Screen.resolutions, res), (value) => { if (Screen.resolutions.Length <= 1) return; if (res.width != Screen.resolutions[(int)value].width && res.height != Screen.resolutions[(int)value].height) Screen.SetResolution(res.width, res.height, Screen.fullScreen); res = Screen.resolutions[(int)value]; });
		GlobalModifierList.AddModifier("Fullscreen", () => Screen.fullScreen, (value) => Screen.SetResolution(res.width, res.height, (bool)value));

		GlobalModifierList.AddModifier("pixelLights", () => QualitySettings.pixelLightCount, (value) => QualitySettings.pixelLightCount = (int)value);

		GlobalModifierList.AddModifier("VSyncs", () => QualitySettings.vSyncCount, (value) => QualitySettings.vSyncCount = (int)value);
		GlobalModifierList.AddModifier("AA", () => QualitySettings.antiAliasing, (value) => QualitySettings.antiAliasing = (int)value == 2 ? 4 : (int)value == 3 ? 8 : (int)value == 1 ? 2 : 0);

		GlobalModifierList.AddModifier("screenX", () => res.width, (value) => res.width = (int)value);
		GlobalModifierList.AddModifier("screenY", () => res.height, (value) => res.height = (int)value);
	}

	public object FindIdInArray<type>(type[] array, type toFind) {
		for (int i = 0; i < array.Length; i++) {
			if (array[i].Equals(toFind)) return i;
		}

		return -1;
	}

	public static void SaveSettings() {
		foreach(string key in GlobalModifierList.modifier.Keys) {
			INIWorker.IniWriteValue("Settings",key, GlobalModifierList.modifier[key].Get().ToString());
        }
	}

	public static void LoadSettings() {
		try {
			foreach (string key in GlobalModifierList.modifier.Keys) {
				object obj = GlobalModifierList.modifier[key].Get();

				if (obj is bool) {
					GlobalModifierList.modifier[key].Set(bool.Parse(INIWorker.IniReadValue("Settings", key)));
				}
				if(obj is float) {
					GlobalModifierList.modifier[key].Set(float.Parse(INIWorker.IniReadValue("Settings", key)));
				}
				if(obj is int) {
					GlobalModifierList.modifier[key].Set(int.Parse(INIWorker.IniReadValue("Settings", key)));
				}
            }
		} catch (Exception e) {
			SaveSettings();
			throw (e);
		}
	}
	/*
		public static void SaveSettings(bool blank = false) {

			Debug.Log("SaveSettings called, blank: " + blank);
			if (blank) {
				INIWorker.IniWriteValue("GraphicSettings", "pixelLights", QualitySettings.pixelLightCount.ToString());
				INIWorker.IniWriteValue("GraphicSettings", "VSyncs", QualitySettings.vSyncCount.ToString());
				INIWorker.IniWriteValue("GraphicSettings", "AA", QualitySettings.vSyncCount.ToString());
				INIWorker.IniWriteValue("GraphicSettings", "screenX", active.res.width.ToString());
				INIWorker.IniWriteValue("GraphicSettings", "screenY", active.res.height.ToString());
				INIWorker.IniWriteValue("GraphicSettings", "fullscreen", Screen.fullScreen.ToString());

				INIWorker.IniWriteValue("GameSettings", "FOV", "90");
				INIWorker.IniWriteValue("GameSettings", "mouseSensX", "1");
				INIWorker.IniWriteValue("GameSettings", "mouseSensY", "1");
				INIWorker.IniWriteValue("GameSettings", "invertX", "false");
				INIWorker.IniWriteValue("GameSettings", "invertY", "false");
			} else {
				INIWorker.IniWriteValue("GraphicSettings", "pixelLights", QualitySettings.pixelLightCount.ToString());
				INIWorker.IniWriteValue("GraphicSettings", "VSyncs", QualitySettings.vSyncCount.ToString());
				INIWorker.IniWriteValue("GraphicSettings", "AA", QualitySettings.vSyncCount.ToString());
				INIWorker.IniWriteValue("GraphicSettings", "fullscreen", Screen.fullScreen.ToString());
				INIWorker.IniWriteValue("GraphicSettings", "screenX", active.res.width.ToString());
				INIWorker.IniWriteValue("GraphicSettings", "screenY", active.res.height.ToString());

				INIWorker.IniWriteValue("GameSettings", "FOV", active.fov.ToString());
				INIWorker.IniWriteValue("GameSettings", "mouseSensX", active.mouseSensX.ToString());
				INIWorker.IniWriteValue("GameSettings", "mouseSensY", active.mouseSensY.ToString());
				INIWorker.IniWriteValue("GameSettings", "invertX", active.invertX.ToString());
				INIWorker.IniWriteValue("GameSettings", "invertY", active.invertY.ToString());
			}
		}

		public static void LoadSettings() {
			try {
				QualitySettings.pixelLightCount = int.Parse(INIWorker.IniReadValue("GraphicSettings", "pixelLights"));
				QualitySettings.vSyncCount = int.Parse(INIWorker.IniReadValue("GraphicSettings", "VSyncs"));
				QualitySettings.antiAliasing = int.Parse(INIWorker.IniReadValue("GraphicSettings", "AA"));
				//Screen.SetResolution(int.Parse(INIWorker.IniReadValue("GraphicSettings", "screenX")), int.Parse(INIWorker.IniReadValue("GraphicSettings", "screenY")), bool.Parse(INIWorker.IniReadValue("GraphicSettings", "fullscreen")));

				active.res = new Resolution();
				active.res.width = int.Parse(INIWorker.IniReadValue("GraphicSettings", "screenX"));
				active.res.height = int.Parse(INIWorker.IniReadValue("GraphicSettings", "screenY"));

				Screen.fullScreen = bool.Parse(INIWorker.IniReadValue("GraphicSettings", "fullscreen"));

				Debug.Log(QualitySettings.pixelLightCount);
				Debug.LogError(int.Parse(INIWorker.IniReadValue("GraphicSettings", "screenX"))+","+ int.Parse(INIWorker.IniReadValue("GraphicSettings", "screenY"))+" f:"+ bool.Parse(INIWorker.IniReadValue("GraphicSettings", "fullscreen")));

				active.fov = float.Parse(INIWorker.IniReadValue("GameSettings", "FOV"));
				active.mouseSensX = float.Parse(INIWorker.IniReadValue("GameSettings", "mouseSensX"));
				active.mouseSensY = float.Parse(INIWorker.IniReadValue("GameSettings", "mouseSensY"));
				active.invertX = bool.Parse(INIWorker.IniReadValue("GameSettings", "invertX"));
				active.invertY = bool.Parse(INIWorker.IniReadValue("GameSettings", "invertY"));
				active.updated = true;
			} catch (Exception e) {
				Debug.LogError(e.Message);
				SaveSettings(true);
			}
		}
	*/
	bool menuActive;

	// Update is called once per frame
	void Update () {


		//Debug.LogError(Screen.fullScreen + "|" + bool.Parse(INIWorker.IniReadValue("GraphicSettings", "fullscreen")));

		if (updated && updatePassed) {
			updated = false;
			updatePassed = false;
		}
		if (updated) {
			updatePassed = true;
		}

		Time.timeScale = menuActive ? 0 : 1;

		// If Ingame
		if (SceneManager.GetActiveScene().name == "main") {

			if (Input.GetButtonDown("Cancel")) {
				if (!menuActive) {
					SceneManager.LoadScene("settings", LoadSceneMode.Additive);

					menuActive = true;
				} else {
					SceneManager.UnloadScene("settings");

					menuActive = false;
				}
			}



		}

	} 
}
