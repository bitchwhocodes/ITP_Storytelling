/*
This snippet of code calculate the latitude and longitude on the globe according to the earth's rotation,
and get the address using Google Reverse Geocoding API
*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using System.Net;
using System.Xml.Linq;
//using Geocoding;
//using Geocoding.Google;


public class Earth_script : MonoBehaviour {

	public GameObject ins_obj;
	public Text location_text;

	
	private Transform real_earth;
	private Transform twin_earth;
	private float speed = 5.0f;
	private GameObject cube1,cube2;


	void Start () {
		real_earth = GetComponent<Transform> ();
		twin_earth = GameObject.Find ("Earth2").GetComponent<Transform>();

	}

	void Update () {
		
		if (Input.GetMouseButton(0)){  
			real_earth.RotateAround (real_earth.position,new Vector3(0.0f, 1.0f, 0.0f),Input.GetAxisRaw ("Mouse X") * (-20) * Time.deltaTime * speed);
			real_earth.RotateAround (real_earth.position,new Vector3(0.0f, 0.0f, 1.0f),Input.GetAxisRaw ("Mouse Y") * 20 * Time.deltaTime * speed);

			cube1 = (GameObject)Instantiate (ins_obj,new Vector3(0.0f,25.0f,0.0f),Quaternion.identity);
			Transform cube_trans1 = cube1.GetComponent<Transform> ();
			cube_trans1.parent = real_earth;

			//Debug.Log (cube_trans1.position+"\t"+cube_trans1.localPosition+"\t"+cube_trans1.localRotation.eulerAngles);

			cube2 = (GameObject)Instantiate (ins_obj,new Vector3(0.0f,0.0f,0.0f),Quaternion.identity);
			Transform cube_trans2 = cube2.GetComponent<Transform> ();
			cube_trans2.parent = twin_earth;
			cube_trans2.localPosition = cube_trans1.localPosition;
			cube_trans2.localRotation = cube_trans1.localRotation;
			//Debug.Log (cube_trans2.position);

			Vector2 coordinate = CartesianToPolar (cube_trans2.position);
			Geocode (coordinate);

		}
	}

	void LateUpdate(){
		Destroy (cube1);
		Destroy (cube2);
	}

	//Convert unity position(Vector3, xyz) to coordinates (Vector2, latitude and longitude)
	public Vector2 CartesianToPolar(Vector3 _point){

		Vector2 polar;

		//calculate longitude
		polar.y = Mathf.Atan2(_point.z,_point.x);
	
		//calculate latitude
		float xzLen = new Vector2 (_point.x, _point.z).magnitude;
		polar.x = Mathf.Atan2(_point.y,xzLen);

		//convert to deg
		polar *= Mathf.Rad2Deg;
		return polar;
	}




	//Google Geocoding api for address0
	public void Geocode(Vector2 coordinate){

		var requestUri = "http://maps.googleapis.com/maps/api/geocode/xml?latlng=" + coordinate.x + "," + coordinate.y + "&sensor=false";
	
		var request = WebRequest.Create(requestUri);
		var response = request.GetResponse();
		var xdoc = XDocument.Load(response.GetResponseStream());

		var result = xdoc.Element("GeocodeResponse").Element("result");
		if (result != null) {
			var locationElement= result.Element("formatted_address");
			//Debug.Log (locationElement);
			location_text.text = (string)locationElement;

		}
	

//		var locationElement = result.Element("geometry").Element("location");
//		var lat = locationElement.Element("lat");
//		var lng = locationElement.Element("lng");
//		Debug.Log ("latitude:"+lat+"\tlongitude"+lng);


//another snippet of code that works
//		System.Xml.XmlDocument doc = new System.Xml.XmlDocument ();
//		doc.Load("http://maps.googleapis.com/maps/api/geocode/xml?latlng=" + coordinate.x + "," + coordinate.y + "&sensor=false");
//		System.Xml.XmlNode element = doc.SelectSingleNode("//GeocodeResponse/status");
//		if (element.InnerText == "ZERO_RESULTS"){
//			Debug.Log ("No data available for the specified location");
//		}
//		else{
//			Debug.Log ("It works!");
//		}
	}

}