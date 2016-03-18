using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Pixelation : MonoBehaviour {
	// Use this for initialization
	void Start() {

	}

	// Update is called once per frame
	void Update() {

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
