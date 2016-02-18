using UnityEngine;
using System.Collections;

public class Sheep_script : MonoBehaviour {

	public float original_angle;

	private float radius=3.0f;
	private Vector3 pluto_pos;

	void Start(){
		//Debug.Log (original_angle);
		pluto_pos= GameObject.FindGameObjectWithTag("pluto").GetComponent<Transform>().position;
	}

	//make sheep rotating
	void FixedUpdate(){
		original_angle += 2;
		float radians=original_angle*Mathf.PI/180;

		Vector3 sheep_pos=new Vector3(pluto_pos.x+radius*Mathf.Cos(radians),pluto_pos.y,pluto_pos.z+radius*Mathf.Sin(radians));
		GetComponent<Transform> ().position = sheep_pos;

	}

	void OnTriggerEnter(Collider other) {
		//Debug.Log (other.name);
		if (other.name == "Bolt(Clone)") {
			Destroy (other.gameObject);
			Destroy (this.gameObject);
			//Debug.Log ("sheep get shot!");
		}
	}
}
