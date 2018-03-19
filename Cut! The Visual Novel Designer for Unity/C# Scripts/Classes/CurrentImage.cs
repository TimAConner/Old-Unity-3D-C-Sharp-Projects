/*
 

CUT Visual Novel Language - Tim A. Conner
Version 1.0
c.timmy@yahoo.com
© 2014 Tim A. Conner

 
 */
using System.Collections;
using UnityEngine;
[System.Serializable]

public class CurrentImage {
    public string Name;
    public double Xpos;
    public double Ypos;
    public float Height;
    public float Width;
    public double Alpha;
    public string Animation;

    // Use this for initialization

    public CurrentImage (string NA, float XA, float YA, float HA, float WA, double FA) {
        Xpos = XA;
        Ypos = YA;
        Height = HA;
        Width = WA;
        Name = NA;
        Alpha = FA;
    }
    public CurrentImage (string NA, float XA, float YA, float HA, float WA, double FA, string AA) {
        Xpos = XA;
        Ypos = YA;
        Height = HA;
        Width = WA;
        Name = NA;
        Animation = AA;
        Alpha = FA;
    }
}