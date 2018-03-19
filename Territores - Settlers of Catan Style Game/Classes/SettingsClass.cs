using System.Collections;
using UnityEngine;
[System.Serializable]
public class SettingsClass {
	public string Name;
	public string Value;
	public SettingsClass (string x, string y) {
		Name = x;
		Value = y;
	}
	public SettingsClass () {
		Name = "";
		Value = "";
	}
	public SettingsClass (string x) {
		Name = x;
	}

	public float GetValue () {
		return (float) System.Convert.ToDouble (Value);
	}
	public void SetValue (float x) {
		Value = "" + x;
	}
	public void SetValue (string x) {
		Value = x;
	}
}