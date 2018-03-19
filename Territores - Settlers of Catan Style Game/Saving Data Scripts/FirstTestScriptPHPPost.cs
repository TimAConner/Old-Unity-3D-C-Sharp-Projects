using System.Collections;
using UnityEngine;

public class FirstTestScriptPHPPost : MonoBehaviour {
    public string url = "http://timaconner.com/test.php";

    // Use this for initialization
    void Start () {
        StartCoroutine (StartB ());
        StopCoroutine ("StartB");
    }
    IEnumerator StartB () {
        WWWForm form = new WWWForm ();
        form.AddField ("name", "hellosdasd");
        WWW download = new WWW (url, form);
        yield return download;
        if ((!string.IsNullOrEmpty (download.error))) {
            print ("Error downloading: " + download.error);
        } else {
            Debug.Log (download.text);
        }

    }

}