using System.Collections;
using UnityEngine;
[System.Serializable]
public class ButtonClass : ButtonClassDefault {

    public string Name;
    public string Command;

    public ButtonClass (string NA, string CA, float XPosA, float YPosA) {
        Name = NA;
        Command = CA;
        XPos = XPosA;
        YPos = YPosA;
    }
    public ButtonClass (string NA, string CA, float XPosA, float YPosA, string TextureA) {
        Name = NA;
        Command = CA;
        XPos = XPosA;
        YPos = YPosA;
        Texture = TextureA;
    }
}