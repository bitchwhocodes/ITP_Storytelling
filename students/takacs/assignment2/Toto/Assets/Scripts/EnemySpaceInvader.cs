using UnityEngine;
using System.Collections;

public class EnemySpaceInvader : MonoBehaviour {

	// Public variable that contains the speed of the enemy
	public float speed= 5;
	private Rigidbody2D rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		rb.velocity = Vector2.down * speed;

		// Make the enemy rotate on itself
//		rigidbody2D.angularVelocity = Random.Range(-200, 200);

		// Destroy the enemy in 3 seconds,
		// when it is no longer visible on the screen
		Destroy(gameObject, 3);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter2D(Collision2D col){
		
		if (col.gameObject.name == "Bucket") {
			rb.velocity = Vector2.zero;
		}
	}
}
