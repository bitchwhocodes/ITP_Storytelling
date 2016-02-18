using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public float speed=10.0f;

	private Vector3 player_vel;
	// Use this for initialization
	void Start () {
		player_vel = GameObject.FindGameObjectWithTag ("Player").GetComponent<Transform> ().forward;
		GetComponent<Rigidbody> ().velocity = player_vel *speed;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnBecameInvisible(){
		Destroy (this.gameObject);
		//Debug.Log ("Destroyed the bullet!");
	}
}
