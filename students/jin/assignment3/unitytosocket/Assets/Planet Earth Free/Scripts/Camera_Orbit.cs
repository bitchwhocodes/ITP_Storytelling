using UnityEngine;
using System.Collections;

public class Camera_Orbit : MonoBehaviour {
	public Transform target;

	private Transform trans;
	private float speed = 5.0f;
	private Vector3 point;

	private float x = 0.0f;
	private float y = 0.0f;

	// Use this for initialization
	void Start () {
		
		trans=GetComponent<Transform>();
		trans.position = GameObject.Find ("Main Camera").GetComponent<Transform> ().position;
		point = target.position;
		//trans.LookAt(target);
	}
		
	void Update()
	{
		if (Input.GetMouseButton(1)){
			//Debug.Log(Input.GetAxis("Mouse X"));

			//translate the distance of mouse movement into angle, and then trans.RotateAround
			//trans.RotateAround(point, new Vector3(0.0f, 1.0f, 0.0f), Input.GetAxisRaw("Mouse X") * (-10) * Time.deltaTime * speed);


			//trans.RotateAround(point, new Vector3(0.0f, 0.0f, 1.0f), Input.GetAxisRaw("Mouse Y") * 10 * Time.deltaTime * speed);

			//trans.LookAt(target);

			//Debug.Log (trans.position);
			//Debug.Log (trans.rotation.eulerAngles);

			y = Input.GetAxisRaw ("Mouse X") * 10 * Time.deltaTime * speed;
			//Vector3 temp=new Vector3(target.rotation.eulerAngles.x, y,target.rotation.eulerAngles.z);
			target.rotation.eulerAngles.Set (target.rotation.eulerAngles.x, y,target.rotation.eulerAngles.z);
			Debug.Log (trans.rotation.eulerAngles);
		}
	}
}
