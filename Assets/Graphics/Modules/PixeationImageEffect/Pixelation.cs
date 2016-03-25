using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Pixelation : MonoBehaviour {

	public static Pixelation active;

	// Use this for initialization
	void Awake() {
		if(active != null) {
			this.enabled = false;
			return;
		}

		active = this;
	}

	// Update is called once per frame
	void Update() {
		if (active != this) {
			this.enabled = false;
			return;
		}
	}

	public Shader PixelationShader;

	public float pixelWidth;
	public float pixelHeight;

	internal Material _mat;
	public Material mat {
		get {
			if(_mat == null) {
				_mat = new Material(PixelationShader);
				_mat.hideFlags = HideFlags.HideAndDontSave;
			}
			return _mat;

		}

	}


	protected void OnDisable() {
		if (mat)
			DestroyImmediate(mat);
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination) {
		mat.SetTexture("_RenderTexture", source);

		mat.SetFloat("_PixelWidth", pixelWidth);
		mat.SetFloat("_PixelHeight", pixelHeight);

		Graphics.Blit(source, destination, mat);


	}
}
