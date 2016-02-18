using UnityEngine;
using System.Collections;

public class Black_Hole_Transfer : MonoBehaviour {

	public GameObject transfer_to_hole;

	private Transform spaceship_pos;

	void  OnTriggerEnter(Collider other) {
		//if it's the spaceship

		spaceship_pos = GameObject.FindGameObjectWithTag ("Player").GetComponent<Transform> ();

		if(other.name=="player"){
			Vector3 destination_pos = transfer_to_hole.GetComponent<Transform> ().position;
			Vector3 current_pos = GetComponent<Transform> ().position;
			spaceship_pos.position=Vector3.Lerp (current_pos,destination_pos,0.9f);

			//Debug.Log ("destination_pos:"+destination_pos+"\t current_pos:"+current_pos);
		}

	}
}
