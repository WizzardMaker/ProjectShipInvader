using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Utils.Modifier;

namespace Utils.UI {
	public class UIAccesor : MonoBehaviour {
		public string variableName;

		public bool getValue = true;

		public bool isNumber, changeMenu;

		public object pValue;



		UnityEngine.UI.Toggle tggl;
		UnityEngine.UI.Slider sldr;
		UnityEngine.UI.InputField inField;
		UnityEngine.UI.Text text;
		UnityEngine.UI.Dropdown drop;

		public void Start() {
			pValue = GlobalModifierList.Get(variableName);

			tggl = GetComponent<UnityEngine.UI.Toggle>();
			sldr = GetComponent<Slider>();
			inField = GetComponent<InputField>();
			text = GetComponent<Text>();
			drop = GetComponent<Dropdown>();

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
			if(drop != null) {
				if (changeMenu)
					AddDropDownMenus();
				else
					drop.value = (int)pValue;
			}
		}

		public void AddDropDownMenus() {
			List<Dropdown.OptionData> ls = new List<Dropdown.OptionData>();

			foreach(Resolution rs in ((List<Resolution>)GlobalModifierList.Get(variableName))) {
				Debug.Log(rs);
				ls.Add(new Dropdown.OptionData(rs.ToString()));
			}

			drop.options = ls;
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
					inField.text = GlobalModifierList.Get(variableName).ToString();
					return;
				}
				if(drop != null) {
					if (changeMenu)
						AddDropDownMenus();
					else
					drop.value = (int)GlobalModifierList.Get(variableName);
					
                }
			}
			if (getValue) {
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
				if (drop != null) {
					GlobalModifierList.SetObject(drop.value, variableName);
                }
			} else {
				if (tggl != null) {
					tggl.isOn = (bool)GlobalModifierList.Get(variableName);

					return;
				}
				if (sldr != null) {
					sldr.value = (float)GlobalModifierList.Get(variableName);

					return;
				}
				if (text != null) {
					if(isNumber)
						text.text = ((float)GlobalModifierList.Get(variableName)).ToString("F1");
					else
						text.text = GlobalModifierList.Get(variableName).ToString();
					return;
				}
			}
		}
	}
}