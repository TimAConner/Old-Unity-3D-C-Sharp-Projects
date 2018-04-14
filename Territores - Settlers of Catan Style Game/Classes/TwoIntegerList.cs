using System.Collections;
using UnityEngine;
[System.Serializable]

public class TwoIntegerList {
    public int i1 = 0;
    public int i2 = 0;
    public TwoIntegerList (int x, int y) {
        i1 = x;
        i2 = y;
    }

    public TwoIntegerList () {
        i1 = 0;
        i2 = 0;
    }
}