using UnityEngine;
using System.Collections;

public class floatBug : MonoBehaviour {

	public float hovSpeed = 0.0f; //this will show up in the Inspector because its pblic
	public float hovDist = 0.0f;

	private Rigidbody rigid;
	private float p0rig;
	private float rot;

	// Use this for initialization
	void Start () {

		//go to the component attached to the gameObject and get the rigidbody
		rigid = GetComponent<Rigidbody> ();

		//go to the position of the rigidbody and store the y
		p0rig = rigid.position.y;

	}

	// Update is called once per frame
	void Update () {

	}

	void FixedUpdate(){

		//
		Vector3 p = rigid.position;

		//use sin to make it move up and down
		float hover = p0rig + hovDist * Mathf.Sin(hovSpeed * Time.time);
		rigid.position = new Vector3 (p.x, hover, p.z);

		//rotate cube
		rot += 5.0f;
		rigid.rotation = Quaternion.Euler (rot, 0.0f, rot);
	}
}
