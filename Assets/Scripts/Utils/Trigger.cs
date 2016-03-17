using UnityEngine;
using System.Collections;
using System;

namespace Utils {

	[Serializable]
	public class Toggle {
		bool _value;
		[SerializeField]
		public bool value {
			get {
				return _value;
			}
			set {
				if (_value != value) {
					Debug.Log("value changed!");
					Debug.Log((OnChange != null) + "|" + (OnChangeValue != null));
					if (OnChangeValue != null)
						OnChangeValue(value);
					if (OnChange != null)
						OnChange();
                }
			}
		}
		public Toggle() {
			_value = false;
		}

		public Toggle(Action onChange) {
			_value = false;
			OnChange += onChange;
		}
		public Toggle(Action<bool> onChange) {
			_value = false;
			OnChangeValue += onChange;
		}

		public Toggle(bool v) {
			_value = v;
		}

		public void Set() {
			value = !_value;
		}
		public void Set(bool v) {
			value = v;
		}


		public Action<bool> OnChangeValue;
		public Action OnChange;
	}

	[Serializable]
	public class Trigger {
		bool _value;
		[SerializeField]
		public bool value {
			get {
				return _value;
			}
			set {
				if (_value != value) {
					if (OnNegative != null && value == false)
						OnNegative();
					else if (OnPositive != null && value == true)
						OnPositive();

					if (OnChange != null)
						OnChange(value);
				}
				_value = value;
			}
		}
		public Trigger() {
			_value = false;
		}

		public Trigger(Action onNegative, Action onPositive) {
			_value = false;
			OnNegative += onNegative;
			OnPositive += onPositive;
		}
		public Trigger(Action<bool> onChange) {
			_value = false;
			OnChange += onChange;
		}

		public Trigger(bool v) {
			_value = v;
		}

		public void Set() {
			value = !_value;
		}
		public void Set(bool v) {
			value = v;
		}


		public Action<bool> OnChange;
		public Action OnNegative, OnPositive;
	}
}