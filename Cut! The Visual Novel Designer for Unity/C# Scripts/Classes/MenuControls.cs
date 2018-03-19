using System.Collections;
using UnityEngine;
[System.Serializable]
public class MenuControls {
    public string Type;

    public float XLocation = 0;
    public float YLocation = 0;
    public float Width = 0;
    public float Height = 0;
    public string Text = "";
    public string Command;
    public float Fadein = 0;
    public float CurrentFade;
    public Texture2D ButtonTexture;
    public Texture2D OnHoverTexture;

    public MenuControls (float XLA, float YLA, float WA, float HA, string TA, string CA, string TY, float FA, Texture2D BT, Texture2D OHT) {
        XLocation = XLA;
        YLocation = YLA;
        Width = WA;
        Height = HA;
        Text = TA;
        Command = CA;
        Type = TY;
        Fadein = FA;
        if (FA == 0.0) {
            CurrentFade = 1.0f;
        } else {
            CurrentFade = 0.0f;
        }

        ButtonTexture = BT;
        OnHoverTexture = OHT;

    }
    public MenuControls (float XLA, float YLA, float WA, float HA, string TA, string CA, string TY, float FA, Texture2D BT) {
        XLocation = XLA;
        YLocation = YLA;
        Width = WA;
        Height = HA;
        Text = TA;
        Command = CA;
        Type = TY;
        Fadein = FA;
        if (FA == 0.0) {
            CurrentFade = 1.0f;
        } else {
            CurrentFade = 0.0f;
        }
        ButtonTexture = BT;

    }

    public MenuControls (float XLA, float YLA, float WA, float HA, string TA, string CA, string TY, float FA) {
        XLocation = XLA;
        YLocation = YLA;
        Width = WA;
        Height = HA;
        Text = TA;
        Command = CA;
        Type = TY;
        Fadein = FA;
        if (FA == 0.0) {
            CurrentFade = 1.0f;
        } else {
            CurrentFade = 0.0f;
        }
    }
}