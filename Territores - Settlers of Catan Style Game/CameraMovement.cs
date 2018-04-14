using System.Collections;
using UnityEngine;

//This script is for teh basic WASD camera movment

public class CameraMovement : MonoBehaviour {

    void Update () {
        if (Input.GetKey (KeyCode.UpArrow)) {
            transform.Translate (Vector3.up * Time.deltaTime * 10);
        }
        if (Input.GetKey (KeyCode.DownArrow)) {
            transform.Translate (Vector3.down * Time.deltaTime * 10);

        }
        if (Input.GetKey (KeyCode.LeftArrow)) {
            transform.Translate (Vector3.left * Time.deltaTime * 10);
        }
        if (Input.GetKey (KeyCode.RightArrow)) {
            transform.Translate (Vector3.right * Time.deltaTime * 10);
        }

    }

}