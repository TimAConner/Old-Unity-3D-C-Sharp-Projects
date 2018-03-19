/*
 

CUT Visual Novel Language - Tim A. Conner
Version 1.0
c.timmy@yahoo.com
© 2014 Tim A. Conner

 
 */
using System.Collections;
using UnityEngine;
[System.Serializable]

public class AnimationClass {
    public string Name;
    public string Animation;

    public AnimationClass (string NA, string AA) {
        Name = NA;
        Animation = AA;
    }
    public AnimationClass () {
        Name = "";
        Animation = "";
    }
}