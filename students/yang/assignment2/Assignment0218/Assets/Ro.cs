using UnityEngine;
using System.Collections;

public class Ro : MonoBehaviour {

	void Start () {

	}

	// Update is called once per frame

	void Update () 
	{

		//if (Input.GetKeyDown (KeyCode.R)) {
		transform.Rotate (new Vector3 (75, 75,75) * Time.deltaTime);

		//}



	}

}


