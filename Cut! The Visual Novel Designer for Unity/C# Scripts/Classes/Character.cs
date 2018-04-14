/*
 

CUT Visual Novel Language - Tim A. Conner
Version 1.0
c.timmy@yahoo.com
� 2014 Tim A. Conner

 
 */
using System.Collections;
using UnityEngine;
[System.Serializable]

public class Character {
    public string CharacterName = "";
    public string CharacterVarName = "";
    public string TextColor = "";
    public Character (string CNA, string CVNA, string TCA) {
        CharacterName = CNA;
        CharacterVarName = CVNA;
        TextColor = TCA;
    }
    public Character () {
        CharacterName = "";
        CharacterVarName = "";
        TextColor = "";
    }
}