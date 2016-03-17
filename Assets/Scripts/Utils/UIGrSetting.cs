using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Utils.Modifier;

namespace Utils.UI {
	public class UIGrSetting : MonoBehaviour {
		public string variableName;

		public bool isNumber;

		public object pValue;
		UnityEngine.UI.Toggle tggl;
		UnityEngine.UI.Slider sldr;
		UnityEngine.UI.InputField inField;
		UnityEngine.UI.Text text;

		public void Start() {
			pValue = GlobalModifierList.Get(variableName);

			tggl = GetComponent<UnityEngine.UI.Toggle>();
			sldr = GetComponent<Slider>();
			inField = GetComponent<InputField>();
			text = GetComponent<Text>();

			if (tggl != null) {
				tggl.isOn = (bool)pValue;
			}
			if(sldr != null) {
				sldr.value = (float)pValue;
			}
			if(inField != null) {
				inField.text = pValue.ToString();
			}
			if (text != null) {
				text.text = pValue.ToString();
			}
		}

		public void OnGUI() {
			if (GameController.active.updated) {
				pValue = GlobalModifierList.Get(variableName);

				if (tggl != null) {
					tggl.isOn = (bool)GlobalModifierList.Get(variableName);
					return;
				}
				if (sldr != null) {
					sldr.value = (float)GlobalModifierList.Get(variableName);
					return;
				}
				if (text != null) {
					if (isNumber)
						text.text = ((float)GlobalModifierList.Get(variableName)).ToString("F1");
					else
						text.text = GlobalModifierList.Get(variableName).ToString();
					return;
				}
				if (inField != null) {
					inField.text = pValue.ToString();
					return;
				}
			}
			if (tggl != null) {
				if (tggl.isOn != (bool)pValue) {
					GlobalModifierList.SetObject(tggl.isOn, variableName);
					pValue = tggl.isOn;
				}
				return;
			}
			if (sldr != null) {
				if (sldr.value != (float)pValue) {
					GlobalModifierList.SetObject(sldr.value, variableName);
					pValue = sldr.value;
				}
				return;
			}
			if (inField != null) {
				if (inField.text != (string)pValue) {
					GlobalModifierList.SetObject(inField.text, variableName);
					pValue = inField.text;
				}
				return;
			}
			
		}
	}
}