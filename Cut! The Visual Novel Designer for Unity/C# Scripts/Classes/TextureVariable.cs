/*
 

CUT Visual Novel Language - Tim A. Conner
Version 1.0
c.timmy@yahoo.com
© 2014 Tim A. Conner

 
 */
using System.Collections;
using UnityEngine;
[System.Serializable]

public class TextureVariable //Image + Background variable
{
    public string Name = "";
    public Texture2D Texture;

    public TextureVariable (string NA, string TA) {
        Name = NA; //var name of image
        Texture = Resources.Load (TA) as Texture2D; //Loads the texture from TA which is texture
    }
    public TextureVariable (int Options = 0) {
        Name = "";
    }

}