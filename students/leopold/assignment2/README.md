# STUDENT

{rebecca (marks) leopold}

## Assignment Number 2

# Basic 3d movements along vertical/horizontal using arrow keys. attempt to import a 3d render but had trouble figuing out how to get the box collider to work. 

using UnityEngine;
using System.Collections;


public class Ball : MonoBehaviour{

public float speed;
private Rigidbody rb;

void Start() {
rb = GetComponent<Rigidbody> ();
}


void FixedUpdate() {
float moveHorizontal = Input.GetAxis ("Horizontal"); //gets input from user on keyboard
float moveVertical = Input.GetAxis ("Vertical"); 

Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
rb.AddForce (movement * speed);

}

}
//public class ball : MonoBehaviour {
//	public float thrust;
//	public Rigidbody rb;
//	public float speed = 0.01f;
//
//	// Use this for initialization
//	void Start () {
//		rb = GetComponent<Rigidbody>().velocity = Vector3.right * speed;
//	
//	}
//	
//	// Update is called once per frame
//	void Update () {
//	
//	}
//
//	void FixedUpdate() {
//		rb.AddForce(transform.forward * thrust);
//
//}


using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

public GameObject Ball;
private Vector3 offset;



// Use this for initialization
void Start () {
offset = transform.position - Ball.transform.position;

}

// Update is called once per frame
void LateUpdate () {
transform.position = Ball.transform.position + offset;

}
}
