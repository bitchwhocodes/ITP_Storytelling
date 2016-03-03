using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class DirectionScript : MonoBehaviour {
	public SerialPort sp = new SerialPort ("/dev/cu.usbmodem1411", 9600);


	// Use this for initialization
	void Start () {
		sp.Open ();
		sp.ReadTimeout = 1000;
	}
	
	// Update is called once per frame
	void Update () {
		if (sp.IsOpen) {
			
			string move = sp.ReadLine ();
			Debug.Log ("number of characters in the move var"+move.Length);

			Debug.Log (move);
			if (move.IndexOf("0")!=-1) {
				Debug.Log ("move is zero");
				transform.Translate(1,0,0);
				//print(0);
		
			}else if(move.IndexOf("1")!=-1){
				Debug.Log ("move is equal to 1");
				transform.Translate(-1,0,0);
				print(1);
			}
		}
	}
}
