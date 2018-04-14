using System.Collections;
using UnityEngine;

public class HexHoverScript : MonoBehaviour { //this is for if you mosue over a hex to highlight it
    public Color StartingColor;
    // Use this for initialization

    void Start () {
        if (gameObject.GetComponent<Renderer> ().material.shader.name != "FX/Water (simple)") {
            if (gameObject.GetComponent<Renderer> ().material.color != Color.black) {
                StartingColor = Color.white;
            }
        } else {
            gameObject.AddComponent<WaterSimple> ();
        }
    }
    void OnMouseOver () {

        if (StartingColor != Color.white && gameObject.GetComponent<Renderer> ().material.shader.name != "FX/Water (simple)") {
            gameObject.GetComponent<Renderer> ().material.color = Color.gray;
        } else {
            gameObject.GetComponent<Renderer> ().material.color = Color.gray;
        }
    }

    void OnMouseExit () {
        if (StartingColor != Color.white && gameObject.GetComponent<Renderer> ().material.shader.name != "FX/Water (simple)") {
            gameObject.GetComponent<Renderer> ().material.color = StartingColor;
        } else {
            if (gameObject.GetComponent<Renderer> ().material.shader.name != "FX/Water (simple)") {
                gameObject.GetComponent<Renderer> ().material.color = StartingColor;
            }
        }
    }
    public void GetColor (Color thisColor) {
        StartingColor = thisColor;
    }
}