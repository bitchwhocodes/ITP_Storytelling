using UnityEngine;
using System.Collections;

public class FloatCube : MonoBehaviour {

	// variables

	public float hoverSpeed = 0.0f; 
	public float hoverDistance = 0.0f; 

	private Rigidbody rigid;	// reference to the Rigidbody object in the game
	private float pOrig; 		// original position
	private float rotation;		// rotation of cube
	

	// Use this for initialization
	void Start () 
	{
		// attach variable to the game object 
		rigid = GetComponent<Rigidbody> ();
		pOrig = rigid.position.y;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// run at a set frame rate
	void FixedUpdate()
	{
		Vector3 pos = rigid.position;

		// calculate the y position so cube bounces on y axis based on sine curve
		float hover = pOrig + hoverDistance * Mathf.Sin (hoverSpeed * Time.time);

		// update the y value so cube moves on the y axis based on sine curve
		rigid.position = new Vector3(pos.x,hover, pos.z);

		// rotate the cube;
		rotation += 1.0f;
		rigid.rotation = Quaternion.Euler (rotation, 0.0f, rotation);

	}
}
