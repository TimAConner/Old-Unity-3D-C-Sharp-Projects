using System.Collections;
using UnityEngine;
[System.Serializable]

public class ChannelClass {
    public int ChannelID;
    public double MaxVolume;
    public double FadeIn;
    public double FadeOut;
    public float PlayedFor;
    //public AudioSource SoundData;

    public ChannelClass (int id) {
        ChannelID = id;
        MaxVolume = 0.0f;
        FadeIn = -1.0;
        FadeOut = -1.0;
        PlayedFor = 0.0f;
    }

}