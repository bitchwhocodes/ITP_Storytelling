using UnityEngine;
using System.Collections;

public class mainCamera : MonoBehaviour {

	public GameObject Cube;
	private Vector3 offset;

	// Use this for initialization
	void Start () {
		offset = transform.position - Cube.transform.position;

	}

	// Update is called once per frame
	void LateUpdate () {
		transform.position = Cube.transform.position + offset;

	}
}
