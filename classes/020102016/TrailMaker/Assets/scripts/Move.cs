using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {
	public Vector3 velocity = Vector3.zero;
	public float speed = 25f;
	// Use this for initialization
	void Start () {
		goRandom ();
	}
	
	// Update is called once per frame
	void Update () {



		transform.position += (velocity * Time.deltaTime);

	
	}

	void goRandom(){
		InvokeRepeating("goRandomDirection", 1.0f, 0.3F);
	}
	void goRandomDirection(){
		velocity = setRandomDirection ();

	}
	Vector3 setRandomDirection(){
	
		float x = Random.Range (-50f, 50f);
		float y = Random.Range (-50f, 50f);
		return new Vector3 (x, y, 0);
	}
}
