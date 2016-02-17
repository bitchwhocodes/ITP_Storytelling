using UnityEngine;
using System.Collections;

public class AddCube : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			for (int y = 0; y < 5; y++) {
				for (int x = 0; x < 5; x++) {
					GameObject cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
					string nameNumber = (x + y).ToString ();
					cube.name = "Brick"+nameNumber;
					cube.transform.position = new Vector3 (Random.Range (-1f, 1f), Random.Range (-1f, 1f), 0);
					cube.AddComponent<Rigidbody> ();
					TrailRenderer tr = cube.AddComponent<TrailRenderer> ();
					tr.startWidth = 1.0f;
					tr.endWidth = 0.10f;
					tr.time = 1.0f;
				}

			}
		
		}
	}
}




