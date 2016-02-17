using UnityEngine;
using System.Collections;

public class MoveCube : MonoBehaviour {

	public float speed = 10;

	private string hAxis = "Horizontal";
	private string vAxis = "Vertical";

	private Rigidbody rb; //access cube

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();

	}
	
	// Update is called once per frame
	void Update () {

		//keep cube in camera view
		//code taken from: http://answers.unity3d.com/questions/799656/how-to-keep-an-object-within-the-camera-view.html

		Vector3 pos = Camera.main.WorldToViewportPoint (transform.position);
		pos.x = Mathf.Clamp (pos.x, 0.15f, 0.85f);
		pos.y = Mathf.Clamp(pos.y,0.0f,1.0f);
		transform.position = Camera.main.ViewportToWorldPoint(pos);

	}

	// called on regular basis. Good for physics/mathematical calculation
	void FixedUpdate(){
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			float v = Input.GetAxisRaw (hAxis);
			rb.velocity = new Vector2 (v, 0) * speed;
		}
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			float v = Input.GetAxisRaw (hAxis);
			rb.velocity = new Vector2 (v, 0) * speed;
		}

		//broken - not working. Restriction of clamp to camera view call?
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			float v = Input.GetAxisRaw (vAxis);
			rb.velocity = new Vector2 (0, v) * speed;
		}
		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			float v = Input.GetAxisRaw (vAxis);
			rb.velocity = new Vector2 (0, v) * speed;
		}


//		float v = Input.GetAxisRaw (hAxis);
//		rb.velocity = new Vector2 (v, 0) * speed;

	}
}
