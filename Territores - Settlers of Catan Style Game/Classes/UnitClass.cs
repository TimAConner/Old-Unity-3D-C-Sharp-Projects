using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitClass {

	public string Name = "";
	public string Owner = "";
	public int Id = 0;

	public float Food = 0;
	public float Wood = 0;
	public float Stone = 0;
	public float Coin = 0;

	public float Count = 0;

	public bool Selected = false;

	//public List<TwoIntegerList> Waves = new List<TwoIntegerList>(); // first is type, second is number

	public List<SoldierClass> Wave1 = new List<SoldierClass> ();
	public List<SoldierClass> Wave2 = new List<SoldierClass> ();
	public List<SoldierClass> Wave3 = new List<SoldierClass> ();

	public List<SoldierClass> Soldiers = new List<SoldierClass> ();
	public UnitClass (string name, string owner) {
		Name = name;
		Owner = owner;
	}
	public UnitClass (string name, int id, string owner, float count, List<SoldierClass> soldiers) {
		Name = name;
		Owner = owner;
		Count = count;
		Id = id;
		Soldiers = soldiers;
	}
	public UnitClass () {

	}

	public bool IsAlive () {
		if (Wave1.Count > 0 || Wave2.Count > 0 || Wave3.Count > 0 || Soldiers.Count > 0) {

			return true;
		} else {

			return false;
		}
	}

}