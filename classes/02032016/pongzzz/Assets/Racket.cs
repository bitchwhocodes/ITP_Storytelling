using UnityEngine;
using System.Collections;

public class Racket : MonoBehaviour {

	public float speed = 30;
	public string axis = "Vertical";
	private Rigidbody2D rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate(){
		float v = Input.GetAxisRaw (axis);
		rb.velocity = new Vector2 (0, v) * speed;
	}
}
