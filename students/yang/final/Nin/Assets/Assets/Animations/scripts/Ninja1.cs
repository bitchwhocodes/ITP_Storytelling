using UnityEngine;
using System.Collections;

public class Ninja1 : MonoBehaviour {

	// Use this for initialization


	private Animator anim;

	void Start () {
		anim = GetComponent<Animatior> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.RightArrow)) {
			anim.Play ("running");
		}
		if (Input.GetKeyUp(KeyCode.RightArrow)) {
			anim.Play ("idle");
		}

	}
}
