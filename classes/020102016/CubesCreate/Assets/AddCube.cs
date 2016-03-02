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
					cube.transform.localScale = new Vector3(4.0f,4.0f,4.0f);
					cube.transform.position = new Vector3 (x, y,0);
					cube.GetComponent<Renderer> ().material.SetColor ("_Color", Color.cyan);
					cube.AddComponent<Rigidbody> ();
					cube.GetComponent<Rigidbody> ().mass = Random.Range (0.1f, 10.0f);
					TrailRenderer tr = cube.AddComponent<TrailRenderer> ();
					tr.startWidth = Random.Range (10.0f, 1.0f);
					tr.endWidth =Random.Range (10.0f, 1.0f);
					tr.time = Random.Range (0.5f, 5.0f);
					// Get the material list of the trail as per the scripting API.
					Material trail = cube.GetComponent<TrailRenderer>().material;
					//cube.transform.position = Vector3 (x, y, 0);
					// Set the color of the material to tint the trail.
					trail.SetColor("_Color", Color.green);
					cube.transform.parent = this.gameObject.transform;
				}

			}
		
		}
	}
}




