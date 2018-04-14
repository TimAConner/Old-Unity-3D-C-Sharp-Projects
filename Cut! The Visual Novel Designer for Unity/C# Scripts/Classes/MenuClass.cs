using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MenuClass {
    public string Name;
    public List<string> Lines = new List<string> ();

    public MenuClass (string NA, List<string> LA) {
        Name = NA;
        Lines = LA;
    }
}