#region License
/*
 * TestSocketIO.cs
 *
 * The MIT License
 *
 * Copyright (c) 2014 Fabio Panettieri
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
#endregion

using System;
using System.Collections;
using UnityEngine;
using SocketIO;
using UnityEngine.UI;
using System.Net;
using System.Xml.Linq;

public class TestSocketIO : MonoBehaviour
{
	public Transform real_earth;
	public Transform twin_earth;
	public GameObject ins_obj;
	public Text location_text;

	private SocketIOComponent socket;
    private Text txt;
	private GameObject cube1,cube2;

	public void Start() 
	{
		GameObject go = GameObject.Find("SocketIO");
		socket = go.GetComponent<SocketIOComponent>();
        
        txt = this.gameObject.GetComponent<Text>(); 

		socket.On("sendname", OnSendName);
		socket.On("img_from_phone",OnGetImage);
		socket.On ("location",OnGetLocation);
		socket.On ("DeviceOrientation",onRotation);
		socket.On("welcome",OnWelcome);
		socket.On("error", TestError);
		socket.On("close", TestClose);

	}
	
	public void OnSendName(SocketIOEvent e)
	{
		Debug.Log("[SocketIO] OnMessage : " + e.name + " " + e.data);
        
	}
    
	public void OnGetImage(SocketIOEvent e){
		Debug.Log ("on get image");

		string image_data = e.data.GetField ("buffer").ToString ();

		// get rid of "" as first and last character
		image_data=image_data.Substring (1,image_data.Length-2);
		//Debug.Log (image_data);

		// string length must a multiple of 4. If it is not a multiple of 4, then "=" characters are appended until it is
		int mod4 = image_data.Length % 4;
		if (mod4 > 0) {
			for (int i = 0; i < 4 - mod4; i++) {
				image_data += "=";
			}
		}
		Debug.Log (image_data.Length);

		try{
			// decode image from base64 to bytes.
			byte[] byte_asset1 = System.Convert.FromBase64String (image_data);

			// create new texture of the image. sieze doesn't matter
			Texture2D new_texture = new Texture2D (2,2);
			new_texture.LoadImage (byte_asset1);

			//display new image on the quad
			GameObject.Find ("Quad").GetComponent<MeshRenderer> ().material.mainTexture = new_texture;
			GameObject.Find ("Quad").GetComponent<MeshRenderer> ().material.shader= Shader.Find("Particles/Additive");

		}catch(Exception err){
			Debug.Log (err.Message);
		}


	}

	public void OnGetLocation(SocketIOEvent e){
		Debug.Log("[SocketIO] OnMessage : " + e.name + " " + e.data);
		//Geocode (e.data.GetField("location").ToString());
	}
		
	public void onRotation(SocketIOEvent e){
		//Debug.Log("[SocketIO] OnMessage : " + e.name + " " + e.data);

		float alpha = float.Parse (e.data.GetField ("alpha").ToString ());
		float beta = float.Parse (e.data.GetField ("beta").ToString ());
		float gamma = float.Parse (e.data.GetField ("gamma").ToString ());
		//Debug.Log ("alpha: "+alpha+" beta: "+beta+" gamma: "+gamma);
		real_earth.rotation=Quaternion.Euler (gamma,alpha,beta);

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

	public void OnWelcome(SocketIOEvent e)
	{
		Debug.Log("[SocketIO] On Welcome: " + e.name + " " + e.data);
	}
	
	
	public void TestError(SocketIOEvent e)
	{
		Debug.Log("[SocketIO] Error received: " + e.name + " " + e.data);
	}
	
	public void TestClose(SocketIOEvent e)
	{	
		Debug.Log("[SocketIO] Close received: " + e.name + " " + e.data);
	}

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

	public void Geocode(Vector2 coordinate){

		var requestUri = "http://maps.googleapis.com/maps/api/geocode/xml?latlng=" + coordinate.x + "," + coordinate.y + "&sensor=false";

		var request = WebRequest.Create(requestUri);
		var response = request.GetResponse();
		var xdoc = XDocument.Load(response.GetResponseStream());

		var result = xdoc.Element("GeocodeResponse").Element("result");
		if (result != null) {
			var locationElement= result.Element("formatted_address");
			Debug.Log ((string)locationElement);
			location_text.text = (string)locationElement;

		}
	}

	void LateUpdate(){
		Destroy (cube1);
		Destroy (cube2);
	}

}


