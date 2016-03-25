using UnityEngine;
using System;
using Utils.Modifier;

public class CameraController : MonoBehaviour {
	Camera cam;
	Pixelation pix;

	public float lookSpeed;
	public bool xInverted, yInverted;
	public float maxAngle, minAngle;
	public float testFloat;

	VariableReference MouseSensX, InvertY, MouseSensY, InvertX, FOV, PixelationMultiplier;

	// Use this for initialization
	void Start () {
		cam = Camera.main;
		pix = Pixelation.active;

		MouseSensX = GlobalModifierList.GetRef("MouseSensX");
		InvertX = GlobalModifierList.GetRef("InvertX");
		MouseSensY = GlobalModifierList.GetRef("MouseSensY");
		InvertY = GlobalModifierList.GetRef("InvertY");
		FOV = GlobalModifierList.GetRef("FOV");
		PixelationMultiplier = GlobalModifierList.GetRef("PixelationMultiplier");
    }

	static Vector3 ClampY(Vector3 angle, float max, float min) {
		return new Vector3(angle.x, angle.y > max ? max : angle.y < min ? min : angle.y, angle.z);
	}
	static Vector3 ClampX(Vector3 angle, float max, float min) {
		if (angle.x > 180) // For the 360 degrees snap, so that we have a range from -180 - 180, looking down y
			angle.x -= 360;

		return new Vector3(angle.x > max ? max : angle.x < min ? min : angle.x,0,0);
	}

	// Update is called once per frame
	void Update () {

		cam.fieldOfView = (float)FOV.Get();

		pix.pixelWidth = 1 * (float)PixelationMultiplier.Get();
		pix.pixelHeight = 1 * (float)PixelationMultiplier.Get();

		pix.enabled = pix.pixelWidth >= 1;

		if (Time.timeScale == 0)
			return;


		transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * (float)MouseSensX.Get() * ((bool)InvertX.Get() ? -1 : 1));

		cam.transform.Rotate(Vector3.right * Input.GetAxisRaw("Mouse Y") * (float)MouseSensY.Get() * ((bool)InvertY.Get() ? 1 : -1));

		cam.transform.localRotation = Quaternion.Euler(ClampX(cam.transform.localRotation.eulerAngles, maxAngle, -minAngle));

	}
}
