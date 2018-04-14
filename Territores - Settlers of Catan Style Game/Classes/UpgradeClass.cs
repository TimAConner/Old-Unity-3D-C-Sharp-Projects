using System.Collections;
using UnityEngine;
[System.Serializable]
public class UpgradeClass {
	public string Name;
	public string Value;

	public float Food = 0;
	public float Wood = 0;
	public float Stone = 0;
	public float Coin = 0;
	public UpgradeClass (string x, string y, float f = 0, float w = 0, float s = 0, float c = 0) {
		Name = x;
		Value = y;
		Food = f;
		Wood = w;
		Stone = s;
		Coin = c;
	}
	public UpgradeClass () {
		Name = "";
		Value = "";
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