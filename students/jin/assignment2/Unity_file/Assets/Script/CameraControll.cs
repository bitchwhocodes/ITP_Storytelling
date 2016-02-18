using UnityEngine;
using System.Collections;

public class CameraControll : MonoBehaviour {

	private Transform camera_pos;
	private Vector3 player_pos;
	private float y_pos;

	// Use this for initialization

	void Start () {
		camera_pos = GetComponent<Transform> ();
		player_pos=GameObject.FindGameObjectWithTag ("Player").GetComponent<Transform> ().position;
		y_pos = camera_pos.position.y;
		camera_pos.position = new Vector3 (player_pos.x,y_pos,player_pos.z);


	}
	
	// Update is called once per frame
	void Update () {
		player_pos=GameObject.FindGameObjectWithTag ("Player").GetComponent<Transform> ().position;
		camera_pos.position = new Vector3 (player_pos.x,y_pos,player_pos.z);
	}
}
