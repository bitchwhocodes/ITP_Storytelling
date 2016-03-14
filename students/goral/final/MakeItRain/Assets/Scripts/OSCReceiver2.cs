using UnityEngine;
using System.Collections;



public class OSCReceiver2 : MonoBehaviour {
    


public string RemoteIP = "127.0.0.1"; //127.0.0.1 signifies a local host (if testing locally
public int SendToPort= 9000; //the port you will be sending from
public int ListenerPort = 8050; //the port you will be listening on
public Transform controller;
public string gameReceiver = "Cube"; //the tag of the object on stage that you want to manipulate
private Osc handler;
private int yRot=0; //the rotation around the y axis

private int xRot = 0; //the rotation around the x axis

private int zRot = 0; //the rotation around the z axis
private float scaleVal = 1;
private float xVal=0;

private int scaleValX =1;
private int scaleValY = 1;
private int scaleValZ = 1;
	// Use this for initialization
	void Start () {
        
  
    UDPPacketIO udp = GetComponent<UDPPacketIO>();
   Debug.Log(udp);
	udp.init(RemoteIP, SendToPort, ListenerPort);
	handler = GetComponent<Osc>();
	handler.init(udp);
	handler.SetAllMessageHandler(AllMessageHandler);
	
	}
	
	// Update is called once per frame
	void Update () {
        GameObject go = GameObject.Find(gameReceiver) as GameObject;
      
        go.transform.Rotate(0, yRot, 0);
        go.transform.localScale = new Vector3(scaleVal, scaleVal, scaleVal);
        
	}
    
    public void AllMessageHandler(OscMessage message){
        
        Debug.Log(message);
        Debug.Log("stuff sent");
        string msgAddress = message.Address; //the message parameters
        string msgString =  Osc.OscMessageToString(message); //the message and value combined
        float myValue = (float)message.Values[0];
        
        int iValue = Mathf.RoundToInt(myValue);
        yRot = iValue;
        
        Debug.Log(msgAddress);
        
        //FUNCTIONS YOU WANT CALLED WHEN A SPECIFIC MESSAGE IS RECEIVED
	switch (msgAddress){
		case "/1/push1":
			xRot = iValue;
			break;
		case "/1/push2":
			yRot = iValue;
			break;
		case "/1/push3":
			zRot = iValue;
			break;
		case "/1/fader1":
			scaleVal = 1+iValue;
			break;
		
		default:
			//
			break;
	}

    }
}










