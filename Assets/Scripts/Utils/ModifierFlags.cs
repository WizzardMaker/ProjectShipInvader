using System;
using System.Collections.Generic;

namespace Utils.Modifier {

	public sealed class ModifierList {
		public Dictionary<string, bool> modifierBool;
		public Dictionary<string, float> modifierFloat;

		public ModifierList() {
			modifierBool = new Dictionary<string, bool>();
			modifierFloat = new Dictionary<string, float>();
		}
		private ModifierList(Dictionary<string, bool> mb, Dictionary<string, float> mf) {
			modifierBool = mb;
			modifierFloat = mf;
		}

		public void AddModifier(bool value, string name) {
			if (modifierBool.ContainsKey(name)) {
				throw (new OverflowException("The modifier has allready a key called: \"" + name + "\"!"));
			}

			modifierBool.Add(name, value);
		}
		public void AddModifier(float value, string name) {
			if (modifierFloat.ContainsKey(name)) {
				throw (new OverflowException("The modifier has allready a key called: \"" + name + "\"!"));
			}

			modifierFloat.Add(name, value);
		}

		public bool GetBoolean(string name) {
			bool temp;
			modifierBool.TryGetValue("name", out temp);
			return temp;
		}
		public float GetFloat(string name) {
			float temp;
			modifierFloat.TryGetValue("name", out temp);
			return temp;
		}

		public void GetReferenceBoolean(string name, ref bool value) {
			bool temp;
			modifierBool.TryGetValue("name", out temp);
			value = temp;
		}
		public void GetReferenceFloat(string name, ref float value) {
			float temp;
			modifierFloat.TryGetValue("name", out temp);
			value = temp;
		}

		public void SetBoolean(string name, bool value) {
			bool temp;
			modifierBool.TryGetValue("name", out temp);
		}
		public void SetFloat(string name, float value) {
			float temp;
			modifierFloat.TryGetValue("name", out temp);
		}
	}

	public sealed class VariableReference {
		public Func<object> Get { get; private set; }
		public Action<object> Set { get; private set; }
		public VariableReference(Func<object> getter, Action<object> setter) {
			Get = getter;
			Set = setter;
		}
	}

	public static class GlobalModifierList {
		public static Dictionary<string, VariableReference> modifier = new Dictionary<string, VariableReference>();

		public static void AddModifier(string key, Func<object> getter, Action<object> setter) {
			modifier.Add(key, new VariableReference(getter, setter));
		}

		public static void SetObject(object v, string name) {
			modifier[name].Set(v);
        }

		public static bool GetBool(string key) {
			if(modifier[key].Get() is bool) {
				return (bool)modifier[key].Get();
			} else {
				throw (new InvalidCastException("The modifier can not be casted to BOOL! Check your type! key: " + key));
			}

		}
		public static float GetFloat(string key) {
			if (modifier[key].Get() is float) {
				return (float)modifier[key].Get();
			} else {
				throw (new InvalidCastException("The modifier can not be casted to FLOAT! Check your type! key: " + key));
			}

		}
		public static object Get(string key) {
			return modifier[key].Get();
		}

		public static VariableReference GetRef(string key) {
			return modifier[key];
        }

		public static Func<object> GetGetter(string key) {
			return modifier[key].Get;
		}

		public static Action<object> GetSetter(string key) {
			return modifier[key].Set;
		}
	}
}