using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Utils.UI {
	public class UIChanger : MonoBehaviour {
		public float _fValue;
		public bool _bValue;

		public float fValue {
			get {
				return _fValue;
			}
			set {
				_fValue = value;
			}
		}
		public bool bValue {
			get {
				return _bValue;
			}
			set {
				_bValue = value;
			}
		}

		UnityEngine.UI.Toggle tggl;
		UnityEngine.UI.Slider sldr;
		UnityEngine.UI.Text text;

		public void Start() {

			tggl = GetComponent<UnityEngine.UI.Toggle>();
			sldr = GetComponent<Slider>();
			text = GetComponent<Text>();
			if (tggl != null) {
				tggl.isOn = bValue;
				return;
			}
			if(sldr != null) {
				sldr.value = fValue;
				return;
			}
			if(text != null) {
				text.text = fValue.ToString();
				return;
			}
		}

		public void OnGUI() {
			if (tggl != null) {
				tggl.isOn = bValue;
                
                return;
			}
			if (sldr != null) {
				sldr.value = fValue;
				
				return;
			}
			if (text != null) {
				text.text = fValue.ToString();
				
				return;
			}
		}
	}
}