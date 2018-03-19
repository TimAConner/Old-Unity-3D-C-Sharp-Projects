using System.Collections;
using UnityEngine;

public class BoatWave : MonoBehaviour {
	float left = 0.0f;
	float right = 0.0f;
	float up = 0.0f;
	// Use this for initialization
	void Start () {
		left = UnityEngine.Random.Range (-5f, 5f);
		right = UnityEngine.Random.Range (-5f, 5f);
		up = UnityEngine.Random.Range (-5f, 5f);

	}

	// Update is called once per frame
	void Update () {
		transform.rotation = Quaternion.Euler (Mathf.PingPong (Time.time * 0.5f, left), Mathf.PingPong (Time.time * 0.5f, right), Mathf.PingPong (Time.time * 0.5f, up));
	}
}