using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerClass {
	public string Name = "None";

	//Resources
	public float Wood;
	public float Food;
	public float Stone;
	public float Coin;
	public float TurnPoints;

	public string Color = "";

	//Messages/Trades
	public List<AlliesClass> Allies = new List<AlliesClass> ();

	public List<SettingsClass> Upgrades = new List<SettingsClass> ();

	public List<MessageClass> Messages = new List<MessageClass> ();

	public PlayerClass () {
		Wood = 10;
		Food = 10;
		Stone = 10;
		Coin = 10;
		TurnPoints = 10;
		Color = "";
	}
	public PlayerClass (string name) {
		Name = name;
		Wood = 10;
		Food = 10;
		Stone = 10;
		Coin = 10;
		TurnPoints = 10;
		Color = "";
	}
}