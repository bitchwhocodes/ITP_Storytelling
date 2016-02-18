using UnityEngine;
using System.Collections;

public class Smoke_Effect : MonoBehaviour {

	private Material smoke_material;
	private Color end_color = Color.white;

	void Start () {
		smoke_material= GetComponent<MeshRenderer>().material;
		end_color.a = 0;
	}
	

	void Update () {
		GetComponent<Transform> ().localScale *= 1.005f;

		//Debug.Log (GetComponent<MeshRenderer>().material.color.a);
		//GetComponent<MeshRenderer> ().material.color.a -= 0.1f;
		//smoke_material.SetColor ("_SpecColor",end_color);

		if (GetComponent<Transform> ().localScale.x> 2.0F) {
			Destroy (this.gameObject);
		}

	}

//	void OnBecomeInvisible(){
//		Destroy (this.gameObject);
//	}
}
