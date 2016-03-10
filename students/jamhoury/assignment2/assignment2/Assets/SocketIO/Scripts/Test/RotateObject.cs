using UnityEngine;
using System.Collections;

public class RotateObject : MonoBehaviour {

	public float speed = 2f;

	void Update () {

		transform.Rotate (Vector3.forward, speed);

	}
}
