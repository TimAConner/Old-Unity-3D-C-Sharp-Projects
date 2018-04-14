using System.Collections;
using UnityEngine;
[System.Serializable]

public class StringIntClass {
    public int i = 0;
    public string s = "";
    public StringIntClass (int x, string y) {
        i = x;
        s = y;
    }
    public StringIntClass () {
        i = 0;
        s = "";
    }
}