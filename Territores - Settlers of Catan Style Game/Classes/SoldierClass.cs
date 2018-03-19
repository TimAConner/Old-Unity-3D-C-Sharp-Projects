using System.Collections;
using UnityEngine;
[System.Serializable]
public class SoldierClass {
	public int Type = 0;
	public int Wounds = 3;
	public SoldierClass () {

	}
	public SoldierClass (int x) {
		Type = x;
	}
}