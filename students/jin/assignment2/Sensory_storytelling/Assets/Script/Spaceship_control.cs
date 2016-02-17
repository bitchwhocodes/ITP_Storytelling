using UnityEngine;
using System.Collections;

public class Spaceship_control : MonoBehaviour {


	public GameObject bolt;
	public GameObject smoke;

	private float movementSpeed=10.0F;

	private Transform trans;
	private Rigidbody rb;
	private Transform smoke_spawn;

	private float fireRate=0.25f;
	private float nextFire=0.0f;

	private Vector3 smoke_position;
	private Quaternion smoke_rotation;

	// Use this for initialization
	void Start () {
		trans = GetComponent<Transform> ();
		rb = GetComponent<Rigidbody> ();
		smoke_spawn = GetComponent<Transform> ().Find ("smoke_spawn");
	}

	void Update () {

		//fire
		if (Input.GetKey (KeyCode.Space) && Time.time > nextFire){
			nextFire = Time.time + fireRate;
			Instantiate(bolt,trans.position,trans.rotation);
			//Debug.Log("instantiate a bolt");
		}
		
	}

	void FixedUpdate(){
		//change direction
		Vector3 rotate_angle = trans.rotation.eulerAngles;
		rotate_angle.y+=Input.GetAxis ("Horizontal");
		transform.rotation = Quaternion.Euler (rotate_angle);
		
		//go forward 
		if(Input.GetKey(KeyCode.Return)){
			rb.AddForce(trans.forward * movementSpeed);

			//eject smoke
			smoke_rotation=Quaternion.identity;
			smoke_rotation.eulerAngles=new Vector3(90,trans.rotation.eulerAngles.y,trans.rotation.eulerAngles.z);
			//smoke_position=new Vector3(trans.position.x,-0.2f,trans.position.z);
			Instantiate(smoke,smoke_spawn.position,smoke_rotation);

			//Debug.Log(trans.rotation.eulerAngles);
		}


		//trying to simulate resistance, end up using Drag in Rigidbody

		//if (rb.velocity.magnitude>0.1) {
		//	rb.AddForce (trans.forward* (-0.5f));
		//}
		//Debug.Log ("Velocity:"+rb.velocity.magnitude);

	}
}
