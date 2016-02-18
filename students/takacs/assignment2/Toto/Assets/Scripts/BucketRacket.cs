using UnityEngine;
using System.Collections;

public class BucketRacket : MonoBehaviour {

	public float speed = 30;
	public string axis = "Horizontal";
	private AudioSource snd;
	private Rigidbody2D rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		float h = Input.GetAxisRaw (axis);
		rb.velocity = new Vector2 (h, 0) * speed;

	}

	void OnCollisionEnter2D(Collision2D col){
		if (col.gameObject.name == "Rain10"){
			snd.Play ();
		}
			}
}
