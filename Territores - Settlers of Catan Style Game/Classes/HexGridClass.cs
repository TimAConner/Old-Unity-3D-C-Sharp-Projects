using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class HexGridClass {
    public int x;
    public int y;
    public string Type; //type of the land 0water 1farm 2forest 3moutain 4hills
    public string Owner = ""; //the owners/viewers of land

    public string Building; //0none, 1hall, 2marekt, 3barracks, 4wall
    public int NextCycle = 3;

    public List<string> Viewers = new List<string> ();
    public List<string> Viewed = new List<string> ();
    public List<UnitClass> Units = new List<UnitClass> ();

    public bool Changed = false;

    public float Rotation = 0.0f;
    public float RotationB = 0.0f;

    public HexGridClass (int XA, int YA, string TA) {
        x = XA;
        y = YA;
        Type = TA;
        Building = "None";
    }
    public HexGridClass () {
        x = -1;
        y = -1;
        Type = "Water";
        Building = "None";
    }
    public HexGridClass (int XA, int YA) {
        x = XA;
        y = YA;
    }
    public HexGridClass (int XA, int YA, string TA, string OA, string BA) {
        x = XA;
        y = YA;
        Type = TA;
        Owner = OA;
        Building = BA;
    }

}