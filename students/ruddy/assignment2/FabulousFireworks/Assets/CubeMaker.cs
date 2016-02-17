using UnityEngine;
using System.Collections;

public class CubeMaker : MonoBehaviour {
	public GameObject brick;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			for (int y = 0; y < 5; y++) {
				for (int x = 0; x < 5; x++) {
					GameObject cube = Instantiate (brick) as GameObject;

					cube.transform.position = new Vector3 (Random.Range (-1f, 1f), Random.Range (-1f, 1f), 0);
	}
}
		}
	}
}