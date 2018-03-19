using System.Collections;
using UnityEngine;

public class GrassWave : MonoBehaviour {
	float left = 0.0f;
	float right = 0.0f;
	// Use this for initialization
	void Start () {
		left = UnityEngine.Random.Range (-0.5f, 0.5f);
		right = UnityEngine.Random.Range (0.5f, 2.5f);

	}

	// Update is called once per frame
	void Update () {
		transform.rotation = Quaternion.Euler (0, Mathf.PingPong (Time.time * 0.5f, right), Mathf.PingPong (Time.time * 0.5f, left));
	}
}