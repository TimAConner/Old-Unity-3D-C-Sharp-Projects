using System.Collections;
using UnityEngine;
[System.Serializable]
public class MovieClass {
    public string Name;
    public string Movie; //its file name
    public MovieTexture MovieFile;
    public MovieClass () {
        Name = "";
        Movie = null;
    }
    public MovieClass (string NA, string MA) {

        Name = NA;
        Movie = MA;
        MovieFile = (MovieTexture) Resources.Load (Movie);

    }

}