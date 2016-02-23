using UnityEngine;
using System.Collections;

public class moveBug : MonoBehaviour {

	public float speed = 30;
	private Rigidbody rbdy;
	private AudioSource snd;

	// Use this for initialization
	void Start () {

		rbdy = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate(){
		
		float v = Input.GetAxisRaw ("Jump");
		//rbdy.velocity = new Vector3 (0, v, 0) * speed;

		if(Input.GetKeyDown(KeyCode.Space)){
			
			rbdy.AddForce(new Vector3(0.0f, 30.0f, 0.0f));
		} 
		else if(Input.GetKeyUp(KeyCode.Space)) {
			rbdy.AddForce(new Vector3(0.0f, -30.0f, 0.0f));
		}
//

		Debug.Log("space bar pressed");

		//float h = Input.GetAxisRaw ("Horizontal");
		//rbdy.velocity = new Vector3 (h, 0, 0) * speed;
	}
//
//	void OnCollisionEnter(Collision col){
//
//		//info about the collision
//		//Debug.Log("collide");
//
//		if (col.gameObject.name == "Cube") {
//
//			float y = Vector3.distance (transform.position, col.transform.position, col.collider.bounds.size.y);
//			Vector3 direction = new Vector3 (1, y, 0).normalized;
//			rbdy.velocity = direction * speed;
//			snd.Play ();
//		}
//
//		if (col.gameObject.name == "RCube") {
//
//			float y = Vector3.distance (transform.position, col.transform.position, col.collider.bounds.size.y);
//			Vector3 direction = new Vector3 (-1, 0, 0).normalized;
//			rbdy.velocity = direction * speed;
//			snd.Play ();
//
//		}
}
