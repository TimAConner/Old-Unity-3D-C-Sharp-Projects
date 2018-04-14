using System.Collections;
using UnityEngine;
[System.Serializable]
public class GridPoint {
    public int x;
    public int y;
    public GridPoint () {
        x = -1;
        y = -1;
    }
    public GridPoint (int XA, int YA) {
        x = XA;
        y = YA;
    }

    public GridPoint (HexGridClass hex) {
        x = hex.x;
        y = hex.y;
    }

}