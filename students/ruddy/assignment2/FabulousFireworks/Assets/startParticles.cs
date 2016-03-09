using UnityEngine;
using System.Collections;

public class startParticles : MonoBehaviour {
	public GameObject PS;
	// Use this for initialization
	void Start () {
		//gameObject.Get
		PS.SetActive(false);

		//Debug.Log ("I found the particle emitter: " + ps.gameObject.name);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			PS.SetActive (true);
			Invoke ("StopPS", 3.0f);


	}
			
}

	void StopPS () {
		PS.SetActive (false);
	}
}