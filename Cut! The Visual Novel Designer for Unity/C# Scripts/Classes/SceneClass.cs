using System.Collections;
using UnityEngine;
[System.Serializable]
public class SceneClass {
    public string Name;
    public int LineNumber;
    public SceneClass (string NA, int LA) {
        Name = NA;
        LineNumber = LA;
    }
}