using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Running_Sheep : MonoBehaviour {

	public GameObject prefab_sheep;

	private float radius=3.0f;
	private Vector3 pluto_pos;
	private Quaternion rotation;
	private List<GameObject> sheep_list;

	void Start () {
		pluto_pos= GetComponent<Transform>().position;
		rotation = Quaternion.identity;
		rotation.eulerAngles = new Vector3(90, 0, 0);
		sheep_list = new List<GameObject> ();

		for (int angle=0; angle<360; angle+=60) {
			float radians=angle*Mathf.PI/180;
			Vector3 sheep_pos=new Vector3(pluto_pos.x+radius*Mathf.Cos(radians),pluto_pos.y,pluto_pos.z+radius*Mathf.Sin(radians));
			//Debug.Log("Cos("+angle+"):"+Mathf.Cos(radians));

			GameObject new_sheep=(GameObject)Instantiate (prefab_sheep,sheep_pos,Quaternion.identity);

			new_sheep.GetComponent<Sheep_script>().original_angle=angle;

			sheep_list.Add(new_sheep);
		}
	}
	
	// Update is called once per frame
	void Update () {
		//rotate the sheeps
//		foreach(GameObject sheep in sheep_list){
//
//		}
	}


}
